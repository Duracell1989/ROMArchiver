using SevenZip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomLister.Controller
{
    internal class CacheController : IDisposable
    {
        internal delegate void ProgressChangedHandler(int progress, string userFriendlyProgress);
        internal delegate void EmptyEventHandler();

        internal event ProgressChangedHandler OnProgressChanged;

        internal event EmptyEventHandler OnCacheLoadingStarted;
        internal event EmptyEventHandler OnCacheLoadingAborted;
        internal event EmptyEventHandler OnCacheLoadingCompleted;

        private BackgroundWorker _backgroundWorker;
        private string _offlineListCacheLocation = string.Empty;
        private string _romLocation = string.Empty;
        private RomType _fileType;
        internal ConcurrentBag<DatCacheEntity> _datCacheEntities;

        internal Boolean IsLoadingCache
        {
            get
            {
                return _backgroundWorker.IsBusy;
            }
        }

        internal RomType RomType
        {
            get { return _fileType; }
        }

        internal IEnumerable<DatCacheEntity> DatCacheEntities
        {
            get { return _datCacheEntities; }
        }

        internal CacheController(RomType fileType)
        {
            _datCacheEntities = new ConcurrentBag<DatCacheEntity>();

            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            _offlineListCacheLocation = ConfigReader.Instance.OfflineListCacheDirectory;
            _romLocation = ConfigReader.Instance.RomDirectory;
            _fileType = fileType;
        }

        private string GetArchivedFilePath(string archiveFilePath, string archiveExtension)
        {
            StringBuilder archivedFilePath = new StringBuilder(archiveFilePath);
            archivedFilePath.Remove(0, archiveFilePath.LastIndexOf("\\") + 1);
            archivedFilePath.Replace(archiveExtension, string.Empty);
            archivedFilePath.Append(string.Format(".{0}", _fileType.GetExtension()));
            return archivedFilePath.ToString();
        }

        internal void LoadCache()
        {
            _datCacheEntities = new ConcurrentBag<DatCacheEntity>();
            if (!_backgroundWorker.IsBusy)
            {
                Started();
                _backgroundWorker.RunWorkerAsync();
            }
        }

        internal void RefreshSingleEntity(DatCacheEntity datCacheEntity)
        {
            LoadSingleCache(datCacheEntity.FilePath, datCacheEntity);
        }


        internal void AbortLoadingCache()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }

        internal void WriteCache()
        {
            if (_datCacheEntities.Count == 0 || _offlineListCacheLocation == string.Empty)
            {
                throw new InvalidDataException();
            }

            File.WriteAllText(Path.Combine(_offlineListCacheLocation, Utils.Utils.GetCacheFileName(_fileType)), CreateFileContentForCache());
            File.WriteAllText(Path.Combine(_offlineListCacheLocation, Utils.Utils.GetRezFileName(_fileType)), CreateFileContentForRez());
        }

        private string CreateFileContentForCache()
        {
            StringBuilder output = new StringBuilder();
            foreach (DatCacheEntity datCacheEntity in _datCacheEntities.OrderBy(entity => entity.Crc))
            {
                output.Append($"{datCacheEntity.FilePath};");
                output.Append($"{datCacheEntity.FilenameInArchive};");
                output.Append($"{datCacheEntity.ModifyNo};");
                output.Append($"{datCacheEntity.CompressedFileSize};");
                output.Append($"{datCacheEntity.Crc};");
                output.Append($"{datCacheEntity.ArchiveType};");
                output.AppendLine($"{ datCacheEntity.CleanedValue};");
            }
            return output.ToString();
        }

        private string CreateFileContentForRez()
        {
            StringBuilder output = new StringBuilder();
            IEnumerable<DatCacheEntity> tempdatCacheEntity = _datCacheEntities.Where(entity => entity.IsArchived).OrderBy(entity => entity.FilePath);
            foreach (DatCacheEntity datCacheEntity in tempdatCacheEntity)
            {
                output.AppendLine($"{datCacheEntity.FilePath};{datCacheEntity.ModifyNo};{datCacheEntity.CompressedFileSize};{datCacheEntity.ArchiveType};{"9"};");
            }
            return output.ToString();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;

            if (!ProcessDirectory(bgw))
            {
                e.Cancel = true;
            }

            if (e.Cancel || !ProcessCacheFile(bgw))
            {
                e.Cancel = true;
            }

            if (e.Cancel || !ProcessRezFile(bgw))
            {
            }
        }

        private Boolean ProcessDirectory(BackgroundWorker bgw)
        {
            int progress = 0;
            string userFriendlyProgress = string.Empty;
            string filePath = string.Empty;
            string fileType = Utils.Utils.GetFileTypeValue(_fileType);

            string[] files = Directory.GetFiles(Path.Combine(_romLocation, fileType));
            //Parallel.ForEach(files, f =>
            //{
            //    if (bgw.CancellationPending)
            //    {
            //        return;
            //    }

            //    //progress = (int)((float)i / (float)files.Length * 100);
            //    //userFriendlyProgress = string.Format("{0}/{1}", i, files.Length);
            //    bgw.ReportProgress(progress, userFriendlyProgress);

            //    filePath = f;

            //    DatCacheEntity datCacheEntity = new DatCacheEntity();
            //    LoadSingleCache(filePath, datCacheEntity);

            //    _datCacheEntities.Add(datCacheEntity);
            //});

            //TODO: This is not good!

            for (int i = 0; i < files.Length; i++)
            {
                if (bgw.CancellationPending)
                {
                    return false;
                }

                progress = (int)((float)i / (float)files.Length * 100);
                userFriendlyProgress = string.Format("{0}/{1}", i, files.Length);
                bgw.ReportProgress(progress, userFriendlyProgress);

                filePath = files[i];

                DatCacheEntity datCacheEntity = new DatCacheEntity();
                LoadSingleCache(filePath, datCacheEntity);

                _datCacheEntities.Add(datCacheEntity);
            }
            return true;
        }

        private Boolean ProcessCacheFile(BackgroundWorker bgw)
        {
            string cacheFilePath = Path.Combine(_offlineListCacheLocation, Utils.Utils.GetCacheFileName(_fileType));

            if (File.Exists(cacheFilePath))
            {
                Parallel.ForEach(File.ReadLines(cacheFilePath), line => { CheckFileHasAltered(bgw.CancellationPending, line); });
            }
            return true;
        }

        private void CheckFileHasAltered(bool isCancellationPending, string line)
        {
            if (isCancellationPending)
            {
                return;
            }

            string[] subitems = line.Split(";".ToArray());

            DatCacheEntity datCacheEntity = _datCacheEntities.FirstOrDefault(x => x.FilePath == subitems[0]);
            if (datCacheEntity != null)
            {
                datCacheEntity.IsAltered = ((datCacheEntity.FilenameInArchive != subitems[1] || datCacheEntity.ModifyNo != Convert.ToUInt32(subitems[2]) || datCacheEntity.CompressedFileSize != (unchecked((Int32)Convert.ToInt64((subitems[3])))) || datCacheEntity.Crc != subitems[4]));
            }
        }

        private Boolean ProcessRezFile(BackgroundWorker bgw)
        {
            string rezFilePath = Path.Combine(_offlineListCacheLocation, Utils.Utils.GetRezFileName(_fileType));

            if (File.Exists(rezFilePath))
            {
                foreach (string item in File.ReadLines(rezFilePath).AsParallel())
                {
                    if (bgw.CancellationPending)
                    {
                        return false;
                    }

                    string[] subitems = item.Split(";".ToArray());

                    DatCacheEntity datCacheEntity = _datCacheEntities.FirstOrDefault(x => x.FilePath == subitems[0]);
                    if (datCacheEntity != null)
                    {
                        if (datCacheEntity.FilePath == subitems[0] &&
                            datCacheEntity.ModifyNo == Convert.ToUInt32(subitems[1]) &&
                            datCacheEntity.CompressedFileSize == unchecked((Int32)Convert.ToInt64(subitems[2])) &&
                            !datCacheEntity.IsAltered)
                        {
                            datCacheEntity.IsArchived = true;
                        }
                    }
                }
            }

            return true;
        }

        private void LoadSingleCache(string filePath, DatCacheEntity datCacheEntity)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string archivedFilePath = GetArchivedFilePath(fileInfo.FullName, fileInfo.Extension);

            datCacheEntity.FilePath = fileInfo.FullName;
            datCacheEntity.FilenameInArchive = archivedFilePath;
            datCacheEntity.ModifyNo = Utils.Utils.CalculateModifyNumber(fileInfo.LastWriteTimeUtc);

            datCacheEntity.CompressedFileSize = unchecked((Int32)(fileInfo.Length));
            datCacheEntity.ArchiveType = fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1);
            datCacheEntity.Cleaned = true;

            using (SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(fileInfo.FullName))
            {
                datCacheEntity.Crc = sevenZipExtractor.ArchiveFileData[0].Crc.ToString("X8");
                datCacheEntity.FileSize = sevenZipExtractor.ArchiveFileData[0].Size;
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage, (string)e.UserState);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _datCacheEntities = new ConcurrentBag<DatCacheEntity>();
                Aborted();
            }
            else
            {
                Complete();
            }
        }

        private void UpdateProgress(int progress, string userFriendlyProgress)
        {
            // Make sure someone is listening to event
            OnProgressChanged?.Invoke(progress, userFriendlyProgress);
        }

        private void Started()
        {
            // Make sure someone is listening to event
            OnCacheLoadingStarted?.Invoke();
        }

        private void Aborted()
        {
            // Make sure someone is listening to event
            OnCacheLoadingAborted?.Invoke();
        }

        private void Complete()
        {
            // Make sure someone is listening to event
            OnCacheLoadingCompleted?.Invoke();
        }

        #region "IDisposable"

        // Flag: Has Dispose already been called? 
        bool disposed = false;

        // private implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here. 
                if (_backgroundWorker != null)
                {
                    _backgroundWorker.DoWork -= BackgroundWorker_DoWork;
                    _backgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                    _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;
                    _backgroundWorker.Dispose();
                }
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        #endregion
    }
}

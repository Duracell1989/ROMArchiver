using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace RomLister.Controller
{
    internal class ArchiveController : IDisposable
    {
        internal delegate void ProgressChangedHandler(int progress, string userFriendlyProgress);
        internal delegate void EmptyEventHandler();
        internal delegate void ExceptionOccuredHandler(string errorMessage);
        internal delegate void RearchiveHandler(DatCacheEntity datCacheEntity);
        internal delegate void RearchiveOverallStartedHandler(int filesToRearchive);

        internal event ProgressChangedHandler OnProgressChanged;
        internal event ProgressChangedHandler OnOverallProgressChanged;

        internal event ExceptionOccuredHandler OnExceptionOccured;
        internal event RearchiveOverallStartedHandler OnRearchiveOverallStarted;
        internal event EmptyEventHandler OnRearchiveOverallAborted;
        internal event EmptyEventHandler OnRearchiveOverallCompleted;
        internal event RearchiveHandler OnRearchiveStarted;
        internal event RearchiveHandler OnRearchiveCompleted;

        private BackgroundWorker _backgroundWorker;

        private SevenZipProcessor _sevenZipProcessor;
        private IEnumerable<DatCacheEntity> _datCacheEntities;

        internal Boolean IsArchiving
        {
            get
            {
                return _backgroundWorker.IsBusy;
            }
        }

        internal Boolean IsAbortPending
        {
            get { return _backgroundWorker.CancellationPending; }
        }


        internal ArchiveController()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        internal void Rearchive(List<DatCacheEntity> DatCacheEntities)
        {
            if (!_backgroundWorker.IsBusy)
            {
                _datCacheEntities = DatCacheEntities;
                RearchiveOverallStarted(DatCacheEntities.Count());
                _backgroundWorker.RunWorkerAsync();
            }
        }

        internal void AbortRearchive()
        {
            if (_backgroundWorker.IsBusy)
            {
                _sevenZipProcessor.Cancel();
                _backgroundWorker.CancelAsync();
            }
        }


        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress = 0;
            int totalCount = _datCacheEntities.Count();

            BackgroundWorker bgw = (BackgroundWorker)sender;

            for (int i = 0; i < totalCount; i++)
            {
                DatCacheEntity datCacheEntity = _datCacheEntities.ElementAt(i);
                RearchiveStarted(datCacheEntity);

                progress = (int)((float)i / (float)totalCount * 100);
                OverallProgressChanged(progress, string.Format("{0}/{1}", i, totalCount));

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                _sevenZipProcessor = new SevenZipProcessor();
                _sevenZipProcessor.OnExceptionOccured += _sevenZipProcessor_OnExceptionOccured;
                _sevenZipProcessor.OnProgressChanged += _sevenZipProcessor_OnProgressChanged;
                _sevenZipProcessor.OnCompleted += _sevenZipProcessor_OnCompleted;

                IProcessInfo decomprocessInfo = new DecompressInfo(datCacheEntity.FilePath, ConfigReader.Instance.WorkingDirectory);
                _sevenZipProcessor.Run(decomprocessInfo);

                string extractedFilePath = Path.Combine(ConfigReader.Instance.WorkingDirectory, datCacheEntity.FilenameInArchive);

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                _sevenZipProcessor = new SevenZipProcessor();
                _sevenZipProcessor.OnExceptionOccured += _sevenZipProcessor_OnExceptionOccured;
                _sevenZipProcessor.OnProgressChanged += _sevenZipProcessor_OnProgressChanged;
                _sevenZipProcessor.OnCompleted += _sevenZipProcessor_OnCompleted;

                IProcessInfo compressInfo = new CompressInfo(extractedFilePath, datCacheEntity.FilePath, ConfigReader.Instance.WorkingDirectory);
                _sevenZipProcessor.Run(compressInfo);

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                datCacheEntity.IsArchived = true;
                datCacheEntity.IsAltered = false;

                RearchiveCompleted(datCacheEntity);
            }
            RearchiveOverallCompleted();
        }

        private void _sevenZipProcessor_OnExceptionOccured(string errorMessage)
        {
            _backgroundWorker.CancelAsync();
            ExceptionOccured(errorMessage);
        }

        private void _sevenZipProcessor_OnProgressChanged(int progress, string userFriendlyProgress)
        {
            ProgressChanged(progress, userFriendlyProgress);
        }

        private void _sevenZipProcessor_OnCompleted()
        {
            string foo = string.Empty;
        }


        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string foo = string.Empty;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ExceptionOccured(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                RearchiveOverallAborted();
            }
            else
            {
                RearchiveOverallCompleted();
            }
        }

        private void ExceptionOccured(string errorMessage)
        {
            // Make sure someone is listening to event
            if (OnExceptionOccured != null)
            {
                OnExceptionOccured(errorMessage);
            }
        }

        private void ProgressChanged(int progress, string userFriendlyProgress)
        {
            // Make sure someone is listening to event
            if (OnProgressChanged != null)
            {
                OnProgressChanged(progress, userFriendlyProgress);
            }
        }

        private void OverallProgressChanged(int progress, string userFriendlyProgress)
        {
            // Make sure someone is listening to event
            if (OnOverallProgressChanged != null)
            {
                OnOverallProgressChanged(progress, userFriendlyProgress);
            }
        }

        private void RearchiveOverallStarted(int filesToRearchive)
        {
            // Make sure someone is listening to event
            if (OnRearchiveOverallStarted != null)
            {
                OnRearchiveOverallStarted(filesToRearchive);
            }
        }

        private void RearchiveOverallAborted()
        {
            // Make sure someone is listening to event
            if (OnRearchiveOverallAborted != null)
            {
                OnRearchiveOverallAborted();
            }
        }

        private void RearchiveOverallCompleted()
        {
            // Make sure someone is listening to event
            if (OnRearchiveOverallCompleted != null)
            {
                OnRearchiveOverallCompleted();
            }
        }

        private void RearchiveStarted(DatCacheEntity datCacheEntity)
        {
            // Make sure someone is listening to event
            if (OnRearchiveStarted != null)
            {
                OnRearchiveStarted(datCacheEntity);
            }
        }

        private void RearchiveCompleted(DatCacheEntity datCacheEntity)
        {
            // Make sure someone is listening to event
            if (OnRearchiveCompleted != null)
            {
                OnRearchiveCompleted(datCacheEntity);
            }
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
                    _backgroundWorker.DoWork -= backgroundWorker_DoWork;
                    _backgroundWorker.ProgressChanged -= backgroundWorker_ProgressChanged;
                    _backgroundWorker.RunWorkerCompleted -= backgroundWorker_RunWorkerCompleted;
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

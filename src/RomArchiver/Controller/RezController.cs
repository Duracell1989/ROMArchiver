using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace RomLister.Controller
{
    internal class RezController
    {
        private delegate void ProgressChangedHandler(int progress, string userFriendlyProgress);
        private delegate void RezCompletedHandler();
        private event ProgressChangedHandler OnProgressChanged;
        private event RezCompletedHandler OnRezLoadCompleted;

        private string _offlineListCacheLocation = string.Empty;
        private RomType _fileType;

        private BackgroundWorker _backgroundWorker;
        private List<DatRezEntity> _datRezEntities;

        private RezController(string offlineListCacheLocation, RomType fileType)
        {
            _datRezEntities = new List<DatRezEntity>();

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            _offlineListCacheLocation = offlineListCacheLocation;
            _fileType = fileType;
        }

        private void LoadRez()
        {
            _datRezEntities.Clear();

            if (!_backgroundWorker.IsBusy)
            {
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = Path.Combine(_offlineListCacheLocation, Utils.Utils.GetRezFileName(_fileType));
            if (File.Exists(filePath))
            {
                List<string> lines = File.ReadLines(Path.Combine(_offlineListCacheLocation, Utils.Utils.GetRezFileName(_fileType))).ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    _backgroundWorker.ReportProgress((int)((float)i / (float)lines.Count * 100));

                    string[] items = lines[i].Split(';');
                    DatRezEntity datRezEntity = new DatRezEntity();

                    datRezEntity.FilePath = items[0];
                    datRezEntity.ModifyNo = Convert.ToUInt32(items[1]);
                    datRezEntity.FileSize = Convert.ToInt64(items[2]);
                    datRezEntity.ArchiveType = items[3];
                    datRezEntity.CompressionLevel = Convert.ToInt16(items[4]);

                    _datRezEntities.Add(datRezEntity);
                }
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage, (string)e.UserState);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Complete();
        }

        private void UpdateProgress(int progress, string userFriendlyProgress)
        {
            // Make sure someone is listening to event
            if (OnProgressChanged != null)
            {
                OnProgressChanged(progress, userFriendlyProgress);
            }
        }

        private void Complete()
        {
            // Make sure someone is listening to event
            if (OnRezLoadCompleted != null)
            {
                OnRezLoadCompleted();
            }
        }
    }
}
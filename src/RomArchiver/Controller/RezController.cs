using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using RomArchiver.Domain.Objects;
using RomLister;

namespace RomArchiver.Controller
{
    internal class RezController
    {
        private delegate void ProgressChangedHandler(int progress, string? userFriendlyProgress);
        private delegate void RezCompletedHandler();
        private event ProgressChangedHandler OnProgressChanged;
        private event RezCompletedHandler OnRezLoadCompleted;

        private readonly string _offlineListCacheLocation;
        private readonly RomType _fileType;

        private readonly BackgroundWorker _backgroundWorker;
        private readonly List<DatRezEntity> _datRezEntities;

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

        private void backgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var filePath = Path.Combine(_offlineListCacheLocation, RomLister.Utils.Utils.GetRezFileName(_fileType));
            if (File.Exists(filePath))
            {
                var lines = File.ReadLines(Path.Combine(_offlineListCacheLocation, RomLister.Utils.Utils.GetRezFileName(_fileType))).ToList();

                for (var i = 0; i < lines.Count; i++)
                {
                    _backgroundWorker.ReportProgress((int)(i / (float)lines.Count * 100));

                    var items = lines[i].Split(';');
                    var datRezEntity = new DatRezEntity();

                    datRezEntity.FilePath = items[0];
                    datRezEntity.ModifyNo = Convert.ToUInt32(items[1]);
                    datRezEntity.FileSize = Convert.ToInt64(items[2]);
                    datRezEntity.ArchiveType = items[3];
                    datRezEntity.CompressionLevel = Convert.ToInt16(items[4]);

                    _datRezEntities.Add(datRezEntity);
                }
            }
        }

        private void backgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage, (string)e.UserState!);
        }

        private void backgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Complete();
        }

        private void UpdateProgress(int progress, string? userFriendlyProgress)
        {
            OnProgressChanged(progress, userFriendlyProgress);
        }

        private void Complete()
        {
            OnRezLoadCompleted();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using RomArchiver.Domain;
using RomArchiver.Domain.Objects;
using static System.String;

namespace RomArchiver.Controller
{
    internal sealed class ArchiveController : IDisposable
    {
        internal delegate void ProgressChangedHandler(int progress, string userFriendlyProgress);
        internal delegate void EmptyEventHandler();
        internal delegate void ExceptionOccuredHandler(string errorMessage);
        internal delegate void RearchiveHandler(DatCacheEntity datCacheEntity);
        
        internal delegate void RearchiveCompletedHandler(DatCacheEntity datCacheEntity, long oldFileSize, long newFileSize);
        internal delegate void RearchiveOverallStartedHandler(int filesToRearchive);

        internal event ProgressChangedHandler? OnProgressChanged;
        internal event ProgressChangedHandler? OnOverallProgressChanged;

        internal event ExceptionOccuredHandler? OnExceptionOccured;
        internal event RearchiveOverallStartedHandler? OnRearchiveOverallStarted;
        internal event EmptyEventHandler? OnRearchiveOverallAborted;
        internal event EmptyEventHandler? OnRearchiveOverallCompleted;
        internal event RearchiveHandler? OnRearchiveStarted;
        internal event RearchiveCompletedHandler? OnRearchiveCompleted;

        private readonly BackgroundWorker _backgroundWorker;

        private SevenZipProcessor? _sevenZipProcessor;
        private IEnumerable<DatCacheEntity>? _datCacheEntities;

        internal bool IsArchiving => _backgroundWorker.IsBusy;

        internal bool IsAbortPending => _backgroundWorker.CancellationPending;


        internal ArchiveController()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        internal void Rearchive(List<DatCacheEntity> datCacheEntities)
        {
            if (!_backgroundWorker.IsBusy)
            {
                _datCacheEntities = datCacheEntities;
                RearchiveOverallStarted(datCacheEntities.Count);
                _backgroundWorker.RunWorkerAsync();
            }
        }

        internal void AbortRearchive()
        {
            if (_backgroundWorker.IsBusy)
            {
                _sevenZipProcessor?.Cancel();
                _backgroundWorker.CancelAsync();
            }
        }


        private void backgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            var totalCount = _datCacheEntities.Count();

            var bgw = (BackgroundWorker)sender!;

            for (var i = 0; i < totalCount; i++)
            {
                var datCacheEntity = _datCacheEntities.ElementAt(i);
                RearchiveStarted(datCacheEntity);
                
                var oldFileSize = datCacheEntity.CompressedFileSize;
                
                var progress = (int)(i / (float)totalCount * 100);
                OverallProgressChanged(progress, $"{i}/{totalCount}");

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                _sevenZipProcessor = new SevenZipProcessor();
                _sevenZipProcessor.OnExceptionOccured += _sevenZipProcessor_OnExceptionOccured;
                _sevenZipProcessor.OnProgressChanged += _sevenZipProcessor_OnProgressChanged;

                IProcessInfo decompressInfo = new DecompressInfo(datCacheEntity.FilePath, ConfigReader.Instance.WorkingDirectory);
                _sevenZipProcessor.Run(decompressInfo);

                var extractedFilePath = Path.Combine(ConfigReader.Instance.WorkingDirectory, datCacheEntity.FilenameInArchive);

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                _sevenZipProcessor = new SevenZipProcessor();
                _sevenZipProcessor.OnExceptionOccured += _sevenZipProcessor_OnExceptionOccured;
                _sevenZipProcessor.OnProgressChanged += _sevenZipProcessor_OnProgressChanged;

                IProcessInfo compressInfo = new CompressInfo(extractedFilePath, datCacheEntity.FilePath, ConfigReader.Instance.WorkingDirectory);
                _sevenZipProcessor.Run(compressInfo);

                if (bgw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                
                var newFileSize = new FileInfo(datCacheEntity.FilePath).Length;

                datCacheEntity.IsArchived = true;
                datCacheEntity.IsAltered = false;

                RearchiveCompleted(datCacheEntity, oldFileSize, newFileSize);
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

        private static void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            var foo = Empty;
        }

        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
            OnExceptionOccured?.Invoke(errorMessage);
        }

        private void ProgressChanged(int progress, string userFriendlyProgress)
        {
            OnProgressChanged?.Invoke(progress, userFriendlyProgress);
        }

        private void OverallProgressChanged(int progress, string userFriendlyProgress)
        {
            OnOverallProgressChanged?.Invoke(progress, userFriendlyProgress);
        }

        private void RearchiveOverallStarted(int filesToRearchive)
        {
            OnRearchiveOverallStarted?.Invoke(filesToRearchive);
        }

        private void RearchiveOverallAborted()
        {
            OnRearchiveOverallAborted?.Invoke();
        }

        private void RearchiveOverallCompleted()
        {
            OnRearchiveOverallCompleted?.Invoke();
        }

        private void RearchiveStarted(DatCacheEntity datCacheEntity)
        {
            OnRearchiveStarted?.Invoke(datCacheEntity);
        }

        private void RearchiveCompleted(DatCacheEntity datCacheEntity, long oldFileSize, long newFileSize)
        {
            OnRearchiveCompleted?.Invoke(datCacheEntity, oldFileSize, newFileSize);
        }

        #region "IDisposable"

        // Flag: Has Disposed already been called? 
        private bool _disposed = false;

        // private implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here. 
                if (_backgroundWorker != null)
                {
                    _backgroundWorker.DoWork -= backgroundWorker_DoWork;
                    _backgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                    _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;
                    _backgroundWorker.Dispose();
                }
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        #endregion
    }
}

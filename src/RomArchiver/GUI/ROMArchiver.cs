using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using RomArchiver.Controller;
using RomArchiver.Domain.Objects;
using RomArchiver.Utils.Extensions;

namespace RomArchiver.GUI
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class MainForm : Form
    {
        private CacheController _cacheController;
        private ArchiveController _archiveController;

        internal MainForm()
        {
            InitializeComponent();
            Text = @"Rom Archiver";
            progressInformationLabel.Text = string.Empty;
            rearchiveProgressInformationLabel.Text = string.Empty;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var romTypeDataSource = new List<KeyValuePair<RomType, string>>();
            foreach (var fileType in Enum.GetValues<RomType>())
            {
                romTypeDataSource.Add(new KeyValuePair<RomType, string>(fileType, fileType.GetUserFriendlyName()));
            }
            RomTypeComboBox.DataSource = romTypeDataSource;
            RomTypeComboBox.ValueMember = "key";
            RomTypeComboBox.DisplayMember = "value";
        }

        private void UpdateFilesToRearchive()
        {
            var filesToRearchiveCount = _cacheController.DatCacheEntities.Count(x => x.IsAltered || !x.IsArchived);
            var total = _cacheController.DatCacheEntities.Count();
            var message = $"{filesToRearchiveCount}/{total} files need rearchiving.";
            InformationTextBox.AppendTextWithNewLine(message);
        }

        private void LoadCacheButton_Click(object sender, EventArgs e)
        {
            if (_cacheController.IsLoadingCache)
            {
                _cacheController.AbortLoadingCache();
            }
            else
            {
                _cacheController.LoadCache();

            }

            UpdateLoadCacheButton();
        }

        private void UpdateLoadCacheButton()
        {
            if (_cacheController.IsLoadingCache)
            {
                loadCacheButton.Text = "Abort loading cache...";
            }
            else
            {
                loadCacheButton.Text = "Load cache";
            }
        }

        private void RearchiveButton_Click(object sender, EventArgs e)
        {
            if (!_cacheController.DatCacheEntities.Any())
            {
                MessageBox.Show("Please load cache first.");
            }
            if (_archiveController.IsArchiving)
            {
                _archiveController.AbortRearchive();
            }
            else
            {
                _archiveController.Rearchive(_cacheController.DatCacheEntities.Where(entity => !entity.IsArchived).ToList());
            }

            UpdateRearchiveButton();
        }

        private void UpdateRearchiveButton()
        {
            if (!_archiveController.IsArchiving || _archiveController.IsAbortPending)
            {
                rearchiveButton.Text = "Start archiving";
            }
            else
            {
                rearchiveButton.Text = "Abort archiving...";
            }
        }

        private void ClearRearchivingProgress()
        {
            rearchiveProgressBar.Value = 0;
            rearchiveProgressInformationLabel.Text = string.Empty;
        }

        private void SetProgressBar(int progress)
        {
            if (progressProgressBar.InvokeRequired)
            {
                progressProgressBar.Invoke(new Action<int>(SetProgressBar), progress);
                return;
            }
            progressProgressBar.Value = progress;
        }

        private void RomTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisposeCacheController();
            DisposeArchiveController();
            CreateCacheController(((KeyValuePair<RomType, string>)RomTypeComboBox.SelectedItem).Key);
            CreateArchiveController();
        }

        #region "Cache controller"

        private void CreateCacheController(RomType romTypeEnum)
        {
            _cacheController = new CacheController(romTypeEnum);
            _cacheController.OnProgressChanged += _cacheController_OnProgressChanged;
            _cacheController.OnCacheLoadingStarted += _cacheController_OnCacheLoadingStarted;
            _cacheController.OnCacheLoadingAborted += _cacheController_OnCacheLoadingAborted;
            _cacheController.OnCacheLoadingCompleted += _cacheController_OnCacheCompleted;
        }

        private void DisposeCacheController()
        {
            if (_cacheController != null)
            {
                _cacheController.OnProgressChanged -= _cacheController_OnProgressChanged;
                _cacheController.OnCacheLoadingStarted -= _cacheController_OnCacheLoadingStarted;
                _cacheController.OnCacheLoadingAborted -= _cacheController_OnCacheLoadingAborted;
                _cacheController.OnCacheLoadingCompleted -= _cacheController_OnCacheCompleted;
                _cacheController.Dispose();
            }
        }

        private void _cacheController_OnCacheLoadingStarted()
        {
            var message = $"Reading {_cacheController.RomType.GetUserFriendlyName()} cache...";
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
        }

        private void _cacheController_OnCacheLoadingAborted()
        {
            var message = $"{_cacheController.RomType.GetUserFriendlyName()} Cache loading aborted.";
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
            progressProgressBar.Value = 0;

            UpdateLoadCacheButton();
            UpdateFilesToRearchive();
        }

        private void _cacheController_OnCacheCompleted()
        {
            var message = $"{_cacheController.RomType.GetUserFriendlyName()} Cache loaded.";
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
            progressProgressBar.Value = 100;

            UpdateLoadCacheButton();
            UpdateFilesToRearchive();
            _cacheController.WriteCache();
        }

        private void _cacheController_OnProgressChanged(int progress, string? userFriendlyProgress)
        {
            var message = $"Reading {_cacheController.RomType.GetUserFriendlyName()} cache...{userFriendlyProgress}";
            progressInformationLabel.Text = message;
            progressProgressBar.Value = progress;
        }

        #endregion

        #region "Archive controller"

        private void CreateArchiveController()
        {
            _archiveController = new ArchiveController();
            _archiveController.OnExceptionOccured += ArchiveController_OnExceptionOccured;
            _archiveController.OnRearchiveStarted += ArchiveController_OnRearchiveStarted;
            _archiveController.OnRearchiveCompleted += ArchiveController_OnRearchiveCompleted;
            _archiveController.OnRearchiveOverallStarted += ArchiveController_OnRearchiveOverallStarted;
            _archiveController.OnRearchiveOverallAborted += ArchiveController_OnRearchiveOverallAborted;
            _archiveController.OnRearchiveOverallCompleted += ArchiveController_OnRearchiveOverallCompleted;
            _archiveController.OnProgressChanged += ArchiveController_OnProgressChanged;
            _archiveController.OnOverallProgressChanged += ArchiveController_OnOverallProgressChanged;
        }

        private void DisposeArchiveController()
        {
            if (_archiveController != null)
            {
                _archiveController.OnRearchiveStarted -= ArchiveController_OnRearchiveStarted;
                _archiveController.OnRearchiveCompleted -= ArchiveController_OnRearchiveCompleted;
                _archiveController.OnRearchiveOverallStarted -= ArchiveController_OnRearchiveOverallStarted;
                _archiveController.OnRearchiveOverallAborted -= ArchiveController_OnRearchiveOverallAborted;
                _archiveController.OnRearchiveOverallCompleted -= ArchiveController_OnRearchiveOverallCompleted;
                _archiveController.OnProgressChanged -= ArchiveController_OnProgressChanged;
                _archiveController.OnOverallProgressChanged -= ArchiveController_OnOverallProgressChanged;
                _archiveController.Dispose();
            }
        }


        private void ArchiveController_OnExceptionOccured(string errorMessage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ArchiveController_OnExceptionOccured), errorMessage);
                return;
            }
            InformationTextBox.AppendTextWithNewLine(errorMessage);
        }

        private void ArchiveController_OnProgressChanged(int progress, string userFriendlyProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(ArchiveController_OnProgressChanged), progress, userFriendlyProgress);
                return;
            }
            rearchiveProgressInformationLabel.Text = userFriendlyProgress;
            rearchiveProgressBar.Value = progress;
        }

        private void ArchiveController_OnOverallProgressChanged(int progress, string userFriendlyProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(ArchiveController_OnOverallProgressChanged), progress, userFriendlyProgress);
                return;
            }
            progressProgressBar.Value = progress;
            progressInformationLabel.Text = userFriendlyProgress;
        }

        private void ArchiveController_OnRearchiveStarted(DatCacheEntity datCacheEntity)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DatCacheEntity>(ArchiveController_OnRearchiveStarted), datCacheEntity);
                return;
            }

            var message = $"Re-Archiving: '{datCacheEntity.FilenameInArchive}'";
            InformationTextBox.AppendTextWithNewLine(message);
        }

        private void ArchiveController_OnRearchiveCompleted(DatCacheEntity datCacheEntity, long oldFileSize, long newFileSize)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DatCacheEntity, long, long>(ArchiveController_OnRearchiveCompleted), new object[] { datCacheEntity, oldFileSize,newFileSize });
                return;
            }

            var fileDifference = oldFileSize - newFileSize;
            var symbol = fileDifference > 0 ? "+" : string.Empty;


            var message = new StringBuilder();
            if(fileDifference != 0)
            {
                message.AppendLine($"Re-Archiving: {symbol}{fileDifference:n0}bytes.");
                message.AppendLine($"Re-Archiving: From {oldFileSize:n0}bytes to {newFileSize:n0} bytes. {symbol}{fileDifference:n0}bytes.");
            }
            else
            {
                message.AppendLine($"Re-Archiving: No size changed. Still: {symbol}{newFileSize:n0}bytes.");
            }

            message.AppendLine($"Re-Archiving: '{datCacheEntity.FilenameInArchive}' done.");
            InformationTextBox.AppendTextWithNewLine(message.ToString());
            _cacheController.RefreshSingleEntity(datCacheEntity);
        }


        private void ArchiveController_OnRearchiveOverallStarted(int filesToRearchive)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(ArchiveController_OnRearchiveOverallStarted), filesToRearchive);
                return;
            }

            const string message = "Rearchive started.";
            InformationTextBox.AppendTextWithNewLine(message);
            InformationTextBox.AppendTextWithNewLine($"{filesToRearchive} files to re-archive.");

            progressProgressBar.Value = 0;
            progressInformationLabel.Text = message;
        }

        private void ArchiveController_OnRearchiveOverallAborted()
        {
            if (InvokeRequired)
            {
                Invoke(ArchiveController_OnRearchiveOverallAborted);
                return;
            }
            
            const string message = "Re-Archiving: aborted.";
            InformationTextBox.AppendTextWithNewLine(message);
            ClearRearchivingProgress();
            UpdateRearchiveButton();

            _cacheController.WriteCache();
        }

        private void ArchiveController_OnRearchiveOverallCompleted()
        {
            if (InvokeRequired)
            {
                Invoke(ArchiveController_OnRearchiveOverallCompleted);
                return;
            }
            
            const string message = "Re-Archiving: done.";
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
            progressProgressBar.Value = 100;
            ClearRearchivingProgress();
            UpdateRearchiveButton();

            _cacheController.WriteCache();
        }

        #endregion
    }
}

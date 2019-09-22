using RomLister.Controller;
using RomLister.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RomLister
{
    internal partial class MainForm : Form
    {
        private CacheController _cacheController;
        private ArchiveController _archiveController;

        internal MainForm()
        {
            InitializeComponent();
            this.Text = $"Rom Archiver {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            List<KeyValuePair<RomType, string>> romTypeDataSource = new List<KeyValuePair<RomType, string>>();
            foreach (RomType fileType in Enum.GetValues(typeof(RomType)))
            {
                romTypeDataSource.Add(new KeyValuePair<RomType, string>(fileType, fileType.GetUserFriendlyName()));
            }
            RomTypeComboBox.DataSource = romTypeDataSource;
            RomTypeComboBox.ValueMember = "key";
            RomTypeComboBox.DisplayMember = "value";
        }

        private void UpdateFilesToRearchve()
        {
            int filesToRearchiveCount = _cacheController.DatCacheEntities.Count(x => x.IsAltered || !x.IsArchived);
            int total = _cacheController.DatCacheEntities.Count();
            string message = string.Format("{0}/{1} files need rearchiving.", filesToRearchiveCount, total);
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
            if (_cacheController == null || _cacheController.DatCacheEntities.Count() == 0)
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
                progressProgressBar.Invoke(new Action<int>(SetProgressBar), new object[] { progress });
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
            string message = string.Format("Reading {0} cache...", _cacheController.RomType.GetUserFriendlyName());
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
        }

        private void _cacheController_OnCacheLoadingAborted()
        {
            string message = string.Format("{0} Cache loading aborted.", _cacheController.RomType.GetUserFriendlyName());
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
            progressProgressBar.Value = 0;

            UpdateLoadCacheButton();
            UpdateFilesToRearchve();
        }

        private void _cacheController_OnCacheCompleted()
        {
            string message = string.Format("{0} Cache loaded.", _cacheController.RomType.GetUserFriendlyName());
            InformationTextBox.AppendTextWithNewLine(message);
            progressInformationLabel.Text = message;
            progressProgressBar.Value = 100;

            UpdateLoadCacheButton();
            UpdateFilesToRearchve();
            _cacheController.WriteCache();
        }

        private void _cacheController_OnProgressChanged(int progress, string userFriendlyProgress)
        {
            string message = string.Format("Reading {0} cache...{1}", _cacheController.RomType.GetUserFriendlyName(), userFriendlyProgress);
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
                Invoke(new Action<string>(ArchiveController_OnExceptionOccured), new object[] { errorMessage });
                return;
            }
            InformationTextBox.AppendTextWithNewLine(errorMessage);
        }

        private void ArchiveController_OnProgressChanged(int progress, string userFriendlyProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(ArchiveController_OnProgressChanged), new object[] { progress, userFriendlyProgress });
                return;
            }
            rearchiveProgressInformationLabel.Text = userFriendlyProgress;
            rearchiveProgressBar.Value = progress;
        }

        private void ArchiveController_OnOverallProgressChanged(int progress, string userFriendlyProgress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(ArchiveController_OnOverallProgressChanged), new object[] { progress, userFriendlyProgress });
                return;
            }
            progressProgressBar.Value = progress;
            progressInformationLabel.Text = userFriendlyProgress;
        }

        private void ArchiveController_OnRearchiveStarted(DatCacheEntity datCacheEntity)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DatCacheEntity>(ArchiveController_OnRearchiveStarted), new object[] { datCacheEntity });
                return;
            }

            string message = string.Format("Re-Archiving: '{0}'", datCacheEntity.FilenameInArchive);
            InformationTextBox.AppendTextWithNewLine(message);
        }

        private void ArchiveController_OnRearchiveCompleted(DatCacheEntity datCacheEntity)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DatCacheEntity>(ArchiveController_OnRearchiveCompleted), new object[] { datCacheEntity });
                return;
            }

            string message = string.Format("Re-Archiving: '{0}' done.", datCacheEntity.FilenameInArchive);
            InformationTextBox.AppendTextWithNewLine(message);
            _cacheController.RefreshSingleEntity(datCacheEntity);
        }


        private void ArchiveController_OnRearchiveOverallStarted(int filesToRearchive)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(ArchiveController_OnRearchiveOverallStarted), new object[] { filesToRearchive });
                return;
            }

            string message = "Rearchive started.";
            InformationTextBox.AppendTextWithNewLine(message);
            InformationTextBox.AppendTextWithNewLine(string.Format("{0} files to re-archive.", filesToRearchive));

            progressProgressBar.Value = 0;
            progressInformationLabel.Text = message;
        }

        private void ArchiveController_OnRearchiveOverallAborted()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ArchiveController_OnRearchiveOverallAborted));
                return;
            }
            string message = string.Format("Re-Archiving: aborted.");
            InformationTextBox.AppendTextWithNewLine(message);
            ClearRearchivingProgress();
            UpdateRearchiveButton();

            _cacheController.WriteCache();
        }

        private void ArchiveController_OnRearchiveOverallCompleted()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ArchiveController_OnRearchiveOverallCompleted));
                return;
            }
            string message = string.Format("Re-Archiving: done.");
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

namespace RomArchiver.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            loadCacheButton = new System.Windows.Forms.Button();
            progressInformationLabel = new System.Windows.Forms.Label();
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            InformationTextBox = new System.Windows.Forms.TextBox();
            rearchiveButton = new System.Windows.Forms.Button();
            RomTypeComboBox = new System.Windows.Forms.ComboBox();
            progressProgressBar = new System.Windows.Forms.ProgressBar();
            rearchiveProgressBar = new System.Windows.Forms.ProgressBar();
            rearchiveProgressInformationLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // loadCacheButton
            // 
            loadCacheButton.Location = new System.Drawing.Point(14, 14);
            loadCacheButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            loadCacheButton.Name = "loadCacheButton";
            loadCacheButton.Size = new System.Drawing.Size(182, 27);
            loadCacheButton.TabIndex = 0;
            loadCacheButton.Text = "Load Cache";
            loadCacheButton.UseVisualStyleBackColor = true;
            loadCacheButton.Click += LoadCacheButton_Click;
            // 
            // progressInformationLabel
            // 
            progressInformationLabel.AutoSize = true;
            progressInformationLabel.Location = new System.Drawing.Point(14, 53);
            progressInformationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            progressInformationLabel.Name = "progressInformationLabel";
            progressInformationLabel.Size = new System.Drawing.Size(41, 15);
            progressInformationLabel.TabIndex = 4;
            progressInformationLabel.Text = "empty";
            // 
            // InformationTextBox
            // 
            InformationTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            InformationTextBox.Location = new System.Drawing.Point(14, 146);
            InformationTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InformationTextBox.Multiline = true;
            InformationTextBox.Name = "InformationTextBox";
            InformationTextBox.ReadOnly = true;
            InformationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            InformationTextBox.Size = new System.Drawing.Size(476, 289);
            InformationTextBox.TabIndex = 2;
            InformationTextBox.WordWrap = false;
            // 
            // rearchiveButton
            // 
            rearchiveButton.Location = new System.Drawing.Point(14, 80);
            rearchiveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rearchiveButton.Name = "rearchiveButton";
            rearchiveButton.Size = new System.Drawing.Size(117, 27);
            rearchiveButton.TabIndex = 5;
            rearchiveButton.Text = "Start archiving";
            rearchiveButton.UseVisualStyleBackColor = true;
            rearchiveButton.Click += RearchiveButton_Click;
            // 
            // RomTypeComboBox
            // 
            RomTypeComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RomTypeComboBox.FormattingEnabled = true;
            RomTypeComboBox.Location = new System.Drawing.Point(211, 16);
            RomTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RomTypeComboBox.Name = "RomTypeComboBox";
            RomTypeComboBox.Size = new System.Drawing.Size(279, 23);
            RomTypeComboBox.TabIndex = 1;
            RomTypeComboBox.SelectedIndexChanged += RomTypeComboBox_SelectedIndexChanged;
            // 
            // progressProgressBar
            // 
            progressProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressProgressBar.Location = new System.Drawing.Point(14, 47);
            progressProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            progressProgressBar.Name = "progressProgressBar";
            progressProgressBar.Size = new System.Drawing.Size(476, 27);
            progressProgressBar.TabIndex = 3;
            // 
            // rearchiveProgressBar
            // 
            rearchiveProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rearchiveProgressBar.Location = new System.Drawing.Point(13, 113);
            rearchiveProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rearchiveProgressBar.Name = "rearchiveProgressBar";
            rearchiveProgressBar.Size = new System.Drawing.Size(476, 27);
            rearchiveProgressBar.TabIndex = 6;
            // 
            // rearchiveProgressInformationLabel
            // 
            rearchiveProgressInformationLabel.AutoSize = true;
            rearchiveProgressInformationLabel.Location = new System.Drawing.Point(14, 119);
            rearchiveProgressInformationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            rearchiveProgressInformationLabel.Name = "rearchiveProgressInformationLabel";
            rearchiveProgressInformationLabel.Size = new System.Drawing.Size(41, 15);
            rearchiveProgressInformationLabel.TabIndex = 7;
            rearchiveProgressInformationLabel.Text = "empty";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(502, 443);
            Controls.Add(rearchiveProgressInformationLabel);
            Controls.Add(RomTypeComboBox);
            Controls.Add(rearchiveButton);
            Controls.Add(InformationTextBox);
            Controls.Add(progressInformationLabel);
            Controls.Add(loadCacheButton);
            Controls.Add(rearchiveProgressBar);
            Controls.Add(progressProgressBar);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(382, 386);
            Name = "MainForm";
            Text = "Rom Archiver";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button loadCacheButton;
        private System.Windows.Forms.Label progressInformationLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox InformationTextBox;
        private System.Windows.Forms.Button rearchiveButton;
        private System.Windows.Forms.ComboBox RomTypeComboBox;
        private System.Windows.Forms.ProgressBar progressProgressBar;
        private System.Windows.Forms.ProgressBar rearchiveProgressBar;
        private System.Windows.Forms.Label rearchiveProgressInformationLabel;
    }
}


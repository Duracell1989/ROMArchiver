namespace RomLister
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.loadCacheButton = new System.Windows.Forms.Button();
            this.progressInformationLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.InformationTextBox = new System.Windows.Forms.TextBox();
            this.rearchiveButton = new System.Windows.Forms.Button();
            this.RomTypeComboBox = new System.Windows.Forms.ComboBox();
            this.progressProgressBar = new System.Windows.Forms.ProgressBar();
            this.rearchiveProgressBar = new System.Windows.Forms.ProgressBar();
            this.rearchiveProgressInformationLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // loadCacheButton
            // 
            this.loadCacheButton.Location = new System.Drawing.Point(12, 12);
            this.loadCacheButton.Name = "loadCacheButton";
            this.loadCacheButton.Size = new System.Drawing.Size(156, 23);
            this.loadCacheButton.TabIndex = 0;
            this.loadCacheButton.Text = "Load Cache";
            this.loadCacheButton.UseVisualStyleBackColor = true;
            this.loadCacheButton.Click += new System.EventHandler(this.LoadCacheButton_Click);
            // 
            // progressInformationLabel
            // 
            this.progressInformationLabel.AutoSize = true;
            this.progressInformationLabel.Location = new System.Drawing.Point(12, 203);
            this.progressInformationLabel.Name = "progressInformationLabel";
            this.progressInformationLabel.Size = new System.Drawing.Size(0, 13);
            this.progressInformationLabel.TabIndex = 4;
            // 
            // InformationTextBox
            // 
            this.InformationTextBox.Location = new System.Drawing.Point(12, 41);
            this.InformationTextBox.Multiline = true;
            this.InformationTextBox.Name = "InformationTextBox";
            this.InformationTextBox.ReadOnly = true;
            this.InformationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InformationTextBox.Size = new System.Drawing.Size(290, 130);
            this.InformationTextBox.TabIndex = 2;
            this.InformationTextBox.WordWrap = false;
            // 
            // rearchiveButton
            // 
            this.rearchiveButton.Location = new System.Drawing.Point(12, 219);
            this.rearchiveButton.Name = "rearchiveButton";
            this.rearchiveButton.Size = new System.Drawing.Size(100, 23);
            this.rearchiveButton.TabIndex = 5;
            this.rearchiveButton.Text = "Start archiving";
            this.rearchiveButton.UseVisualStyleBackColor = true;
            this.rearchiveButton.Click += new System.EventHandler(this.RearchiveButton_Click);
            // 
            // RomTypeComboBox
            // 
            this.RomTypeComboBox.FormattingEnabled = true;
            this.RomTypeComboBox.Location = new System.Drawing.Point(181, 14);
            this.RomTypeComboBox.Name = "RomTypeComboBox";
            this.RomTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.RomTypeComboBox.TabIndex = 1;
            this.RomTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.RomTypeComboBox_SelectedIndexChanged);
            // 
            // progressProgressBar
            // 
            this.progressProgressBar.Location = new System.Drawing.Point(12, 177);
            this.progressProgressBar.Name = "progressProgressBar";
            this.progressProgressBar.Size = new System.Drawing.Size(290, 23);
            this.progressProgressBar.TabIndex = 3;
            // 
            // rearchiveProgressBar
            // 
            this.rearchiveProgressBar.Location = new System.Drawing.Point(12, 248);
            this.rearchiveProgressBar.Name = "rearchiveProgressBar";
            this.rearchiveProgressBar.Size = new System.Drawing.Size(290, 23);
            this.rearchiveProgressBar.TabIndex = 6;
            // 
            // rearchiveProgressInformationLabel
            // 
            this.rearchiveProgressInformationLabel.AutoSize = true;
            this.rearchiveProgressInformationLabel.Location = new System.Drawing.Point(12, 274);
            this.rearchiveProgressInformationLabel.Name = "rearchiveProgressInformationLabel";
            this.rearchiveProgressInformationLabel.Size = new System.Drawing.Size(0, 13);
            this.rearchiveProgressInformationLabel.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 302);
            this.Controls.Add(this.rearchiveProgressInformationLabel);
            this.Controls.Add(this.rearchiveProgressBar);
            this.Controls.Add(this.progressProgressBar);
            this.Controls.Add(this.RomTypeComboBox);
            this.Controls.Add(this.rearchiveButton);
            this.Controls.Add(this.InformationTextBox);
            this.Controls.Add(this.progressInformationLabel);
            this.Controls.Add(this.loadCacheButton);
            this.MinimumSize = new System.Drawing.Size(330, 340);
            this.Name = "MainForm";
            this.Text = "Rom Archiver";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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


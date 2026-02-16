namespace RedshiftGuardianNET.Forms
{
    partial class ClusterEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelHost = new System.Windows.Forms.Label();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.numericPort = new System.Windows.Forms.NumericUpDown();
            this.labelDatabase = new System.Windows.Forms.Label();
            this.textBoxDatabase = new System.Windows.Forms.TextBox();
            this.labelClusterType = new System.Windows.Forms.Label();
            this.comboClusterType = new System.Windows.Forms.ComboBox();
            this.labelRegion = new System.Windows.Forms.Label();
            this.textBoxRegion = new System.Windows.Forms.TextBox();
            this.labelAwsProfile = new System.Windows.Forms.Label();
            this.textBoxAwsProfile = new System.Windows.Forms.TextBox();
            this.checkBoxUseIAM = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonTestConnection = new System.Windows.Forms.Button();
            this.buttonSetDefaults = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.groupBoxAWS = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).BeginInit();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxAWS.SuspendLayout();
            this.SuspendLayout();
            //
            // labelName
            //
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(15, 25);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(77, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Cluster Name:";
            //
            // textBoxName
            //
            this.textBoxName.Location = new System.Drawing.Point(110, 22);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(380, 20);
            this.textBoxName.TabIndex = 1;
            //
            // labelHost
            //
            this.labelHost.AutoSize = true;
            this.labelHost.Location = new System.Drawing.Point(15, 51);
            this.labelHost.Name = "labelHost";
            this.labelHost.Size = new System.Drawing.Size(32, 13);
            this.labelHost.TabIndex = 2;
            this.labelHost.Text = "Host:";
            //
            // textBoxHost
            //
            this.textBoxHost.Location = new System.Drawing.Point(110, 48);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(380, 20);
            this.textBoxHost.TabIndex = 3;
            //
            // labelPort
            //
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(15, 77);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(29, 13);
            this.labelPort.TabIndex = 4;
            this.labelPort.Text = "Port:";
            //
            // numericPort
            //
            this.numericPort.Location = new System.Drawing.Point(110, 75);
            this.numericPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericPort.Name = "numericPort";
            this.numericPort.Size = new System.Drawing.Size(120, 20);
            this.numericPort.TabIndex = 5;
            this.numericPort.Value = new decimal(new int[] {
            5439,
            0,
            0,
            0});
            //
            // labelDatabase
            //
            this.labelDatabase.AutoSize = true;
            this.labelDatabase.Location = new System.Drawing.Point(15, 103);
            this.labelDatabase.Name = "labelDatabase";
            this.labelDatabase.Size = new System.Drawing.Size(56, 13);
            this.labelDatabase.TabIndex = 6;
            this.labelDatabase.Text = "Database:";
            //
            // textBoxDatabase
            //
            this.textBoxDatabase.Location = new System.Drawing.Point(110, 100);
            this.textBoxDatabase.Name = "textBoxDatabase";
            this.textBoxDatabase.Size = new System.Drawing.Size(380, 20);
            this.textBoxDatabase.TabIndex = 7;
            //
            // labelClusterType
            //
            this.labelClusterType.AutoSize = true;
            this.labelClusterType.Location = new System.Drawing.Point(15, 129);
            this.labelClusterType.Name = "labelClusterType";
            this.labelClusterType.Size = new System.Drawing.Size(71, 13);
            this.labelClusterType.TabIndex = 8;
            this.labelClusterType.Text = "Cluster Type:";
            //
            // comboClusterType
            //
            this.comboClusterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClusterType.FormattingEnabled = true;
            this.comboClusterType.Location = new System.Drawing.Point(110, 126);
            this.comboClusterType.Name = "comboClusterType";
            this.comboClusterType.Size = new System.Drawing.Size(200, 21);
            this.comboClusterType.TabIndex = 9;
            //
            // labelRegion
            //
            this.labelRegion.AutoSize = true;
            this.labelRegion.Location = new System.Drawing.Point(15, 30);
            this.labelRegion.Name = "labelRegion";
            this.labelRegion.Size = new System.Drawing.Size(44, 13);
            this.labelRegion.TabIndex = 0;
            this.labelRegion.Text = "Region:";
            //
            // textBoxRegion
            //
            this.textBoxRegion.Location = new System.Drawing.Point(110, 27);
            this.textBoxRegion.Name = "textBoxRegion";
            this.textBoxRegion.Size = new System.Drawing.Size(200, 20);
            this.textBoxRegion.TabIndex = 1;
            //
            // labelAwsProfile
            //
            this.labelAwsProfile.AutoSize = true;
            this.labelAwsProfile.Location = new System.Drawing.Point(15, 56);
            this.labelAwsProfile.Name = "labelAwsProfile";
            this.labelAwsProfile.Size = new System.Drawing.Size(67, 13);
            this.labelAwsProfile.TabIndex = 2;
            this.labelAwsProfile.Text = "AWS Profile:";
            //
            // textBoxAwsProfile
            //
            this.textBoxAwsProfile.Location = new System.Drawing.Point(110, 53);
            this.textBoxAwsProfile.Name = "textBoxAwsProfile";
            this.textBoxAwsProfile.Size = new System.Drawing.Size(200, 20);
            this.textBoxAwsProfile.TabIndex = 3;
            //
            // checkBoxUseIAM
            //
            this.checkBoxUseIAM.AutoSize = true;
            this.checkBoxUseIAM.Checked = true;
            this.checkBoxUseIAM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseIAM.Location = new System.Drawing.Point(18, 85);
            this.checkBoxUseIAM.Name = "checkBoxUseIAM";
            this.checkBoxUseIAM.Size = new System.Drawing.Size(143, 17);
            this.checkBoxUseIAM.TabIndex = 4;
            this.checkBoxUseIAM.Text = "Use IAM Authentication";
            this.checkBoxUseIAM.UseVisualStyleBackColor = true;
            this.checkBoxUseIAM.CheckedChanged += new System.EventHandler(this.checkBoxUseIAM_CheckedChanged);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(275, 430);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // buttonCancel
            //
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(390, 430);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            //
            // buttonTestConnection
            //
            this.buttonTestConnection.Location = new System.Drawing.Point(18, 430);
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(120, 30);
            this.buttonTestConnection.TabIndex = 12;
            this.buttonTestConnection.Text = "Test Connection";
            this.buttonTestConnection.UseVisualStyleBackColor = true;
            this.buttonTestConnection.Click += new System.EventHandler(this.buttonTestConnection_Click);
            //
            // buttonSetDefaults
            //
            this.buttonSetDefaults.Location = new System.Drawing.Point(148, 430);
            this.buttonSetDefaults.Name = "buttonSetDefaults";
            this.buttonSetDefaults.Size = new System.Drawing.Size(100, 30);
            this.buttonSetDefaults.TabIndex = 13;
            this.buttonSetDefaults.Text = "Set Defaults";
            this.buttonSetDefaults.UseVisualStyleBackColor = true;
            this.buttonSetDefaults.Click += new System.EventHandler(this.buttonSetDefaults_Click);
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(18, 405);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 11;
            //
            // groupBoxConnection
            //
            this.groupBoxConnection.Controls.Add(this.labelName);
            this.groupBoxConnection.Controls.Add(this.textBoxName);
            this.groupBoxConnection.Controls.Add(this.labelHost);
            this.groupBoxConnection.Controls.Add(this.textBoxHost);
            this.groupBoxConnection.Controls.Add(this.labelPort);
            this.groupBoxConnection.Controls.Add(this.numericPort);
            this.groupBoxConnection.Controls.Add(this.labelDatabase);
            this.groupBoxConnection.Controls.Add(this.textBoxDatabase);
            this.groupBoxConnection.Controls.Add(this.labelClusterType);
            this.groupBoxConnection.Controls.Add(this.comboClusterType);
            this.groupBoxConnection.Location = new System.Drawing.Point(18, 18);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(510, 170);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection Settings";
            //
            // groupBoxAWS
            //
            this.groupBoxAWS.Controls.Add(this.labelRegion);
            this.groupBoxAWS.Controls.Add(this.textBoxRegion);
            this.groupBoxAWS.Controls.Add(this.labelAwsProfile);
            this.groupBoxAWS.Controls.Add(this.textBoxAwsProfile);
            this.groupBoxAWS.Controls.Add(this.checkBoxUseIAM);
            this.groupBoxAWS.Location = new System.Drawing.Point(18, 205);
            this.groupBoxAWS.Name = "groupBoxAWS";
            this.groupBoxAWS.Size = new System.Drawing.Size(510, 120);
            this.groupBoxAWS.TabIndex = 10;
            this.groupBoxAWS.TabStop = false;
            this.groupBoxAWS.Text = "AWS Settings";
            //
            // ClusterEditForm
            //
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(544, 477);
            this.Controls.Add(this.groupBoxAWS);
            this.Controls.Add(this.groupBoxConnection);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonSetDefaults);
            this.Controls.Add(this.buttonTestConnection);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClusterEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cluster Editor";
            ((System.ComponentModel.ISupportInitialize)(this.numericPort)).EndInit();
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxAWS.ResumeLayout(false);
            this.groupBoxAWS.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelHost;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.NumericUpDown numericPort;
        private System.Windows.Forms.Label labelDatabase;
        private System.Windows.Forms.TextBox textBoxDatabase;
        private System.Windows.Forms.Label labelClusterType;
        private System.Windows.Forms.ComboBox comboClusterType;
        private System.Windows.Forms.Label labelRegion;
        private System.Windows.Forms.TextBox textBoxRegion;
        private System.Windows.Forms.Label labelAwsProfile;
        private System.Windows.Forms.TextBox textBoxAwsProfile;
        private System.Windows.Forms.CheckBox checkBoxUseIAM;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonTestConnection;
        private System.Windows.Forms.Button buttonSetDefaults;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.GroupBox groupBoxAWS;
    }
}

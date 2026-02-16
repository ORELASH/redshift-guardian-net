namespace RedshiftGuardianNET.Forms
{
    partial class PermissionsViewForm
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxUsers = new System.Windows.Forms.GroupBox();
            this.dataGridViewUsers = new System.Windows.Forms.DataGridView();
            this.groupBoxPermissions = new System.Windows.Forms.GroupBox();
            this.dataGridViewPermissions = new System.Windows.Forms.DataGridView();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelFilter = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonExportUsers = new System.Windows.Forms.Button();
            this.buttonExportPermissions = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).BeginInit();
            this.groupBoxPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPermissions)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 50);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer.Panel1
            //
            this.splitContainer.Panel1.Controls.Add(this.groupBoxUsers);
            //
            // splitContainer.Panel2
            //
            this.splitContainer.Panel2.Controls.Add(this.groupBoxPermissions);
            this.splitContainer.Size = new System.Drawing.Size(1000, 550);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 1;
            //
            // groupBoxUsers
            //
            this.groupBoxUsers.Controls.Add(this.dataGridViewUsers);
            this.groupBoxUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxUsers.Location = new System.Drawing.Point(0, 0);
            this.groupBoxUsers.Name = "groupBoxUsers";
            this.groupBoxUsers.Padding = new System.Windows.Forms.Padding(5);
            this.groupBoxUsers.Size = new System.Drawing.Size(1000, 250);
            this.groupBoxUsers.TabIndex = 0;
            this.groupBoxUsers.TabStop = false;
            this.groupBoxUsers.Text = "Users";
            //
            // dataGridViewUsers
            //
            this.dataGridViewUsers.AllowUserToAddRows = false;
            this.dataGridViewUsers.AllowUserToDeleteRows = false;
            this.dataGridViewUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewUsers.Location = new System.Drawing.Point(5, 18);
            this.dataGridViewUsers.MultiSelect = false;
            this.dataGridViewUsers.Name = "dataGridViewUsers";
            this.dataGridViewUsers.ReadOnly = true;
            this.dataGridViewUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUsers.Size = new System.Drawing.Size(990, 227);
            this.dataGridViewUsers.TabIndex = 0;
            //
            // groupBoxPermissions
            //
            this.groupBoxPermissions.Controls.Add(this.dataGridViewPermissions);
            this.groupBoxPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPermissions.Location = new System.Drawing.Point(0, 0);
            this.groupBoxPermissions.Name = "groupBoxPermissions";
            this.groupBoxPermissions.Padding = new System.Windows.Forms.Padding(5);
            this.groupBoxPermissions.Size = new System.Drawing.Size(1000, 296);
            this.groupBoxPermissions.TabIndex = 0;
            this.groupBoxPermissions.TabStop = false;
            this.groupBoxPermissions.Text = "Table Permissions";
            //
            // dataGridViewPermissions
            //
            this.dataGridViewPermissions.AllowUserToAddRows = false;
            this.dataGridViewPermissions.AllowUserToDeleteRows = false;
            this.dataGridViewPermissions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPermissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPermissions.Location = new System.Drawing.Point(5, 18);
            this.dataGridViewPermissions.MultiSelect = false;
            this.dataGridViewPermissions.Name = "dataGridViewPermissions";
            this.dataGridViewPermissions.ReadOnly = true;
            this.dataGridViewPermissions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPermissions.Size = new System.Drawing.Size(990, 273);
            this.dataGridViewPermissions.TabIndex = 0;
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.labelFilter);
            this.panelTop.Controls.Add(this.textBoxFilter);
            this.panelTop.Controls.Add(this.buttonRefresh);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            this.panelTop.Size = new System.Drawing.Size(1000, 50);
            this.panelTop.TabIndex = 0;
            //
            // labelFilter
            //
            this.labelFilter.AutoSize = true;
            this.labelFilter.Location = new System.Drawing.Point(13, 18);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(32, 13);
            this.labelFilter.TabIndex = 0;
            this.labelFilter.Text = "Filter:";
            //
            // textBoxFilter
            //
            this.textBoxFilter.Location = new System.Drawing.Point(55, 15);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(300, 20);
            this.textBoxFilter.TabIndex = 1;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(370, 13);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(80, 25);
            this.buttonRefresh.TabIndex = 2;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.buttonExportUsers);
            this.panelBottom.Controls.Add(this.buttonExportPermissions);
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 600);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(10);
            this.panelBottom.Size = new System.Drawing.Size(1000, 50);
            this.panelBottom.TabIndex = 2;
            //
            // buttonExportUsers
            //
            this.buttonExportUsers.Location = new System.Drawing.Point(13, 13);
            this.buttonExportUsers.Name = "buttonExportUsers";
            this.buttonExportUsers.Size = new System.Drawing.Size(120, 25);
            this.buttonExportUsers.TabIndex = 0;
            this.buttonExportUsers.Text = "Export Users";
            this.buttonExportUsers.UseVisualStyleBackColor = true;
            this.buttonExportUsers.Click += new System.EventHandler(this.buttonExportUsers_Click);
            //
            // buttonExportPermissions
            //
            this.buttonExportPermissions.Location = new System.Drawing.Point(148, 13);
            this.buttonExportPermissions.Name = "buttonExportPermissions";
            this.buttonExportPermissions.Size = new System.Drawing.Size(140, 25);
            this.buttonExportPermissions.TabIndex = 1;
            this.buttonExportPermissions.Text = "Export Permissions";
            this.buttonExportPermissions.UseVisualStyleBackColor = true;
            this.buttonExportPermissions.Click += new System.EventHandler(this.buttonExportPermissions_Click);
            //
            // buttonClose
            //
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(897, 13);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(90, 25);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 650);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(109, 17);
            this.statusLabel.Text = "Users: 0  |  Permissions: 0";
            //
            // PermissionsViewForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 672);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelTop);
            this.Name = "PermissionsViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Permissions Viewer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.groupBoxUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUsers)).EndInit();
            this.groupBoxPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPermissions)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBoxUsers;
        private System.Windows.Forms.DataGridView dataGridViewUsers;
        private System.Windows.Forms.GroupBox groupBoxPermissions;
        private System.Windows.Forms.DataGridView dataGridViewPermissions;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelFilter;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonExportUsers;
        private System.Windows.Forms.Button buttonExportPermissions;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}

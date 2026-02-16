namespace RedshiftGuardianNET.Forms
{
    partial class DashboardForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemQueryEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemQueryEditorHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSQLReference = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonEdit = new System.Windows.Forms.ToolStripButton();
            this.buttonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonScan = new System.Windows.Forms.ToolStripButton();
            this.buttonViewPermissions = new System.Windows.Forms.ToolStripButton();
            this.buttonTestConnection = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonRefresh = new System.Windows.Forms.ToolStripButton();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.dataGridViewClusters = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClusters)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1000, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            //
            // fileToolStripMenuItem
            //
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            //
            // menuItemExit
            //
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(92, 22);
            this.menuItemExit.Text = "E&xit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            //
            // toolsToolStripMenuItem
            //
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemQueryEditor,
            this.menuItemPermissionManagement,
            this.menuItemSettings});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            //
            // menuItemQueryEditor
            //
            this.menuItemQueryEditor.Name = "menuItemQueryEditor";
            this.menuItemQueryEditor.Size = new System.Drawing.Size(180, 22);
            this.menuItemQueryEditor.Text = "SQL Query &Editor";
            this.menuItemQueryEditor.Click += new System.EventHandler(this.menuItemQueryEditor_Click);
            //
            // menuItemSettings
            //
            this.menuItemSettings.Name = "menuItemSettings";
            this.menuItemSettings.Size = new System.Drawing.Size(220, 22);
            this.menuItemSettings.Text = "&Settings...";
            this.menuItemSettings.Click += new System.EventHandler(this.menuItemSettings_Click);
            //
            // menuItemPermissionManagement
            //
            this.menuItemPermissionManagement.Name = "menuItemPermissionManagement";
            this.menuItemPermissionManagement.Size = new System.Drawing.Size(220, 22);
            this.menuItemPermissionManagement.Text = "&Permission Management...";
            this.menuItemPermissionManagement.Click += new System.EventHandler(this.menuItemPermissionManagement_Click);
            //
            // helpToolStripMenuItem
            //
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHelpContents,
            this.menuItemQueryEditorHelp,
            this.menuItemSQLReference,
            this.menuItemSeparator,
            this.menuItemAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            //
            // menuItemHelpContents
            //
            this.menuItemHelpContents.Name = "menuItemHelpContents";
            this.menuItemHelpContents.Size = new System.Drawing.Size(200, 22);
            this.menuItemHelpContents.Text = "&Help Contents";
            this.menuItemHelpContents.Click += new System.EventHandler(this.menuItemHelpContents_Click);
            //
            // menuItemQueryEditorHelp
            //
            this.menuItemQueryEditorHelp.Name = "menuItemQueryEditorHelp";
            this.menuItemQueryEditorHelp.Size = new System.Drawing.Size(200, 22);
            this.menuItemQueryEditorHelp.Text = "&Query Editor Help";
            this.menuItemQueryEditorHelp.Click += new System.EventHandler(this.menuItemQueryEditorHelp_Click);
            //
            // menuItemSQLReference
            //
            this.menuItemSQLReference.Name = "menuItemSQLReference";
            this.menuItemSQLReference.Size = new System.Drawing.Size(200, 22);
            this.menuItemSQLReference.Text = "&SQL Reference";
            this.menuItemSQLReference.Click += new System.EventHandler(this.menuItemSQLReference_Click);
            //
            // menuItemSeparator
            //
            this.menuItemSeparator.Name = "menuItemSeparator";
            this.menuItemSeparator.Size = new System.Drawing.Size(197, 6);
            //
            // menuItemAbout
            //
            this.menuItemAbout.Name = "menuItemAbout";
            this.menuItemAbout.Size = new System.Drawing.Size(200, 22);
            this.menuItemAbout.Text = "&About";
            this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
            //
            // toolStrip
            //
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAdd,
            this.buttonEdit,
            this.buttonDelete,
            this.toolStripSeparator1,
            this.buttonScan,
            this.buttonViewPermissions,
            this.buttonManagePermissions,
            this.buttonTestConnection,
            this.toolStripSeparator2,
            this.buttonRefresh});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1000, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            //
            // buttonAdd
            //
            this.buttonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(33, 22);
            this.buttonAdd.Text = "Add";
            this.buttonAdd.ToolTipText = "Add new cluster";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            //
            // buttonEdit
            //
            this.buttonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(31, 22);
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.ToolTipText = "Edit selected cluster";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            //
            // buttonDelete
            //
            this.buttonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(44, 22);
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.ToolTipText = "Delete selected cluster";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            // buttonScan
            //
            this.buttonScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(37, 22);
            this.buttonScan.Text = "Scan";
            this.buttonScan.ToolTipText = "Scan selected cluster for permissions";
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            //
            // buttonViewPermissions
            //
            this.buttonViewPermissions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonViewPermissions.Name = "buttonViewPermissions";
            this.buttonViewPermissions.Size = new System.Drawing.Size(102, 22);
            this.buttonViewPermissions.Text = "View Permissions";
            this.buttonViewPermissions.ToolTipText = "View cached permissions for selected cluster";
            this.buttonViewPermissions.Click += new System.EventHandler(this.buttonViewPermissions_Click);
            //
            // buttonManagePermissions
            //
            this.buttonManagePermissions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonManagePermissions.Name = "buttonManagePermissions";
            this.buttonManagePermissions.Size = new System.Drawing.Size(128, 22);
            this.buttonManagePermissions.Text = "Manage Permissions";
            this.buttonManagePermissions.ToolTipText = "GRANT/REVOKE permissions, manage users and groups";
            this.buttonManagePermissions.Click += new System.EventHandler(this.buttonManagePermissions_Click);
            //
            // buttonTestConnection
            //
            this.buttonTestConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonTestConnection.Name = "buttonTestConnection";
            this.buttonTestConnection.Size = new System.Drawing.Size(96, 22);
            this.buttonTestConnection.Text = "Test Connection";
            this.buttonTestConnection.ToolTipText = "Test connection to selected cluster";
            this.buttonTestConnection.Click += new System.EventHandler(this.buttonTestConnection_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            //
            // buttonRefresh
            //
            this.buttonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(50, 22);
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.ToolTipText = "Refresh cluster list";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // panelGrid
            //
            this.panelGrid.Controls.Add(this.dataGridViewClusters);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 49);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(5);
            this.panelGrid.Size = new System.Drawing.Size(1000, 520);
            this.panelGrid.TabIndex = 2;
            //
            // dataGridViewClusters
            //
            this.dataGridViewClusters.AllowUserToAddRows = false;
            this.dataGridViewClusters.AllowUserToDeleteRows = false;
            this.dataGridViewClusters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewClusters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClusters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClusters.Location = new System.Drawing.Point(5, 5);
            this.dataGridViewClusters.MultiSelect = false;
            this.dataGridViewClusters.Name = "dataGridViewClusters";
            this.dataGridViewClusters.ReadOnly = true;
            this.dataGridViewClusters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClusters.Size = new System.Drawing.Size(990, 510);
            this.dataGridViewClusters.TabIndex = 0;
            this.dataGridViewClusters.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewClusters_CellDoubleClick);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 569);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(109, 17);
            this.statusLabel.Text = "Total Clusters: 0";
            //
            // DashboardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 591);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "DashboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Redshift Guardian - Dashboard";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClusters)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemQueryEditor;
        private System.Windows.Forms.ToolStripMenuItem menuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem menuItemPermissionManagement;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpContents;
        private System.Windows.Forms.ToolStripMenuItem menuItemQueryEditorHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemSQLReference;
        private System.Windows.Forms.ToolStripSeparator menuItemSeparator;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton buttonAdd;
        private System.Windows.Forms.ToolStripButton buttonEdit;
        private System.Windows.Forms.ToolStripButton buttonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonScan;
        private System.Windows.Forms.ToolStripButton buttonViewPermissions;
        private System.Windows.Forms.ToolStripButton buttonManagePermissions;
        private System.Windows.Forms.ToolStripButton buttonTestConnection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton buttonRefresh;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dataGridViewClusters;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}

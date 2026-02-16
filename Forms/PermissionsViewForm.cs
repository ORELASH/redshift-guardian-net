using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Form for viewing cached permissions for a cluster
    /// </summary>
    public partial class PermissionsViewForm : Form
    {
        private Cluster _cluster;
        private ScannerService _scannerService;
        private List<RedshiftUser> _users;
        private List<TablePermission> _permissions;

        public PermissionsViewForm(Cluster cluster)
        {
            InitializeComponent();
            _cluster = cluster;
            _scannerService = new ScannerService();

            this.Text = "Permissions - " + cluster.Name;
            LoadData();
        }

        /// <summary>
        /// Loads cached permissions from database
        /// </summary>
        private void LoadData()
        {
            try
            {
                _users = _scannerService.GetCachedUsers(_cluster.Id);
                _permissions = _scannerService.GetCachedPermissions(_cluster.Id);

                RefreshUserGrid();
                RefreshPermissionGrid();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load permissions:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Refreshes the users grid
        /// </summary>
        private void RefreshUserGrid()
        {
            dataGridViewUsers.DataSource = null;
            dataGridViewUsers.DataSource = _users;

            if (dataGridViewUsers.Columns.Count > 0)
            {
                dataGridViewUsers.Columns["Username"].Width = 150;
                dataGridViewUsers.Columns["UserId"].Width = 80;
                dataGridViewUsers.Columns["IsSuperuser"].HeaderText = "Superuser";
                dataGridViewUsers.Columns["IsSuperuser"].Width = 80;
                dataGridViewUsers.Columns["CanCreateDB"].HeaderText = "Create DB";
                dataGridViewUsers.Columns["CanCreateDB"].Width = 80;
                dataGridViewUsers.Columns["CanCreateUser"].HeaderText = "Create User";
                dataGridViewUsers.Columns["CanCreateUser"].Width = 80;
                dataGridViewUsers.Columns["ValidUntil"].HeaderText = "Valid Until";
                dataGridViewUsers.Columns["ValidUntil"].Width = 140;
            }
        }

        /// <summary>
        /// Refreshes the permissions grid
        /// </summary>
        private void RefreshPermissionGrid()
        {
            dataGridViewPermissions.DataSource = null;
            dataGridViewPermissions.DataSource = _permissions;

            if (dataGridViewPermissions.Columns.Count > 0)
            {
                dataGridViewPermissions.Columns["Username"].Width = 150;
                dataGridViewPermissions.Columns["SchemaName"].HeaderText = "Schema";
                dataGridViewPermissions.Columns["SchemaName"].Width = 120;
                dataGridViewPermissions.Columns["TableName"].HeaderText = "Table";
                dataGridViewPermissions.Columns["TableName"].Width = 200;
                dataGridViewPermissions.Columns["PermissionType"].HeaderText = "Permission";
                dataGridViewPermissions.Columns["PermissionType"].Width = 100;
            }
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        private void UpdateStatusBar()
        {
            int userCount = _users != null ? _users.Count : 0;
            int permCount = _permissions != null ? _permissions.Count : 0;

            statusLabel.Text = string.Format("Users: {0}  |  Permissions: {1}", userCount, permCount);

            if (_cluster.LastScanTime.HasValue)
            {
                statusLabel.Text += string.Format("  |  Last Scan: {0:yyyy-MM-dd HH:mm:ss}",
                    _cluster.LastScanTime.Value);
            }
        }

        // ===== Event Handlers =====

        private void buttonExportUsers_Click(object sender, EventArgs e)
        {
            if (_users == null || _users.Count == 0)
            {
                MessageBox.Show(
                    "No users to export.",
                    "Export Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            saveDialog.FileName = string.Format("{0}_users_{1:yyyyMMdd}.csv",
                _cluster.Name, DateTime.Now);
            saveDialog.DefaultExt = "csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportUsersToCsv(saveDialog.FileName);
                    MessageBox.Show(
                        "Users exported successfully!",
                        "Export Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Export failed:\n\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void buttonExportPermissions_Click(object sender, EventArgs e)
        {
            if (_permissions == null || _permissions.Count == 0)
            {
                MessageBox.Show(
                    "No permissions to export.",
                    "Export Permissions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            saveDialog.FileName = string.Format("{0}_permissions_{1:yyyyMMdd}.csv",
                _cluster.Name, DateTime.Now);
            saveDialog.DefaultExt = "csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportPermissionsToCsv(saveDialog.FileName);
                    MessageBox.Show(
                        "Permissions exported successfully!",
                        "Export Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Export failed:\n\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Applies filter to the grids
        /// </summary>
        private void ApplyFilter()
        {
            string filter = textBoxFilter.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filter))
            {
                RefreshUserGrid();
                RefreshPermissionGrid();
                return;
            }

            // Filter users
            var filteredUsers = _users.FindAll(u =>
                (u.Username != null && u.Username.ToLower().Contains(filter))
            );
            dataGridViewUsers.DataSource = null;
            dataGridViewUsers.DataSource = filteredUsers;

            // Filter permissions
            var filteredPerms = _permissions.FindAll(p =>
                (p.Username != null && p.Username.ToLower().Contains(filter)) ||
                (p.SchemaName != null && p.SchemaName.ToLower().Contains(filter)) ||
                (p.TableName != null && p.TableName.ToLower().Contains(filter)) ||
                (p.PermissionType != null && p.PermissionType.ToLower().Contains(filter))
            );
            dataGridViewPermissions.DataSource = null;
            dataGridViewPermissions.DataSource = filteredPerms;
        }

        /// <summary>
        /// Exports users to CSV file
        /// </summary>
        private void ExportUsersToCsv(string fileName)
        {
            StringBuilder sb = new StringBuilder();

            // Header
            sb.AppendLine("Username,UserId,IsSuperuser,CanCreateDB,CanCreateUser,ValidUntil");

            // Data
            foreach (var user in _users)
            {
                sb.AppendLine(string.Format("\"{0}\",{1},{2},{3},{4},\"{5}\"",
                    EscapeCsv(user.Username),
                    user.UserId,
                    user.IsSuperuser,
                    user.CanCreateDB,
                    user.CanCreateUser,
                    user.ValidUntil.HasValue ? user.ValidUntil.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
                ));
            }

            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exports permissions to CSV file
        /// </summary>
        private void ExportPermissionsToCsv(string fileName)
        {
            StringBuilder sb = new StringBuilder();

            // Header
            sb.AppendLine("Username,SchemaName,TableName,PermissionType");

            // Data
            foreach (var perm in _permissions)
            {
                sb.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                    EscapeCsv(perm.Username),
                    EscapeCsv(perm.SchemaName),
                    EscapeCsv(perm.TableName),
                    EscapeCsv(perm.PermissionType)
                ));
            }

            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Escapes CSV values
        /// </summary>
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value.Replace("\"", "\"\"");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (_scannerService != null)
                {
                    _scannerService.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

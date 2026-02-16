using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Main dashboard form showing all Redshift clusters
    /// </summary>
    public partial class DashboardForm : Form
    {
        private ClusterService _clusterService;
        private ScannerService _scannerService;
        private RedshiftOdbcService _odbcService;
        private List<Cluster> _clusters;

        public DashboardForm()
        {
            InitializeComponent();
            InitializeServices();
            LoadClusters();
        }

        private void InitializeServices()
        {
            _clusterService = new ClusterService();
            _scannerService = new ScannerService();
            _odbcService = null; // Will be initialized when needed
        }

        /// <summary>
        /// Loads all clusters from the database
        /// </summary>
        private void LoadClusters()
        {
            try
            {
                _clusters = _clusterService.GetAllClusters();
                RefreshGrid();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load clusters:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Refreshes the data grid
        /// </summary>
        private void RefreshGrid()
        {
            dataGridViewClusters.DataSource = null;
            dataGridViewClusters.DataSource = _clusters;

            // Configure grid columns
            if (dataGridViewClusters.Columns.Count > 0)
            {
                dataGridViewClusters.Columns["Id"].Visible = false;
                dataGridViewClusters.Columns["Name"].HeaderText = "Cluster Name";
                dataGridViewClusters.Columns["Name"].Width = 150;
                dataGridViewClusters.Columns["Host"].HeaderText = "Host";
                dataGridViewClusters.Columns["Host"].Width = 250;
                dataGridViewClusters.Columns["Port"].Width = 60;
                dataGridViewClusters.Columns["Database"].Width = 100;
                dataGridViewClusters.Columns["Region"].Width = 100;
                dataGridViewClusters.Columns["ClusterType"].HeaderText = "Type";
                dataGridViewClusters.Columns["ClusterType"].Width = 100;
                dataGridViewClusters.Columns["UseIAM"].HeaderText = "Use IAM";
                dataGridViewClusters.Columns["UseIAM"].Width = 70;
                dataGridViewClusters.Columns["LastScanTime"].HeaderText = "Last Scan";
                dataGridViewClusters.Columns["LastScanTime"].Width = 140;
                dataGridViewClusters.Columns["LastScanStatus"].HeaderText = "Status";
                dataGridViewClusters.Columns["LastScanStatus"].Width = 80;

                // Hide internal fields
                if (dataGridViewClusters.Columns["AwsProfile"] != null)
                    dataGridViewClusters.Columns["AwsProfile"].Visible = false;
                if (dataGridViewClusters.Columns["CreatedAt"] != null)
                    dataGridViewClusters.Columns["CreatedAt"].Visible = false;
                if (dataGridViewClusters.Columns["UpdatedAt"] != null)
                    dataGridViewClusters.Columns["UpdatedAt"].Visible = false;
            }
        }

        /// <summary>
        /// Updates the status bar with cluster count
        /// </summary>
        private void UpdateStatusBar()
        {
            int count = _clusters != null ? _clusters.Count : 0;
            statusLabel.Text = string.Format("Total Clusters: {0}", count);
        }

        /// <summary>
        /// Gets the currently selected cluster
        /// </summary>
        private Cluster GetSelectedCluster()
        {
            if (dataGridViewClusters.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a cluster first.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return null;
            }

            return (Cluster)dataGridViewClusters.SelectedRows[0].DataBoundItem;
        }

        // ===== Event Handlers =====

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var editForm = new ClusterEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadClusters();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            var editForm = new ClusterEditForm(cluster);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadClusters();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            var result = MessageBox.Show(
                string.Format("Are you sure you want to delete cluster '{0}'?\n\nThis will also delete all cached scan data.", cluster.Name),
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                string error;
                bool success = _clusterService.DeleteCluster(cluster.Id, out error);

                if (success)
                {
                    MessageBox.Show(
                        "Cluster deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadClusters();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete cluster:\n\n" + error,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            // Disable buttons during scan
            SetButtonsEnabled(false);
            statusLabel.Text = "Scanning cluster...";
            Application.DoEvents();

            try
            {
                var result = _scannerService.ScanCluster(cluster);

                if (result.Success)
                {
                    MessageBox.Show(
                        string.Format(
                            "Scan completed successfully!\n\n" +
                            "Users found: {0}\n" +
                            "Permissions found: {1}\n" +
                            "Duration: {2:F1} seconds",
                            result.UserCount,
                            result.PermissionCount,
                            result.Duration.TotalSeconds
                        ),
                        "Scan Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadClusters();
                }
                else
                {
                    MessageBox.Show(
                        "Scan failed:\n\n" + result.Message,
                        "Scan Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Scan error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetButtonsEnabled(true);
                UpdateStatusBar();
            }
        }

        private void buttonViewPermissions_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            // Check if cluster has been scanned
            if (cluster.LastScanTime == null)
            {
                var result = MessageBox.Show(
                    "This cluster has not been scanned yet.\n\nWould you like to scan it now?",
                    "Not Scanned",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    buttonScan_Click(sender, e);
                    return;
                }
                else
                {
                    return;
                }
            }

            // Open permissions view form
            var permForm = new PermissionsViewForm(cluster);
            permForm.ShowDialog();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadClusters();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            SetButtonsEnabled(false);
            statusLabel.Text = "Testing connection...";
            Application.DoEvents();

            try
            {
                string error;
                bool connected = _clusterService.TestConnection(cluster, out error);

                if (connected)
                {
                    MessageBox.Show(
                        "Connection successful!",
                        "Test Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Connection failed:\n\n" + error,
                        "Test Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Connection test error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetButtonsEnabled(true);
                UpdateStatusBar();
            }
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItemHelpContents_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpViewerForm();
            helpForm.Show();
        }

        private void menuItemQueryEditorHelp_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpViewerForm();
            helpForm.Show();
            // TODO: Navigate directly to Query Editor topic
        }

        private void menuItemSQLReference_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpViewerForm();
            helpForm.Show();
            // TODO: Navigate directly to SQL Reference topic
        }

        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void menuItemQueryEditor_Click(object sender, EventArgs e)
        {
            // Check if there is a cluster connection
            if (_odbcService == null || !_odbcService.IsConnected())
            {
                // If not - try to connect to selected cluster
                if (dataGridViewClusters.SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        "Please select a cluster first and test connection.",
                        "No Cluster Selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Cluster selectedCluster = dataGridViewClusters.SelectedRows[0].DataBoundItem as Cluster;
                if (selectedCluster == null)
                {
                    return;
                }

                try
                {
                    _odbcService = new RedshiftOdbcService();
                    _odbcService.Connect(selectedCluster);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Failed to connect to cluster:\n\n" + ex.Message,
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            // Open Query Editor
            QueryEditorForm queryEditor = new QueryEditorForm(_odbcService);
            queryEditor.Show();
        }

        private void menuItemPermissionManagement_Click(object sender, EventArgs e)
        {
            var cluster = GetSelectedCluster();
            if (cluster == null) return;

            // Open Permission Management form
            var permMgmtForm = new PermissionManagementForm(cluster);
            permMgmtForm.ShowDialog(this);
        }

        private void buttonManagePermissions_Click(object sender, EventArgs e)
        {
            menuItemPermissionManagement_Click(sender, e);
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Redshift Guardian .NET 4.0\n\n" +
                "A tool for scanning and managing Amazon Redshift permissions.\n\n" +
                "Features:\n" +
                "- ODBC connection with IAM authentication\n" +
                "- Local caching of permissions\n" +
                "- Multiple cluster management\n" +
                "- GRANT/REVOKE permission operations\n" +
                "- User and Group management\n\n" +
                "Built with .NET Framework 4.0",
                "About Redshift Guardian",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void dataGridViewClusters_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                buttonEdit_Click(sender, e);
            }
        }

        /// <summary>
        /// Enables or disables all action buttons
        /// </summary>
        private void SetButtonsEnabled(bool enabled)
        {
            buttonAdd.Enabled = enabled;
            buttonEdit.Enabled = enabled;
            buttonDelete.Enabled = enabled;
            buttonScan.Enabled = enabled;
            buttonViewPermissions.Enabled = enabled;
            buttonManagePermissions.Enabled = enabled;
            buttonRefresh.Enabled = enabled;
            buttonTestConnection.Enabled = enabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (_clusterService != null)
                {
                    _clusterService.Dispose();
                }
                if (_scannerService != null)
                {
                    _scannerService.Dispose();
                }
                if (_odbcService != null)
                {
                    _odbcService.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

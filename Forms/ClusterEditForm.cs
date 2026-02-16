using System;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Form for adding or editing a Redshift cluster
    /// </summary>
    public partial class ClusterEditForm : Form
    {
        private Cluster _cluster;
        private ClusterService _clusterService;
        private bool _isEditMode;

        /// <summary>
        /// Constructor for adding new cluster
        /// </summary>
        public ClusterEditForm()
        {
            InitializeComponent();
            _isEditMode = false;
            _clusterService = new ClusterService();
            _cluster = new Cluster
            {
                Port = 5439,
                ClusterType = "Provisioned",
                UseIAM = true,
                Region = "us-east-1",
                AwsProfile = "default"
            };

            this.Text = "Add Cluster";
            LoadClusterData();
        }

        /// <summary>
        /// Constructor for editing existing cluster
        /// </summary>
        public ClusterEditForm(Cluster cluster)
        {
            InitializeComponent();
            _isEditMode = true;
            _clusterService = new ClusterService();
            _cluster = cluster;

            this.Text = "Edit Cluster - " + cluster.Name;
            LoadClusterData();
        }

        /// <summary>
        /// Loads cluster data into form controls
        /// </summary>
        private void LoadClusterData()
        {
            textBoxName.Text = _cluster.Name ?? "";
            textBoxHost.Text = _cluster.Host ?? "";
            numericPort.Value = _cluster.Port;
            textBoxDatabase.Text = _cluster.Database ?? "";
            comboClusterType.Text = _cluster.ClusterType ?? "Provisioned";
            textBoxRegion.Text = _cluster.Region ?? "us-east-1";
            textBoxAwsProfile.Text = _cluster.AwsProfile ?? "default";
            checkBoxUseIAM.Checked = _cluster.UseIAM;

            // Populate cluster type dropdown
            if (comboClusterType.Items.Count == 0)
            {
                comboClusterType.Items.Add("Provisioned");
                comboClusterType.Items.Add("Serverless");
            }
        }

        /// <summary>
        /// Saves form data back to cluster object
        /// </summary>
        private void SaveFormData()
        {
            _cluster.Name = textBoxName.Text.Trim();
            _cluster.Host = textBoxHost.Text.Trim();
            _cluster.Port = (int)numericPort.Value;
            _cluster.Database = textBoxDatabase.Text.Trim();
            _cluster.ClusterType = comboClusterType.Text;
            _cluster.Region = textBoxRegion.Text.Trim();
            _cluster.AwsProfile = textBoxAwsProfile.Text.Trim();
            _cluster.UseIAM = checkBoxUseIAM.Checked;
        }

        /// <summary>
        /// Validates form input
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Cluster name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxHost.Text))
            {
                MessageBox.Show("Host is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxHost.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxDatabase.Text))
            {
                MessageBox.Show("Database name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxDatabase.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxRegion.Text))
            {
                MessageBox.Show("AWS region is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxRegion.Focus();
                return false;
            }

            if (checkBoxUseIAM.Checked && string.IsNullOrWhiteSpace(textBoxAwsProfile.Text))
            {
                MessageBox.Show("AWS profile is required when using IAM authentication.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAwsProfile.Focus();
                return false;
            }

            return true;
        }

        // ===== Event Handlers =====

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            SaveFormData();

            string errorMessage;
            bool success = _clusterService.SaveCluster(_cluster, out errorMessage);

            if (success)
            {
                MessageBox.Show(
                    _isEditMode ? "Cluster updated successfully." : "Cluster created successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Failed to save cluster:\n\n" + errorMessage,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            SaveFormData();

            // Disable buttons during test
            buttonTestConnection.Enabled = false;
            buttonSave.Enabled = false;
            labelStatus.Text = "Testing connection...";
            Application.DoEvents();

            try
            {
                string errorMessage;
                bool connected = _clusterService.TestConnection(_cluster, out errorMessage);

                if (connected)
                {
                    labelStatus.Text = "Connection successful!";
                    labelStatus.ForeColor = System.Drawing.Color.Green;

                    MessageBox.Show(
                        "Connection test successful!",
                        "Test Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    labelStatus.Text = "Connection failed";
                    labelStatus.ForeColor = System.Drawing.Color.Red;

                    MessageBox.Show(
                        "Connection failed:\n\n" + errorMessage,
                        "Test Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Connection test error";
                labelStatus.ForeColor = System.Drawing.Color.Red;

                MessageBox.Show(
                    "Connection test error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                buttonTestConnection.Enabled = true;
                buttonSave.Enabled = true;
            }
        }

        private void checkBoxUseIAM_CheckedChanged(object sender, EventArgs e)
        {
            // Enable/disable AWS profile field based on IAM checkbox
            textBoxAwsProfile.Enabled = checkBoxUseIAM.Checked;
            labelAwsProfile.Enabled = checkBoxUseIAM.Checked;

            if (!checkBoxUseIAM.Checked)
            {
                labelStatus.Text = "Note: Username/password authentication not implemented. Use IAM.";
                labelStatus.ForeColor = System.Drawing.Color.Orange;
            }
            else
            {
                labelStatus.Text = "";
                labelStatus.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void buttonSetDefaults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This will reset the following fields to default values:\n\n" +
                "Port: 5439\n" +
                "Region: us-east-1\n" +
                "AWS Profile: default\n" +
                "Cluster Type: Provisioned\n" +
                "Use IAM: Yes\n\n" +
                "Continue?",
                "Set Defaults",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                numericPort.Value = 5439;
                textBoxRegion.Text = "us-east-1";
                textBoxAwsProfile.Text = "default";
                comboClusterType.Text = "Provisioned";
                checkBoxUseIAM.Checked = true;
            }
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
            }
            base.Dispose(disposing);
        }
    }
}

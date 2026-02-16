using System;
using System.Drawing;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Settings form for configuring application preferences
    /// </summary>
    public class SettingsForm : Form
    {
        private AppSettings _settings;
        private TabControl tabControl;
        private Button buttonSave;
        private Button buttonCancel;
        private Button buttonReset;
        private Button buttonBrowse;

        // Connection tab
        private NumericUpDown numericConnectionTimeout;
        private NumericUpDown numericQueryTimeout;
        private CheckBox checkBoxAutoConnect;

        // UI tab
        private NumericUpDown numericFontSize;
        private ComboBox comboTheme;
        private CheckBox checkBoxShowStatusBar;
        private CheckBox checkBoxShowToolBar;

        // Export tab
        private TextBox textBoxExportPath;
        private ComboBox comboExportFormat;
        private CheckBox checkBoxTimestamp;
        private CheckBox checkBoxOpenAfter;

        // Scan tab
        private NumericUpDown numericAutoRefresh;
        private CheckBox checkBoxEnableAutoRefresh;
        private CheckBox checkBoxScanOnStartup;
        private CheckBox checkBoxCacheResults;

        // Query Editor tab
        private NumericUpDown numericHistorySize;
        private CheckBox checkBoxSaveHistory;
        private CheckBox checkBoxAutoComplete;
        private NumericUpDown numericMaxRows;

        // Advanced tab
        private ComboBox comboLogLevel;
        private CheckBox checkBoxDetailedLog;
        private NumericUpDown numericMaxScans;
        private ComboBox comboLanguage;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        public SettingsForm(AppSettings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Tab Control
            tabControl = new TabControl();
            tabControl.Location = new Point(12, 12);
            tabControl.Size = new Size(560, 400);
            this.Controls.Add(tabControl);

            // Create tabs
            CreateConnectionTab();
            CreateUITab();
            CreateExportTab();
            CreateScanTab();
            CreateQueryEditorTab();
            CreateAdvancedTab();

            // Buttons
            buttonSave = new Button();
            buttonSave.Text = "Save";
            buttonSave.Location = new Point(320, 425);
            buttonSave.Size = new Size(80, 25);
            buttonSave.Click += buttonSave_Click;
            this.Controls.Add(buttonSave);

            buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.Location = new Point(410, 425);
            buttonCancel.Size = new Size(80, 25);
            buttonCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(buttonCancel);

            buttonReset = new Button();
            buttonReset.Text = "Reset Defaults";
            buttonReset.Location = new Point(12, 425);
            buttonReset.Size = new Size(100, 25);
            buttonReset.Click += buttonReset_Click;
            this.Controls.Add(buttonReset);

            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;
        }

        private void CreateConnectionTab()
        {
            TabPage tab = new TabPage("Connection");
            int y = 20;

            // Connection Timeout
            Label lblConnTimeout = new Label();
            lblConnTimeout.Text = "Connection Timeout (seconds):";
            lblConnTimeout.Location = new Point(20, y);
            lblConnTimeout.AutoSize = true;
            tab.Controls.Add(lblConnTimeout);

            numericConnectionTimeout = new NumericUpDown();
            numericConnectionTimeout.Location = new Point(250, y);
            numericConnectionTimeout.Size = new Size(80, 20);
            numericConnectionTimeout.Minimum = 5;
            numericConnectionTimeout.Maximum = 300;
            tab.Controls.Add(numericConnectionTimeout);

            y += 40;

            // Query Timeout
            Label lblQueryTimeout = new Label();
            lblQueryTimeout.Text = "Query Timeout (seconds):";
            lblQueryTimeout.Location = new Point(20, y);
            lblQueryTimeout.AutoSize = true;
            tab.Controls.Add(lblQueryTimeout);

            numericQueryTimeout = new NumericUpDown();
            numericQueryTimeout.Location = new Point(250, y);
            numericQueryTimeout.Size = new Size(80, 20);
            numericQueryTimeout.Minimum = 10;
            numericQueryTimeout.Maximum = 600;
            tab.Controls.Add(numericQueryTimeout);

            y += 40;

            // Auto Connect
            checkBoxAutoConnect = new CheckBox();
            checkBoxAutoConnect.Text = "Auto-connect to last used cluster on startup";
            checkBoxAutoConnect.Location = new Point(20, y);
            checkBoxAutoConnect.AutoSize = true;
            tab.Controls.Add(checkBoxAutoConnect);

            tabControl.TabPages.Add(tab);
        }

        private void CreateUITab()
        {
            TabPage tab = new TabPage("User Interface");
            int y = 20;

            // Font Size
            Label lblFontSize = new Label();
            lblFontSize.Text = "Font Size (points):";
            lblFontSize.Location = new Point(20, y);
            lblFontSize.AutoSize = true;
            tab.Controls.Add(lblFontSize);

            numericFontSize = new NumericUpDown();
            numericFontSize.Location = new Point(250, y);
            numericFontSize.Size = new Size(80, 20);
            numericFontSize.Minimum = 6;
            numericFontSize.Maximum = 20;
            tab.Controls.Add(numericFontSize);

            y += 40;

            // Theme
            Label lblTheme = new Label();
            lblTheme.Text = "Theme:";
            lblTheme.Location = new Point(20, y);
            lblTheme.AutoSize = true;
            tab.Controls.Add(lblTheme);

            comboTheme = new ComboBox();
            comboTheme.Location = new Point(250, y);
            comboTheme.Size = new Size(150, 21);
            comboTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTheme.Items.AddRange(new object[] { "Default", "Light", "Dark" });
            tab.Controls.Add(comboTheme);

            y += 40;

            // Show Status Bar
            checkBoxShowStatusBar = new CheckBox();
            checkBoxShowStatusBar.Text = "Show status bar";
            checkBoxShowStatusBar.Location = new Point(20, y);
            checkBoxShowStatusBar.AutoSize = true;
            tab.Controls.Add(checkBoxShowStatusBar);

            y += 30;

            // Show Tool Bar
            checkBoxShowToolBar = new CheckBox();
            checkBoxShowToolBar.Text = "Show toolbar";
            checkBoxShowToolBar.Location = new Point(20, y);
            checkBoxShowToolBar.AutoSize = true;
            tab.Controls.Add(checkBoxShowToolBar);

            tabControl.TabPages.Add(tab);
        }

        private void CreateExportTab()
        {
            TabPage tab = new TabPage("Export");
            int y = 20;

            // Export Path
            Label lblExportPath = new Label();
            lblExportPath.Text = "Default Export Path:";
            lblExportPath.Location = new Point(20, y);
            lblExportPath.AutoSize = true;
            tab.Controls.Add(lblExportPath);

            textBoxExportPath = new TextBox();
            textBoxExportPath.Location = new Point(20, y + 20);
            textBoxExportPath.Size = new Size(400, 20);
            tab.Controls.Add(textBoxExportPath);

            buttonBrowse = new Button();
            buttonBrowse.Text = "Browse...";
            buttonBrowse.Location = new Point(430, y + 18);
            buttonBrowse.Size = new Size(80, 25);
            buttonBrowse.Click += buttonBrowse_Click;
            tab.Controls.Add(buttonBrowse);

            y += 60;

            // Export Format
            Label lblFormat = new Label();
            lblFormat.Text = "Default Export Format:";
            lblFormat.Location = new Point(20, y);
            lblFormat.AutoSize = true;
            tab.Controls.Add(lblFormat);

            comboExportFormat = new ComboBox();
            comboExportFormat.Location = new Point(250, y);
            comboExportFormat.Size = new Size(150, 21);
            comboExportFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            comboExportFormat.Items.AddRange(new object[] { "CSV", "Excel", "JSON", "XML" });
            tab.Controls.Add(comboExportFormat);

            y += 40;

            // Include Timestamp
            checkBoxTimestamp = new CheckBox();
            checkBoxTimestamp.Text = "Include timestamp in filename";
            checkBoxTimestamp.Location = new Point(20, y);
            checkBoxTimestamp.AutoSize = true;
            tab.Controls.Add(checkBoxTimestamp);

            y += 30;

            // Open After Export
            checkBoxOpenAfter = new CheckBox();
            checkBoxOpenAfter.Text = "Open file after export";
            checkBoxOpenAfter.Location = new Point(20, y);
            checkBoxOpenAfter.AutoSize = true;
            tab.Controls.Add(checkBoxOpenAfter);

            tabControl.TabPages.Add(tab);
        }

        private void CreateScanTab()
        {
            TabPage tab = new TabPage("Scanning");
            int y = 20;

            // Auto Refresh
            checkBoxEnableAutoRefresh = new CheckBox();
            checkBoxEnableAutoRefresh.Text = "Enable auto-refresh";
            checkBoxEnableAutoRefresh.Location = new Point(20, y);
            checkBoxEnableAutoRefresh.AutoSize = true;
            checkBoxEnableAutoRefresh.CheckedChanged += checkBoxEnableAutoRefresh_CheckedChanged;
            tab.Controls.Add(checkBoxEnableAutoRefresh);

            y += 35;

            Label lblAutoRefresh = new Label();
            lblAutoRefresh.Text = "Auto-refresh interval (minutes):";
            lblAutoRefresh.Location = new Point(40, y);
            lblAutoRefresh.AutoSize = true;
            tab.Controls.Add(lblAutoRefresh);

            numericAutoRefresh = new NumericUpDown();
            numericAutoRefresh.Location = new Point(250, y);
            numericAutoRefresh.Size = new Size(80, 20);
            numericAutoRefresh.Minimum = 1;
            numericAutoRefresh.Maximum = 1440;
            numericAutoRefresh.Enabled = false;
            tab.Controls.Add(numericAutoRefresh);

            y += 40;

            // Scan on Startup
            checkBoxScanOnStartup = new CheckBox();
            checkBoxScanOnStartup.Text = "Scan clusters on application startup";
            checkBoxScanOnStartup.Location = new Point(20, y);
            checkBoxScanOnStartup.AutoSize = true;
            tab.Controls.Add(checkBoxScanOnStartup);

            y += 30;

            // Cache Results
            checkBoxCacheResults = new CheckBox();
            checkBoxCacheResults.Text = "Cache scan results locally";
            checkBoxCacheResults.Location = new Point(20, y);
            checkBoxCacheResults.AutoSize = true;
            tab.Controls.Add(checkBoxCacheResults);

            tabControl.TabPages.Add(tab);
        }

        private void CreateQueryEditorTab()
        {
            TabPage tab = new TabPage("Query Editor");
            int y = 20;

            // Query History Size
            Label lblHistorySize = new Label();
            lblHistorySize.Text = "Query History Size:";
            lblHistorySize.Location = new Point(20, y);
            lblHistorySize.AutoSize = true;
            tab.Controls.Add(lblHistorySize);

            numericHistorySize = new NumericUpDown();
            numericHistorySize.Location = new Point(250, y);
            numericHistorySize.Size = new Size(80, 20);
            numericHistorySize.Minimum = 0;
            numericHistorySize.Maximum = 1000;
            tab.Controls.Add(numericHistorySize);

            y += 40;

            // Save History
            checkBoxSaveHistory = new CheckBox();
            checkBoxSaveHistory.Text = "Save query history between sessions";
            checkBoxSaveHistory.Location = new Point(20, y);
            checkBoxSaveHistory.AutoSize = true;
            tab.Controls.Add(checkBoxSaveHistory);

            y += 30;

            // Auto Complete
            checkBoxAutoComplete = new CheckBox();
            checkBoxAutoComplete.Text = "Enable auto-complete (basic)";
            checkBoxAutoComplete.Location = new Point(20, y);
            checkBoxAutoComplete.AutoSize = true;
            tab.Controls.Add(checkBoxAutoComplete);

            y += 40;

            // Max Rows
            Label lblMaxRows = new Label();
            lblMaxRows.Text = "Maximum result rows to display:";
            lblMaxRows.Location = new Point(20, y);
            lblMaxRows.AutoSize = true;
            tab.Controls.Add(lblMaxRows);

            numericMaxRows = new NumericUpDown();
            numericMaxRows.Location = new Point(250, y);
            numericMaxRows.Size = new Size(80, 20);
            numericMaxRows.Minimum = 100;
            numericMaxRows.Maximum = 100000;
            numericMaxRows.Increment = 1000;
            tab.Controls.Add(numericMaxRows);

            tabControl.TabPages.Add(tab);
        }

        private void CreateAdvancedTab()
        {
            TabPage tab = new TabPage("Advanced");
            int y = 20;

            // Log Level
            Label lblLogLevel = new Label();
            lblLogLevel.Text = "Log Level:";
            lblLogLevel.Location = new Point(20, y);
            lblLogLevel.AutoSize = true;
            tab.Controls.Add(lblLogLevel);

            comboLogLevel = new ComboBox();
            comboLogLevel.Location = new Point(250, y);
            comboLogLevel.Size = new Size(150, 21);
            comboLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLogLevel.Items.AddRange(new object[] { "Error", "Warning", "Info", "Debug" });
            tab.Controls.Add(comboLogLevel);

            y += 40;

            // Detailed Logging
            checkBoxDetailedLog = new CheckBox();
            checkBoxDetailedLog.Text = "Enable detailed logging";
            checkBoxDetailedLog.Location = new Point(20, y);
            checkBoxDetailedLog.AutoSize = true;
            tab.Controls.Add(checkBoxDetailedLog);

            y += 40;

            // Max Concurrent Scans
            Label lblMaxScans = new Label();
            lblMaxScans.Text = "Max concurrent scans:";
            lblMaxScans.Location = new Point(20, y);
            lblMaxScans.AutoSize = true;
            tab.Controls.Add(lblMaxScans);

            numericMaxScans = new NumericUpDown();
            numericMaxScans.Location = new Point(250, y);
            numericMaxScans.Size = new Size(80, 20);
            numericMaxScans.Minimum = 1;
            numericMaxScans.Maximum = 10;
            tab.Controls.Add(numericMaxScans);

            y += 40;

            // Language
            Label lblLanguage = new Label();
            lblLanguage.Text = "Language:";
            lblLanguage.Location = new Point(20, y);
            lblLanguage.AutoSize = true;
            tab.Controls.Add(lblLanguage);

            comboLanguage = new ComboBox();
            comboLanguage.Location = new Point(250, y);
            comboLanguage.Size = new Size(150, 21);
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.Items.AddRange(new object[] { "English", "Hebrew" });
            tab.Controls.Add(comboLanguage);

            tabControl.TabPages.Add(tab);
        }

        private void LoadSettings()
        {
            if (_settings == null)
            {
                _settings = AppSettings.Load();
            }

            // Connection
            numericConnectionTimeout.Value = _settings.ConnectionTimeoutSeconds;
            numericQueryTimeout.Value = _settings.QueryTimeoutSeconds;
            checkBoxAutoConnect.Checked = _settings.AutoConnectLastCluster;

            // UI
            numericFontSize.Value = _settings.FontSizePoints;
            comboTheme.SelectedItem = _settings.Theme;
            checkBoxShowStatusBar.Checked = _settings.ShowStatusBar;
            checkBoxShowToolBar.Checked = _settings.ShowToolBar;

            // Export
            textBoxExportPath.Text = _settings.DefaultExportPath;
            comboExportFormat.SelectedItem = _settings.DefaultExportFormat;
            checkBoxTimestamp.Checked = _settings.IncludeTimestampInFilename;
            checkBoxOpenAfter.Checked = _settings.OpenFileAfterExport;

            // Scan
            checkBoxEnableAutoRefresh.Checked = _settings.EnableAutoRefresh;
            numericAutoRefresh.Value = _settings.AutoRefreshMinutes > 0 ? _settings.AutoRefreshMinutes : 5;
            numericAutoRefresh.Enabled = _settings.EnableAutoRefresh;
            checkBoxScanOnStartup.Checked = _settings.ScanOnStartup;
            checkBoxCacheResults.Checked = _settings.CacheResults;

            // Query Editor
            numericHistorySize.Value = _settings.QueryHistorySize;
            checkBoxSaveHistory.Checked = _settings.SaveQueryHistory;
            checkBoxAutoComplete.Checked = _settings.AutoCompleteEnabled;
            numericMaxRows.Value = _settings.QueryResultsMaxRows;

            // Advanced
            comboLogLevel.SelectedItem = _settings.LogLevel;
            checkBoxDetailedLog.Checked = _settings.EnableDetailedLogging;
            numericMaxScans.Value = _settings.MaxConcurrentScans;
            comboLanguage.SelectedItem = _settings.Language;
        }

        private void SaveSettings()
        {
            // Connection
            _settings.ConnectionTimeoutSeconds = (int)numericConnectionTimeout.Value;
            _settings.QueryTimeoutSeconds = (int)numericQueryTimeout.Value;
            _settings.AutoConnectLastCluster = checkBoxAutoConnect.Checked;

            // UI
            _settings.FontSizePoints = (int)numericFontSize.Value;
            _settings.Theme = comboTheme.SelectedItem.ToString();
            _settings.ShowStatusBar = checkBoxShowStatusBar.Checked;
            _settings.ShowToolBar = checkBoxShowToolBar.Checked;

            // Export
            _settings.DefaultExportPath = textBoxExportPath.Text;
            _settings.DefaultExportFormat = comboExportFormat.SelectedItem.ToString();
            _settings.IncludeTimestampInFilename = checkBoxTimestamp.Checked;
            _settings.OpenFileAfterExport = checkBoxOpenAfter.Checked;

            // Scan
            _settings.EnableAutoRefresh = checkBoxEnableAutoRefresh.Checked;
            _settings.AutoRefreshMinutes = checkBoxEnableAutoRefresh.Checked ? (int)numericAutoRefresh.Value : 0;
            _settings.ScanOnStartup = checkBoxScanOnStartup.Checked;
            _settings.CacheResults = checkBoxCacheResults.Checked;

            // Query Editor
            _settings.QueryHistorySize = (int)numericHistorySize.Value;
            _settings.SaveQueryHistory = checkBoxSaveHistory.Checked;
            _settings.AutoCompleteEnabled = checkBoxAutoComplete.Checked;
            _settings.QueryResultsMaxRows = (int)numericMaxRows.Value;

            // Advanced
            _settings.LogLevel = comboLogLevel.SelectedItem.ToString();
            _settings.EnableDetailedLogging = checkBoxDetailedLog.Checked;
            _settings.MaxConcurrentScans = (int)numericMaxScans.Value;
            _settings.Language = comboLanguage.SelectedItem.ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();

                string errorMessage;
                if (!_settings.Validate(out errorMessage))
                {
                    MessageBox.Show(
                        errorMessage,
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                _settings.Save();

                MessageBox.Show(
                    "Settings saved successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save settings:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset all settings to defaults?",
                "Reset Settings",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _settings.Reset();
                LoadSettings();
                MessageBox.Show(
                    "Settings reset to defaults.",
                    "Reset Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = textBoxExportPath.Text;
            dialog.Description = "Select default export folder";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxExportPath.Text = dialog.SelectedPath;
            }
        }

        private void checkBoxEnableAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            numericAutoRefresh.Enabled = checkBoxEnableAutoRefresh.Checked;
        }
    }
}

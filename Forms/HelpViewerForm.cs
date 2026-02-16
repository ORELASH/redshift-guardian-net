using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Help Viewer form with navigation and content display
    /// </summary>
    public class HelpViewerForm : Form
    {
        private SplitContainer splitContainer;
        private TreeView treeViewTopics;
        private RichTextBox richTextBoxContent;
        private Panel panelTop;
        private TextBox textBoxSearch;
        private Button buttonSearch;
        private Button buttonPrint;
        private ToolStrip toolStrip;
        private ToolStripButton buttonBack;
        private ToolStripButton buttonForward;
        private ToolStripButton buttonHome;
        private ToolStripSeparator separator1;
        private ToolStripLabel labelTitle;

        private HelpContentProvider _contentProvider;
        private List<string> _navigationHistory;
        private int _currentHistoryIndex;

        public HelpViewerForm()
        {
            InitializeComponent();
            _contentProvider = new HelpContentProvider();
            _navigationHistory = new List<string>();
            _currentHistoryIndex = -1;
            LoadHelpTopics();
            NavigateToTopic("welcome");
        }

        private void InitializeComponent()
        {
            this.Text = "Redshift Guardian - Help";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ToolStrip
            toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;

            buttonBack = new ToolStripButton();
            buttonBack.Text = "< Back";
            buttonBack.Click += buttonBack_Click;
            buttonBack.Enabled = false;

            buttonForward = new ToolStripButton();
            buttonForward.Text = "Forward >";
            buttonForward.Click += buttonForward_Click;
            buttonForward.Enabled = false;

            buttonHome = new ToolStripButton();
            buttonHome.Text = "Home";
            buttonHome.Click += buttonHome_Click;

            separator1 = new ToolStripSeparator();

            labelTitle = new ToolStripLabel();
            labelTitle.Text = "Welcome";
            labelTitle.Font = new Font(labelTitle.Font, FontStyle.Bold);

            toolStrip.Items.Add(buttonBack);
            toolStrip.Items.Add(buttonForward);
            toolStrip.Items.Add(buttonHome);
            toolStrip.Items.Add(separator1);
            toolStrip.Items.Add(labelTitle);

            this.Controls.Add(toolStrip);

            // Top Panel for Search
            panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 40;
            panelTop.Padding = new Padding(5);

            Label labelSearch = new Label();
            labelSearch.Text = "Search:";
            labelSearch.Location = new Point(10, 12);
            labelSearch.AutoSize = true;
            panelTop.Controls.Add(labelSearch);

            textBoxSearch = new TextBox();
            textBoxSearch.Location = new Point(70, 10);
            textBoxSearch.Width = 300;
            textBoxSearch.KeyDown += textBoxSearch_KeyDown;
            panelTop.Controls.Add(textBoxSearch);

            buttonSearch = new Button();
            buttonSearch.Text = "Search";
            buttonSearch.Location = new Point(380, 9);
            buttonSearch.Click += buttonSearch_Click;
            panelTop.Controls.Add(buttonSearch);

            buttonPrint = new Button();
            buttonPrint.Text = "Print";
            buttonPrint.Location = new Point(470, 9);
            buttonPrint.Click += buttonPrint_Click;
            panelTop.Controls.Add(buttonPrint);

            this.Controls.Add(panelTop);

            // SplitContainer
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = 250;
            splitContainer.BorderStyle = BorderStyle.Fixed3D;

            // TreeView for Topics
            treeViewTopics = new TreeView();
            treeViewTopics.Dock = DockStyle.Fill;
            treeViewTopics.AfterSelect += treeViewTopics_AfterSelect;
            splitContainer.Panel1.Controls.Add(treeViewTopics);

            // RichTextBox for Content
            richTextBoxContent = new RichTextBox();
            richTextBoxContent.Dock = DockStyle.Fill;
            richTextBoxContent.ReadOnly = true;
            richTextBoxContent.Font = new Font("Segoe UI", 10);
            richTextBoxContent.BackColor = Color.White;
            richTextBoxContent.LinkClicked += richTextBoxContent_LinkClicked;
            splitContainer.Panel2.Controls.Add(richTextBoxContent);

            this.Controls.Add(splitContainer);
        }

        private void LoadHelpTopics()
        {
            treeViewTopics.Nodes.Clear();

            // Welcome
            TreeNode welcomeNode = new TreeNode("Welcome");
            welcomeNode.Tag = "welcome";
            treeViewTopics.Nodes.Add(welcomeNode);

            // Getting Started
            TreeNode gettingStartedNode = new TreeNode("Getting Started");
            gettingStartedNode.Tag = "getting_started";
            gettingStartedNode.Nodes.Add(CreateNode("Installation", "installation"));
            gettingStartedNode.Nodes.Add(CreateNode("First Steps", "first_steps"));
            gettingStartedNode.Nodes.Add(CreateNode("Adding Clusters", "adding_clusters"));
            treeViewTopics.Nodes.Add(gettingStartedNode);

            // Cluster Management
            TreeNode clusterNode = new TreeNode("Cluster Management");
            clusterNode.Tag = "cluster_management";
            clusterNode.Nodes.Add(CreateNode("Adding Clusters", "cluster_add"));
            clusterNode.Nodes.Add(CreateNode("Editing Clusters", "cluster_edit"));
            clusterNode.Nodes.Add(CreateNode("Testing Connections", "cluster_test"));
            clusterNode.Nodes.Add(CreateNode("ODBC Configuration", "odbc_config"));
            treeViewTopics.Nodes.Add(clusterNode);

            // Permissions Scanning
            TreeNode scanNode = new TreeNode("Permissions Scanning");
            scanNode.Tag = "permissions_scanning";
            scanNode.Nodes.Add(CreateNode("Running a Scan", "scan_run"));
            scanNode.Nodes.Add(CreateNode("Viewing Results", "scan_results"));
            scanNode.Nodes.Add(CreateNode("Understanding Permissions", "scan_understanding"));
            treeViewTopics.Nodes.Add(scanNode);

            // Query Editor
            TreeNode queryNode = new TreeNode("SQL Query Editor");
            queryNode.Tag = "query_editor";
            queryNode.Nodes.Add(CreateNode("Opening Query Editor", "query_open"));
            queryNode.Nodes.Add(CreateNode("Query Library", "query_library"));
            queryNode.Nodes.Add(CreateNode("Writing Custom Queries", "query_custom"));
            queryNode.Nodes.Add(CreateNode("Using Parameters", "query_parameters"));
            queryNode.Nodes.Add(CreateNode("Exporting Results", "query_export"));
            queryNode.Nodes.Add(CreateNode("Built-in Queries Reference", "query_builtin"));
            treeViewTopics.Nodes.Add(queryNode);

            // SQL Reference
            TreeNode sqlNode = new TreeNode("SQL Queries Reference");
            sqlNode.Tag = "sql_reference";
            sqlNode.Nodes.Add(CreateNode("User Queries", "sql_users"));
            sqlNode.Nodes.Add(CreateNode("Role Queries", "sql_roles"));
            sqlNode.Nodes.Add(CreateNode("Permission Queries", "sql_permissions"));
            sqlNode.Nodes.Add(CreateNode("Table Queries", "sql_tables"));
            sqlNode.Nodes.Add(CreateNode("Schema Queries", "sql_schemas"));
            sqlNode.Nodes.Add(CreateNode("System Catalog", "sql_catalog"));
            treeViewTopics.Nodes.Add(sqlNode);

            // Troubleshooting
            TreeNode troubleshootNode = new TreeNode("Troubleshooting");
            troubleshootNode.Tag = "troubleshooting";
            troubleshootNode.Nodes.Add(CreateNode("Connection Issues", "trouble_connection"));
            troubleshootNode.Nodes.Add(CreateNode("Build Errors", "trouble_build"));
            troubleshootNode.Nodes.Add(CreateNode("Query Errors", "trouble_query"));
            troubleshootNode.Nodes.Add(CreateNode("Database Issues", "trouble_database"));
            treeViewTopics.Nodes.Add(troubleshootNode);

            // About
            TreeNode aboutNode = new TreeNode("About");
            aboutNode.Tag = "about";
            aboutNode.Nodes.Add(CreateNode("Version Info", "about_version"));
            aboutNode.Nodes.Add(CreateNode("System Capabilities", "about_capabilities"));
            aboutNode.Nodes.Add(CreateNode("Technical Details", "about_technical"));
            treeViewTopics.Nodes.Add(aboutNode);

            treeViewTopics.ExpandAll();
        }

        private TreeNode CreateNode(string text, string tag)
        {
            TreeNode node = new TreeNode(text);
            node.Tag = tag;
            return node;
        }

        private void treeViewTopics_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                string topicId = e.Node.Tag.ToString();
                NavigateToTopic(topicId);
            }
        }

        private void NavigateToTopic(string topicId)
        {
            string content = _contentProvider.GetTopicContent(topicId);
            string title = _contentProvider.GetTopicTitle(topicId);

            richTextBoxContent.Clear();
            richTextBoxContent.Rtf = content;
            labelTitle.Text = title;

            // Add to history
            if (_currentHistoryIndex == -1 ||
                _navigationHistory[_currentHistoryIndex] != topicId)
            {
                // Remove forward history
                if (_currentHistoryIndex < _navigationHistory.Count - 1)
                {
                    _navigationHistory.RemoveRange(
                        _currentHistoryIndex + 1,
                        _navigationHistory.Count - _currentHistoryIndex - 1);
                }

                _navigationHistory.Add(topicId);
                _currentHistoryIndex = _navigationHistory.Count - 1;
            }

            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            buttonBack.Enabled = _currentHistoryIndex > 0;
            buttonForward.Enabled = _currentHistoryIndex < _navigationHistory.Count - 1;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (_currentHistoryIndex > 0)
            {
                _currentHistoryIndex--;
                string topicId = _navigationHistory[_currentHistoryIndex];
                LoadTopicWithoutHistory(topicId);
                UpdateNavigationButtons();
            }
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            if (_currentHistoryIndex < _navigationHistory.Count - 1)
            {
                _currentHistoryIndex++;
                string topicId = _navigationHistory[_currentHistoryIndex];
                LoadTopicWithoutHistory(topicId);
                UpdateNavigationButtons();
            }
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            NavigateToTopic("welcome");
        }

        private void LoadTopicWithoutHistory(string topicId)
        {
            string content = _contentProvider.GetTopicContent(topicId);
            string title = _contentProvider.GetTopicTitle(topicId);

            richTextBoxContent.Clear();
            richTextBoxContent.Rtf = content;
            labelTitle.Text = title;

            // Select node in tree
            SelectTreeNode(topicId);
        }

        private void SelectTreeNode(string topicId)
        {
            foreach (TreeNode node in treeViewTopics.Nodes)
            {
                if (SelectNodeRecursive(node, topicId))
                {
                    break;
                }
            }
        }

        private bool SelectNodeRecursive(TreeNode node, string topicId)
        {
            if (node.Tag != null && node.Tag.ToString() == topicId)
            {
                treeViewTopics.SelectedNode = node;
                return true;
            }

            foreach (TreeNode child in node.Nodes)
            {
                if (SelectNodeRecursive(child, topicId))
                {
                    return true;
                }
            }

            return false;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void PerformSearch()
        {
            string searchText = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                return;
            }

            List<HelpSearchResult> results = _contentProvider.SearchContent(searchText);

            if (results.Count == 0)
            {
                MessageBox.Show(
                    "No results found for: " + searchText,
                    "Search Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Show search results form
            ShowSearchResults(results, searchText);
        }

        private void ShowSearchResults(List<HelpSearchResult> results, string searchText)
        {
            Form resultsForm = new Form();
            resultsForm.Text = "Search Results - " + searchText;
            resultsForm.Size = new Size(600, 400);
            resultsForm.StartPosition = FormStartPosition.CenterParent;

            ListBox listResults = new ListBox();
            listResults.Dock = DockStyle.Fill;
            listResults.DisplayMember = "DisplayText";
            listResults.Font = new Font("Segoe UI", 9);

            foreach (HelpSearchResult result in results)
            {
                listResults.Items.Add(result);
            }

            listResults.DoubleClick += delegate
            {
                if (listResults.SelectedItem != null)
                {
                    HelpSearchResult selected = (HelpSearchResult)listResults.SelectedItem;
                    NavigateToTopic(selected.TopicId);
                    resultsForm.Close();
                }
            };

            resultsForm.Controls.Add(listResults);
            resultsForm.ShowDialog(this);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBoxContent.Print(printDialog.PrinterSettings);
                    MessageBox.Show(
                        "Content sent to printer successfully.",
                        "Print",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to print:\n\n" + ex.Message,
                    "Print Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void richTextBoxContent_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string link = e.LinkText;

            // Internal link format: help://topicId
            if (link.StartsWith("help://"))
            {
                string topicId = link.Substring(7);
                NavigateToTopic(topicId);
            }
            else
            {
                // External link
                try
                {
                    System.Diagnostics.Process.Start(link);
                }
                catch
                {
                    MessageBox.Show(
                        "Failed to open link: " + link,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.Forms
{
    /// <summary>
    /// Advanced SQL Query Editor with library management
    /// </summary>
    public partial class QueryEditorForm : Form
    {
        private readonly QueryLibraryService _libraryService;
        private readonly QueryExecutorService _executorService;
        private QueryTemplate _currentQuery;

        // UI Controls
        private SplitContainer mainSplit;
        private SplitContainer leftSplit;

        // Left panel - Query Library
        private ComboBox comboCategory;
        private ListBox listQueries;
        private Button buttonLoadQuery;
        private Button buttonNewQuery;
        private Button buttonSaveQuery;
        private Button buttonDeleteQuery;
        private TextBox textQueryDescription;

        // Center panel - Query Editor
        private TextBox textSqlQuery;
        private Panel panelParameters;
        private FlowLayoutPanel flowParameters;
        private Button buttonExecute;
        private Button buttonValidate;
        private Button buttonExplain;
        private Button buttonClear;
        private Label labelRowCount;
        private Label labelExecutionTime;

        // Right panel - Results
        private DataGridView gridResults;
        private Button buttonExportCsv;
        private TextBox textMessages;

        public QueryEditorForm(RedshiftOdbcService odbcService)
        {
            _libraryService = new QueryLibraryService();
            _executorService = new QueryExecutorService(odbcService);

            InitializeComponent();
            LoadCategories();
            LoadQueries();
        }

        private void InitializeComponent()
        {
            this.Text = "SQL Query Editor & Library";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main split container (library | editor+results)
            mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.SplitterDistance = 300;
            mainSplit.Orientation = Orientation.Vertical;
            this.Controls.Add(mainSplit);

            // Left split (library | description)
            leftSplit = new SplitContainer();
            leftSplit.Dock = DockStyle.Fill;
            leftSplit.Orientation = Orientation.Horizontal;
            leftSplit.SplitterDistance = 500;
            mainSplit.Panel1.Controls.Add(leftSplit);

            // === LEFT PANEL - Query Library ===
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Fill;
            leftSplit.Panel1.Controls.Add(leftPanel);

            Label labelLibrary = new Label();
            labelLibrary.Text = "Query Library";
            labelLibrary.Font = new Font(labelLibrary.Font, FontStyle.Bold);
            labelLibrary.Location = new Point(10, 10);
            labelLibrary.AutoSize = true;
            leftPanel.Controls.Add(labelLibrary);

            Label labelCategory = new Label();
            labelCategory.Text = "Category:";
            labelCategory.Location = new Point(10, 40);
            labelCategory.AutoSize = true;
            leftPanel.Controls.Add(labelCategory);

            comboCategory = new ComboBox();
            comboCategory.Location = new Point(10, 60);
            comboCategory.Width = 270;
            comboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            comboCategory.SelectedIndexChanged += ComboCategory_SelectedIndexChanged;
            leftPanel.Controls.Add(comboCategory);

            listQueries = new ListBox();
            listQueries.Location = new Point(10, 90);
            listQueries.Size = new Size(270, 350);
            listQueries.SelectedIndexChanged += ListQueries_SelectedIndexChanged;
            leftPanel.Controls.Add(listQueries);

            buttonLoadQuery = new Button();
            buttonLoadQuery.Text = "Load Query";
            buttonLoadQuery.Location = new Point(10, 450);
            buttonLoadQuery.Width = 130;
            buttonLoadQuery.Click += ButtonLoadQuery_Click;
            leftPanel.Controls.Add(buttonLoadQuery);

            buttonNewQuery = new Button();
            buttonNewQuery.Text = "New Query";
            buttonNewQuery.Location = new Point(150, 450);
            buttonNewQuery.Width = 130;
            buttonNewQuery.Click += ButtonNewQuery_Click;
            leftPanel.Controls.Add(buttonNewQuery);

            // === DESCRIPTION PANEL ===
            Panel descPanel = new Panel();
            descPanel.Dock = DockStyle.Fill;
            leftSplit.Panel2.Controls.Add(descPanel);

            Label labelDesc = new Label();
            labelDesc.Text = "Description:";
            labelDesc.Location = new Point(10, 10);
            labelDesc.AutoSize = true;
            descPanel.Controls.Add(labelDesc);

            textQueryDescription = new TextBox();
            textQueryDescription.Location = new Point(10, 30);
            textQueryDescription.Size = new Size(270, 100);
            textQueryDescription.Multiline = true;
            textQueryDescription.ReadOnly = true;
            textQueryDescription.ScrollBars = ScrollBars.Vertical;
            descPanel.Controls.Add(textQueryDescription);

            buttonSaveQuery = new Button();
            buttonSaveQuery.Text = "Save as Custom";
            buttonSaveQuery.Location = new Point(10, 140);
            buttonSaveQuery.Width = 130;
            buttonSaveQuery.Click += ButtonSaveQuery_Click;
            descPanel.Controls.Add(buttonSaveQuery);

            buttonDeleteQuery = new Button();
            buttonDeleteQuery.Text = "Delete Custom";
            buttonDeleteQuery.Location = new Point(150, 140);
            buttonDeleteQuery.Width = 130;
            buttonDeleteQuery.Click += ButtonDeleteQuery_Click;
            descPanel.Controls.Add(buttonDeleteQuery);

            // === RIGHT SIDE - Editor + Results ===
            SplitContainer rightSplit = new SplitContainer();
            rightSplit.Dock = DockStyle.Fill;
            rightSplit.Orientation = Orientation.Horizontal;
            rightSplit.SplitterDistance = 350;
            mainSplit.Panel2.Controls.Add(rightSplit);

            // === QUERY EDITOR PANEL ===
            Panel editorPanel = new Panel();
            editorPanel.Dock = DockStyle.Fill;
            rightSplit.Panel1.Controls.Add(editorPanel);

            Label labelEditor = new Label();
            labelEditor.Text = "SQL Query Editor";
            labelEditor.Font = new Font(labelEditor.Font, FontStyle.Bold);
            labelEditor.Location = new Point(10, 10);
            labelEditor.AutoSize = true;
            editorPanel.Controls.Add(labelEditor);

            textSqlQuery = new TextBox();
            textSqlQuery.Location = new Point(10, 35);
            textSqlQuery.Size = new Size(1050, 200);
            textSqlQuery.Multiline = true;
            textSqlQuery.ScrollBars = ScrollBars.Both;
            textSqlQuery.Font = new Font("Consolas", 10);
            textSqlQuery.WordWrap = false;
            editorPanel.Controls.Add(textSqlQuery);

            // Parameters panel (hidden by default)
            panelParameters = new Panel();
            panelParameters.Location = new Point(10, 240);
            panelParameters.Size = new Size(1050, 60);
            panelParameters.BorderStyle = BorderStyle.FixedSingle;
            panelParameters.Visible = false;
            editorPanel.Controls.Add(panelParameters);

            Label labelParams = new Label();
            labelParams.Text = "Parameters:";
            labelParams.Location = new Point(5, 5);
            labelParams.AutoSize = true;
            panelParameters.Controls.Add(labelParams);

            flowParameters = new FlowLayoutPanel();
            flowParameters.Location = new Point(5, 25);
            flowParameters.Size = new Size(1040, 30);
            flowParameters.FlowDirection = FlowDirection.LeftToRight;
            panelParameters.Controls.Add(flowParameters);

            // Action buttons
            int buttonY = 305;

            buttonExecute = new Button();
            buttonExecute.Text = "Execute (F5)";
            buttonExecute.Location = new Point(10, buttonY);
            buttonExecute.Width = 120;
            buttonExecute.BackColor = Color.LightGreen;
            buttonExecute.Click += ButtonExecute_Click;
            editorPanel.Controls.Add(buttonExecute);

            buttonValidate = new Button();
            buttonValidate.Text = "Validate";
            buttonValidate.Location = new Point(140, buttonY);
            buttonValidate.Width = 100;
            buttonValidate.Click += ButtonValidate_Click;
            editorPanel.Controls.Add(buttonValidate);

            buttonExplain = new Button();
            buttonExplain.Text = "Explain Plan";
            buttonExplain.Location = new Point(250, buttonY);
            buttonExplain.Width = 110;
            buttonExplain.Click += ButtonExplain_Click;
            editorPanel.Controls.Add(buttonExplain);

            buttonClear = new Button();
            buttonClear.Text = "Clear";
            buttonClear.Location = new Point(370, buttonY);
            buttonClear.Width = 80;
            buttonClear.Click += ButtonClear_Click;
            editorPanel.Controls.Add(buttonClear);

            labelRowCount = new Label();
            labelRowCount.Location = new Point(470, buttonY + 5);
            labelRowCount.AutoSize = true;
            labelRowCount.Text = "Rows: 0";
            editorPanel.Controls.Add(labelRowCount);

            labelExecutionTime = new Label();
            labelExecutionTime.Location = new Point(600, buttonY + 5);
            labelExecutionTime.AutoSize = true;
            labelExecutionTime.Text = "Time: 0ms";
            editorPanel.Controls.Add(labelExecutionTime);

            // === RESULTS PANEL ===
            Panel resultsPanel = new Panel();
            resultsPanel.Dock = DockStyle.Fill;
            rightSplit.Panel2.Controls.Add(resultsPanel);

            Label labelResults = new Label();
            labelResults.Text = "Query Results";
            labelResults.Font = new Font(labelResults.Font, FontStyle.Bold);
            labelResults.Location = new Point(10, 10);
            labelResults.AutoSize = true;
            resultsPanel.Controls.Add(labelResults);

            gridResults = new DataGridView();
            gridResults.Location = new Point(10, 35);
            gridResults.Size = new Size(1050, 350);
            gridResults.ReadOnly = true;
            gridResults.AllowUserToAddRows = false;
            gridResults.AllowUserToDeleteRows = false;
            gridResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            resultsPanel.Controls.Add(gridResults);

            buttonExportCsv = new Button();
            buttonExportCsv.Text = "Export to CSV";
            buttonExportCsv.Location = new Point(10, 395);
            buttonExportCsv.Width = 120;
            buttonExportCsv.Click += ButtonExportCsv_Click;
            resultsPanel.Controls.Add(buttonExportCsv);

            Label labelMessages = new Label();
            labelMessages.Text = "Messages:";
            labelMessages.Location = new Point(10, 430);
            labelMessages.AutoSize = true;
            resultsPanel.Controls.Add(labelMessages);

            textMessages = new TextBox();
            textMessages.Location = new Point(10, 450);
            textMessages.Size = new Size(1050, 60);
            textMessages.Multiline = true;
            textMessages.ScrollBars = ScrollBars.Vertical;
            textMessages.ReadOnly = true;
            textMessages.BackColor = Color.LightYellow;
            resultsPanel.Controls.Add(textMessages);

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += QueryEditorForm_KeyDown;
        }

        private void LoadCategories()
        {
            comboCategory.Items.Clear();
            comboCategory.Items.Add("All");

            List<string> categories = _libraryService.GetCategories();
            foreach (string category in categories)
            {
                comboCategory.Items.Add(category);
            }

            if (comboCategory.Items.Count > 0)
            {
                comboCategory.SelectedIndex = 0;
            }
        }

        private void LoadQueries()
        {
            string selectedCategory = comboCategory.SelectedItem != null ? comboCategory.SelectedItem.ToString() : "All";

            listQueries.Items.Clear();

            List<QueryTemplate> queries = null;
            if (selectedCategory == "All")
            {
                queries = _libraryService.GetAllQueries();
            }
            else
            {
                queries = _libraryService.GetQueriesByCategory(selectedCategory);
            }

            foreach (QueryTemplate query in queries)
            {
                listQueries.Items.Add(query.Name + (query.IsBuiltIn ? "" : " *"));
                listQueries.Tag = queries;  // Store full list
            }
        }

        private void ComboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadQueries();
        }

        private void ListQueries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listQueries.SelectedIndex >= 0)
            {
                List<QueryTemplate> queries = listQueries.Tag as List<QueryTemplate>;
                if (queries != null && listQueries.SelectedIndex < queries.Count)
                {
                    QueryTemplate query = queries[listQueries.SelectedIndex];
                    textQueryDescription.Text = query.Description;
                }
            }
        }

        private void ButtonLoadQuery_Click(object sender, EventArgs e)
        {
            if (listQueries.SelectedIndex >= 0)
            {
                List<QueryTemplate> queries = listQueries.Tag as List<QueryTemplate>;
                if (queries != null && listQueries.SelectedIndex < queries.Count)
                {
                    _currentQuery = queries[listQueries.SelectedIndex];
                    textSqlQuery.Text = _currentQuery.SqlQuery;
                    textQueryDescription.Text = _currentQuery.Description;

                    // Setup parameters if needed
                    if (_currentQuery.HasParameters)
                    {
                        SetupParameterInputs();
                    }
                    else
                    {
                        panelParameters.Visible = false;
                    }

                    AddMessage("Loaded query: " + _currentQuery.Name);
                }
            }
            else
            {
                MessageBox.Show("Please select a query from the list", "No Query Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupParameterInputs()
        {
            flowParameters.Controls.Clear();

            if (_currentQuery == null || !_currentQuery.HasParameters)
            {
                return;
            }

            string[] paramNames = _currentQuery.ParameterNames.Split(',');
            foreach (string paramName in paramNames)
            {
                Label lbl = new Label();
                lbl.Text = paramName.Trim() + ":";
                lbl.AutoSize = true;
                lbl.Margin = new Padding(5, 5, 5, 5);
                flowParameters.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = "param_" + paramName.Trim();
                txt.Width = 150;
                txt.Margin = new Padding(0, 2, 15, 2);
                flowParameters.Controls.Add(txt);
            }

            panelParameters.Visible = true;
        }

        private Dictionary<string, object> GetParameterValues()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (Control ctrl in flowParameters.Controls)
            {
                if (ctrl is TextBox && ctrl.Name.StartsWith("param_"))
                {
                    string paramName = ctrl.Name.Substring(6);
                    parameters[paramName] = ctrl.Text;
                }
            }

            return parameters;
        }

        private void ButtonNewQuery_Click(object sender, EventArgs e)
        {
            _currentQuery = null;
            textSqlQuery.Text = "-- Enter your SQL query here\nSELECT ";
            textQueryDescription.Text = "";
            panelParameters.Visible = false;
            AddMessage("New query editor opened");
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textSqlQuery.Text))
            {
                MessageBox.Show("Please enter a SQL query", "Empty Query",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                AddMessage("Executing query...");

                DateTime startTime = DateTime.Now;

                Dictionary<string, object> parameters = null;
                if (panelParameters.Visible)
                {
                    parameters = GetParameterValues();
                }

                DataTable dt = _executorService.ExecuteQuery(textSqlQuery.Text, parameters);

                TimeSpan executionTime = DateTime.Now - startTime;

                gridResults.DataSource = dt;

                labelRowCount.Text = "Rows: " + dt.Rows.Count;
                labelExecutionTime.Text = "Time: " + executionTime.TotalMilliseconds.ToString("F0") + "ms";

                AddMessage("Query executed successfully. " + dt.Rows.Count + " rows returned.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Query execution failed:\n\n" + ex.Message, "Execution Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddMessage("ERROR: " + ex.Message);
            }
        }

        private void ButtonValidate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textSqlQuery.Text))
            {
                return;
            }

            try
            {
                QueryValidationResult result = _executorService.ValidateQuery(textSqlQuery.Text);

                if (result.IsValid)
                {
                    string message = "Query validation passed.";
                    if (result.Warnings.Count > 0)
                    {
                        message = message + "\n\nWarnings:\n- " + string.Join("\n- ", result.Warnings.ToArray());
                    }
                    MessageBox.Show(message, "Validation Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddMessage("Validation: PASSED");
                }
                else
                {
                    MessageBox.Show("Query validation failed:\n\n" + result.ErrorMessage, "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddMessage("Validation: FAILED - " + result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Validation error:\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonExplain_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textSqlQuery.Text))
            {
                return;
            }

            try
            {
                AddMessage("Getting query execution plan...");
                DataTable dt = _executorService.GetQueryPlan(textSqlQuery.Text);
                gridResults.DataSource = dt;
                AddMessage("Execution plan retrieved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get execution plan:\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddMessage("ERROR: " + ex.Message);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textSqlQuery.Text = "";
            gridResults.DataSource = null;
            textMessages.Text = "";
            labelRowCount.Text = "Rows: 0";
            labelExecutionTime.Text = "Time: 0ms";
            panelParameters.Visible = false;
        }

        private void ButtonSaveQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textSqlQuery.Text))
            {
                MessageBox.Show("Please enter a SQL query to save", "Empty Query",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show input dialog for query name
            string queryName = InputDialog.Show(
                "Enter a name for this query:", "Save Query", "My Custom Query");

            if (string.IsNullOrWhiteSpace(queryName))
            {
                return;
            }

            string category = InputDialog.Show(
                "Enter category:", "Query Category", "Custom");

            if (string.IsNullOrWhiteSpace(category))
            {
                category = "Custom";
            }

            string description = InputDialog.Show(
                "Enter description (optional):", "Query Description", "");

            try
            {
                QueryTemplate newQuery = new QueryTemplate
                {
                    Name = queryName,
                    Category = category,
                    Description = description,
                    SqlQuery = textSqlQuery.Text,
                    HasParameters = false,
                    IsBuiltIn = false
                };

                _libraryService.SaveQuery(newQuery);

                MessageBox.Show("Query saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCategories();
                LoadQueries();
                AddMessage("Query saved: " + queryName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save query:\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonDeleteQuery_Click(object sender, EventArgs e)
        {
            if (listQueries.SelectedIndex >= 0)
            {
                List<QueryTemplate> queries = listQueries.Tag as List<QueryTemplate>;
                if (queries != null && listQueries.SelectedIndex < queries.Count)
                {
                    QueryTemplate query = queries[listQueries.SelectedIndex];

                    if (query.IsBuiltIn)
                    {
                        MessageBox.Show("Cannot delete built-in queries.", "Built-in Query",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to delete this query?\n\n" + query.Name,
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            _libraryService.DeleteQuery(query.Id);
                            MessageBox.Show("Query deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadQueries();
                            AddMessage("Query deleted: " + query.Name);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to delete query:\n\n" + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a custom query to delete", "No Query Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonExportCsv_Click(object sender, EventArgs e)
        {
            if (gridResults.DataSource == null)
            {
                MessageBox.Show("No results to export", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveDialog.DefaultExt = "csv";
            saveDialog.FileName = "query_results_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = gridResults.DataSource as DataTable;
                    _executorService.ExportToCsv(dt, saveDialog.FileName);

                    MessageBox.Show("Results exported successfully to:\n" + saveDialog.FileName,
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddMessage("Exported to: " + saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed:\n\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void QueryEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ButtonExecute_Click(sender, e);
                e.Handled = true;
            }
        }

        private void AddMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            textMessages.AppendText("[" + timestamp + "] " + message + Environment.NewLine);
        }
    }
}

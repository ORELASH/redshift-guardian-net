using System;
using System.Collections.Generic;
using System.Text;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Search result for help content
    /// </summary>
    public class HelpSearchResult
    {
        public string TopicId { get; set; }
        public string Title { get; set; }
        public string Preview { get; set; }

        public string DisplayText
        {
            get { return Title + " - " + Preview; }
        }
    }

    /// <summary>
    /// Provides help content for the Help Viewer
    /// </summary>
    public class HelpContentProvider
    {
        private Dictionary<string, string> _topicTitles;
        private Dictionary<string, string> _topicContent;

        public HelpContentProvider()
        {
            InitializeTopics();
        }

        private void InitializeTopics()
        {
            _topicTitles = new Dictionary<string, string>();
            _topicContent = new Dictionary<string, string>();

            // Welcome
            AddTopic("welcome", "Welcome to Redshift Guardian", GetWelcomeContent());

            // Getting Started
            AddTopic("getting_started", "Getting Started", GetGettingStartedContent());
            AddTopic("installation", "Installation", GetInstallationContent());
            AddTopic("first_steps", "First Steps", GetFirstStepsContent());
            AddTopic("adding_clusters", "Adding Clusters", GetAddingClustersContent());

            // Cluster Management
            AddTopic("cluster_management", "Cluster Management", GetClusterManagementContent());
            AddTopic("cluster_add", "Adding Clusters", GetClusterAddContent());
            AddTopic("cluster_edit", "Editing Clusters", GetClusterEditContent());
            AddTopic("cluster_test", "Testing Connections", GetClusterTestContent());
            AddTopic("odbc_config", "ODBC Configuration", GetOdbcConfigContent());

            // Permissions Scanning
            AddTopic("permissions_scanning", "Permissions Scanning", GetPermissionsScanningContent());
            AddTopic("scan_run", "Running a Scan", GetScanRunContent());
            AddTopic("scan_results", "Viewing Results", GetScanResultsContent());
            AddTopic("scan_understanding", "Understanding Permissions", GetScanUnderstandingContent());

            // Query Editor
            AddTopic("query_editor", "SQL Query Editor", GetQueryEditorContent());
            AddTopic("query_open", "Opening Query Editor", GetQueryOpenContent());
            AddTopic("query_library", "Query Library", GetQueryLibraryContent());
            AddTopic("query_custom", "Writing Custom Queries", GetQueryCustomContent());
            AddTopic("query_parameters", "Using Parameters", GetQueryParametersContent());
            AddTopic("query_export", "Exporting Results", GetQueryExportContent());
            AddTopic("query_builtin", "Built-in Queries Reference", GetQueryBuiltinContent());

            // SQL Reference
            AddTopic("sql_reference", "SQL Queries Reference", GetSqlReferenceContent());
            AddTopic("sql_users", "User Queries", GetSqlUsersContent());
            AddTopic("sql_roles", "Role Queries", GetSqlRolesContent());
            AddTopic("sql_permissions", "Permission Queries", GetSqlPermissionsContent());
            AddTopic("sql_tables", "Table Queries", GetSqlTablesContent());
            AddTopic("sql_schemas", "Schema Queries", GetSqlSchemasContent());
            AddTopic("sql_catalog", "System Catalog", GetSqlCatalogContent());

            // Troubleshooting
            AddTopic("troubleshooting", "Troubleshooting", GetTroubleshootingContent());
            AddTopic("trouble_connection", "Connection Issues", GetTroubleConnectionContent());
            AddTopic("trouble_build", "Build Errors", GetTroubleBuildContent());
            AddTopic("trouble_query", "Query Errors", GetTroubleQueryContent());
            AddTopic("trouble_database", "Database Issues", GetTroubleDatabaseContent());

            // About
            AddTopic("about", "About Redshift Guardian", GetAboutContent());
            AddTopic("about_version", "Version Info", GetAboutVersionContent());
            AddTopic("about_capabilities", "System Capabilities", GetAboutCapabilitiesContent());
            AddTopic("about_technical", "Technical Details", GetAboutTechnicalContent());
        }

        private void AddTopic(string id, string title, string content)
        {
            _topicTitles[id] = title;
            _topicContent[id] = content;
        }

        public string GetTopicTitle(string topicId)
        {
            if (_topicTitles.ContainsKey(topicId))
            {
                return _topicTitles[topicId];
            }
            return "Unknown Topic";
        }

        public string GetTopicContent(string topicId)
        {
            if (_topicContent.ContainsKey(topicId))
            {
                return _topicContent[topicId];
            }
            return CreateRtfDocument("Topic Not Found", "The requested help topic could not be found.");
        }

        public List<HelpSearchResult> SearchContent(string searchText)
        {
            List<HelpSearchResult> results = new List<HelpSearchResult>();
            string searchLower = searchText.ToLower();

            foreach (string topicId in _topicContent.Keys)
            {
                string title = _topicTitles[topicId];
                string content = _topicContent[topicId];

                // Remove RTF formatting for search
                string plainContent = StripRtf(content);

                if (title.ToLower().Contains(searchLower) ||
                    plainContent.ToLower().Contains(searchLower))
                {
                    int index = plainContent.ToLower().IndexOf(searchLower);
                    string preview = "";
                    if (index >= 0)
                    {
                        int start = Math.Max(0, index - 50);
                        int length = Math.Min(100, plainContent.Length - start);
                        preview = plainContent.Substring(start, length).Trim();
                        if (start > 0) preview = "..." + preview;
                        if (start + length < plainContent.Length) preview = preview + "...";
                    }

                    results.Add(new HelpSearchResult
                    {
                        TopicId = topicId,
                        Title = title,
                        Preview = preview
                    });
                }
            }

            return results;
        }

        private string StripRtf(string rtf)
        {
            // Simple RTF stripper - removes RTF control sequences
            StringBuilder plain = new StringBuilder();
            bool inControlWord = false;
            bool inGroup = false;

            for (int i = 0; i < rtf.Length; i++)
            {
                char c = rtf[i];

                if (c == '\\')
                {
                    inControlWord = true;
                }
                else if (inControlWord)
                {
                    if (c == ' ' || c == '\n' || c == '\r')
                    {
                        inControlWord = false;
                    }
                }
                else if (c == '{')
                {
                    inGroup = true;
                }
                else if (c == '}')
                {
                    inGroup = false;
                }
                else if (!inControlWord && !inGroup)
                {
                    plain.Append(c);
                }
            }

            return plain.ToString();
        }

        // RTF Helper Methods
        private string CreateRtfDocument(string title, string body)
        {
            StringBuilder rtf = new StringBuilder();
            rtf.AppendLine(@"{\rtf1\ansi\deff0");
            rtf.AppendLine(@"{\fonttbl{\f0\fswiss Segoe UI;}{\f1\fmodern Consolas;}}");
            rtf.AppendLine(@"{\colortbl;\red0\green0\blue0;\red0\green0\blue255;\red0\green128\blue0;\red255\green0\blue0;}");
            rtf.AppendLine(@"\viewkind4\uc1\pard");
            rtf.AppendLine(@"\f0\fs28\b " + EscapeRtf(title) + @"\b0\fs20");
            rtf.AppendLine(@"\par\par");
            rtf.AppendLine(body);
            rtf.AppendLine(@"}");
            return rtf.ToString();
        }

        private string EscapeRtf(string text)
        {
            if (text == null) return "";
            return text.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}");
        }

        private string RtfBold(string text)
        {
            return @"\b " + EscapeRtf(text) + @"\b0 ";
        }

        private string RtfItalic(string text)
        {
            return @"\i " + EscapeRtf(text) + @"\i0 ";
        }

        private string RtfHeading(string text)
        {
            return @"\par\fs24\b " + EscapeRtf(text) + @"\b0\fs20\par\par";
        }

        private string RtfCode(string text)
        {
            return @"\f1\fs18 " + EscapeRtf(text) + @"\f0\fs20 ";
        }

        private string RtfParagraph(string text)
        {
            return EscapeRtf(text) + @"\par\par";
        }

        private string RtfBullet(string text)
        {
            return @"\bullet " + EscapeRtf(text) + @"\par";
        }

        // Content Methods
        private string GetWelcomeContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Welcome to Redshift Guardian"));
            content.Append(RtfParagraph("Redshift Guardian is a comprehensive management tool for Amazon Redshift permissions and access control."));
            content.Append(RtfHeading("Key Features:"));
            content.Append(RtfBullet("Cluster Management - Add, edit, and manage multiple Redshift clusters"));
            content.Append(RtfBullet("Permissions Scanning - Scan clusters for user permissions and roles"));
            content.Append(RtfBullet("SQL Query Editor - Execute queries with built-in library of 17 queries"));
            content.Append(RtfBullet("Export Capabilities - Export results to CSV for analysis"));
            content.Append(RtfBullet("Local Caching - Cache scan results for offline access"));
            content.Append(@"\par");
            content.Append(RtfHeading("Getting Started:"));
            content.Append(RtfParagraph("1. Add a Redshift cluster with connection details\n2. Test the connection\n3. Run a permissions scan\n4. View results and export as needed"));
            content.Append(RtfHeading("Need Help?"));
            content.Append(RtfParagraph("Browse topics in the left panel or use the Search feature to find specific information."));

            return CreateRtfDocument("Welcome", content.ToString());
        }

        private string GetGettingStartedContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Getting Started with Redshift Guardian"));
            content.Append(RtfParagraph("Follow these steps to start using Redshift Guardian:"));
            content.Append(RtfHeading("Prerequisites:"));
            content.Append(RtfBullet("Amazon Redshift ODBC Driver installed"));
            content.Append(RtfBullet("Redshift cluster credentials (host, port, database, username, password)"));
            content.Append(RtfBullet("Network access to Redshift cluster"));
            content.Append(@"\par");
            content.Append(RtfHeading("Quick Start:"));
            content.Append(RtfParagraph("1. Launch the application"));
            content.Append(RtfParagraph("2. Click 'Add' to add your first cluster"));
            content.Append(RtfParagraph("3. Enter connection details"));
            content.Append(RtfParagraph("4. Click 'Test Connection' to verify"));
            content.Append(RtfParagraph("5. Click 'Scan' to perform permissions scan"));
            content.Append(RtfParagraph("6. Click 'View Permissions' to see results"));

            return CreateRtfDocument("Getting Started", content.ToString());
        }

        private string GetInstallationContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Installation"));
            content.Append(RtfParagraph("Redshift Guardian requires the following components:"));
            content.Append(RtfHeading("Required:"));
            content.Append(RtfBullet(".NET Framework 4.0 or later"));
            content.Append(RtfBullet("SQL Server Compact 4.0 Runtime"));
            content.Append(RtfBullet("Amazon Redshift ODBC Driver"));
            content.Append(@"\par");
            content.Append(RtfHeading("Installation Steps:"));
            content.Append(RtfParagraph("1. Install .NET Framework 4.0 (if not already installed)"));
            content.Append(RtfParagraph("2. Download and install SQL Server Compact 4.0 from Microsoft"));
            content.Append(RtfParagraph("3. Download and install Amazon Redshift ODBC Driver from AWS"));
            content.Append(RtfParagraph("4. Extract RedshiftGuardianNET.zip to desired location"));
            content.Append(RtfParagraph("5. Run RedshiftGuardianNET.exe"));
            content.Append(RtfHeading("First Launch:"));
            content.Append(RtfParagraph("On first launch, the application will create a local database file (RedshiftGuardian.sdf) to store cluster configurations and cached scan results."));

            return CreateRtfDocument("Installation", content.ToString());
        }

        private string GetFirstStepsContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("First Steps"));
            content.Append(RtfParagraph("After installing Redshift Guardian, follow these steps:"));
            content.Append(RtfHeading("Step 1: Add a Cluster"));
            content.Append(RtfParagraph("Click the 'Add' button in the toolbar. Fill in the cluster details:"));
            content.Append(RtfBullet("Cluster Name - A friendly name for identification"));
            content.Append(RtfBullet("Host - The Redshift cluster endpoint"));
            content.Append(RtfBullet("Port - Usually 5439"));
            content.Append(RtfBullet("Database - The database name (e.g., 'dev')"));
            content.Append(RtfBullet("Username - Your Redshift username"));
            content.Append(RtfBullet("Password - Your password"));
            content.Append(@"\par");
            content.Append(RtfHeading("Step 2: Test Connection"));
            content.Append(RtfParagraph("Select the cluster and click 'Test Connection'. You should see a success message if the connection is working properly."));
            content.Append(RtfHeading("Step 3: Run Your First Scan"));
            content.Append(RtfParagraph("With the cluster selected, click 'Scan' to perform a permissions scan. This will query the Redshift system tables and cache the results locally."));
            content.Append(RtfHeading("Step 4: View Results"));
            content.Append(RtfParagraph("Click 'View Permissions' to see the scan results. You can filter by user, role, or schema."));

            return CreateRtfDocument("First Steps", content.ToString());
        }

        private string GetAddingClustersContent()
        {
            return GetClusterAddContent();
        }

        private string GetClusterManagementContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Cluster Management"));
            content.Append(RtfParagraph("Manage your Redshift clusters from the main dashboard."));
            content.Append(RtfHeading("Available Operations:"));
            content.Append(RtfBullet("Add - Create a new cluster configuration"));
            content.Append(RtfBullet("Edit - Modify existing cluster settings"));
            content.Append(RtfBullet("Delete - Remove a cluster configuration"));
            content.Append(RtfBullet("Test Connection - Verify ODBC connectivity"));
            content.Append(RtfBullet("Scan - Perform permissions scan"));
            content.Append(RtfBullet("View Permissions - Display cached scan results"));
            content.Append(RtfBullet("Refresh - Reload cluster list"));
            content.Append(@"\par");
            content.Append(RtfHeading("Cluster Information Displayed:"));
            content.Append(RtfParagraph("The cluster list shows: Cluster Name, Host, Port, Database, Username, and Last Scan date."));
            content.Append(RtfHeading("Tips:"));
            content.Append(RtfBullet("Double-click a cluster to edit it"));
            content.Append(RtfBullet("Always test connection before scanning"));
            content.Append(RtfBullet("Scan regularly to keep permissions data up-to-date"));

            return CreateRtfDocument("Cluster Management", content.ToString());
        }

        private string GetClusterAddContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Adding a Cluster"));
            content.Append(RtfParagraph("To add a new Redshift cluster:"));
            content.Append(RtfHeading("Steps:"));
            content.Append(RtfParagraph("1. Click the 'Add' button in the toolbar"));
            content.Append(RtfParagraph("2. Fill in the cluster details in the dialog"));
            content.Append(RtfParagraph("3. Click 'Save' to store the configuration"));
            content.Append(RtfHeading("Required Fields:"));
            content.Append(RtfBullet("Cluster Name - A unique friendly name"));
            content.Append(RtfBullet("Host - The cluster endpoint (without port)"));
            content.Append(RtfBullet("Port - Default is 5439"));
            content.Append(RtfBullet("Database - The database name"));
            content.Append(RtfBullet("Username - Redshift username with appropriate permissions"));
            content.Append(RtfBullet("Password - The user's password"));
            content.Append(@"\par");
            content.Append(RtfHeading("Example:"));
            content.Append(RtfParagraph("Cluster Name: Production Cluster\nHost: prod-cluster.abc123.us-east-1.redshift.amazonaws.com\nPort: 5439\nDatabase: analytics\nUsername: admin\nPassword: ********"));
            content.Append(RtfHeading("Important Notes:"));
            content.Append(RtfBullet("Passwords are stored encrypted in the local database"));
            content.Append(RtfBullet("The username needs SELECT permissions on system tables"));
            content.Append(RtfBullet("Test the connection after adding"));

            return CreateRtfDocument("Adding a Cluster", content.ToString());
        }

        private string GetClusterEditContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Editing a Cluster"));
            content.Append(RtfParagraph("To edit an existing cluster configuration:"));
            content.Append(RtfHeading("Methods:"));
            content.Append(RtfParagraph("Method 1: Select the cluster and click 'Edit' button"));
            content.Append(RtfParagraph("Method 2: Double-click the cluster in the list"));
            content.Append(RtfHeading("Editable Fields:"));
            content.Append(RtfBullet("Cluster Name"));
            content.Append(RtfBullet("Host"));
            content.Append(RtfBullet("Port"));
            content.Append(RtfBullet("Database"));
            content.Append(RtfBullet("Username"));
            content.Append(RtfBullet("Password"));
            content.Append(@"\par");
            content.Append(RtfHeading("Notes:"));
            content.Append(RtfBullet("Editing a cluster does NOT delete cached scan results"));
            content.Append(RtfBullet("After changing connection details, test the connection"));
            content.Append(RtfBullet("Re-scan after editing to update cached permissions"));

            return CreateRtfDocument("Editing a Cluster", content.ToString());
        }

        private string GetClusterTestContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Testing Connections"));
            content.Append(RtfParagraph("Test the ODBC connection to ensure connectivity before scanning."));
            content.Append(RtfHeading("How to Test:"));
            content.Append(RtfParagraph("1. Select a cluster from the list"));
            content.Append(RtfParagraph("2. Click 'Test Connection' button"));
            content.Append(RtfParagraph("3. Wait for the result message"));
            content.Append(RtfHeading("Success:"));
            content.Append(RtfParagraph("If successful, you'll see: 'Connection successful! Connected to [cluster_name]'"));
            content.Append(RtfHeading("Failure:"));
            content.Append(RtfParagraph("If failed, you'll see an error message with details. Common issues:"));
            content.Append(RtfBullet("Invalid credentials"));
            content.Append(RtfBullet("Network connectivity issues"));
            content.Append(RtfBullet("Firewall blocking port 5439"));
            content.Append(RtfBullet("ODBC driver not installed"));
            content.Append(RtfBullet("Cluster is paused or unavailable"));
            content.Append(@"\par");
            content.Append(RtfHeading("Troubleshooting:"));
            content.Append(RtfParagraph("See the 'Connection Issues' topic in Troubleshooting section for detailed help."));

            return CreateRtfDocument("Testing Connections", content.ToString());
        }

        private string GetOdbcConfigContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("ODBC Configuration"));
            content.Append(RtfParagraph("Redshift Guardian uses the Amazon Redshift ODBC Driver to connect to clusters."));
            content.Append(RtfHeading("Driver Installation:"));
            content.Append(RtfParagraph("1. Download the 64-bit Amazon Redshift ODBC Driver from AWS"));
            content.Append(RtfParagraph("2. Run the installer"));
            content.Append(RtfParagraph("3. Complete the installation wizard"));
            content.Append(RtfHeading("Connection String Format:"));
            content.Append(RtfParagraph("The application automatically builds ODBC connection strings in this format:"));
            content.Append(RtfCode("Driver={Amazon Redshift (x64)};Server=host;Port=5439;Database=db;UID=user;PWD=pass"));
            content.Append(@"\par\par");
            content.Append(RtfHeading("IAM Authentication:"));
            content.Append(RtfParagraph("For IAM authentication, additional parameters can be added:"));
            content.Append(RtfBullet("AccessKeyID - AWS access key"));
            content.Append(RtfBullet("SecretAccessKey - AWS secret key"));
            content.Append(RtfBullet("SessionToken - Optional session token"));
            content.Append(@"\par");
            content.Append(RtfHeading("Verification:"));
            content.Append(RtfParagraph("To verify ODBC driver installation:"));
            content.Append(RtfParagraph("1. Open Control Panel > Administrative Tools > ODBC Data Sources (64-bit)"));
            content.Append(RtfParagraph("2. Go to 'Drivers' tab"));
            content.Append(RtfParagraph("3. Look for 'Amazon Redshift (x64)' in the list"));

            return CreateRtfDocument("ODBC Configuration", content.ToString());
        }

        private string GetPermissionsScanningContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Permissions Scanning"));
            content.Append(RtfParagraph("Scan Redshift clusters to extract user permissions and roles."));
            content.Append(RtfHeading("What is Scanned:"));
            content.Append(RtfBullet("All users and their attributes"));
            content.Append(RtfBullet("All groups/roles"));
            content.Append(RtfBullet("Schema-level permissions"));
            content.Append(RtfBullet("Table-level permissions"));
            content.Append(RtfBullet("User-to-group memberships"));
            content.Append(RtfBullet("Role hierarchies and lineage"));
            content.Append(@"\par");
            content.Append(RtfHeading("System Tables Queried:"));
            content.Append(RtfBullet("pg_user - User accounts"));
            content.Append(RtfBullet("pg_group - Groups/roles"));
            content.Append(RtfBullet("pg_namespace - Schemas"));
            content.Append(RtfBullet("pg_class - Tables and views"));
            content.Append(RtfBullet("pg_user_group_mapping - Memberships"));
            content.Append(RtfBullet("has_schema_privilege - Schema permissions"));
            content.Append(RtfBullet("has_table_privilege - Table permissions"));
            content.Append(@"\par");
            content.Append(RtfHeading("Scan Duration:"));
            content.Append(RtfParagraph("Typical scan times: 10-60 seconds depending on cluster size and number of objects."));
            content.Append(RtfHeading("Caching:"));
            content.Append(RtfParagraph("Scan results are cached in the local database. Re-scan periodically to update."));

            return CreateRtfDocument("Permissions Scanning", content.ToString());
        }

        private string GetScanRunContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Running a Scan"));
            content.Append(RtfParagraph("To perform a permissions scan:"));
            content.Append(RtfHeading("Steps:"));
            content.Append(RtfParagraph("1. Select a cluster from the list"));
            content.Append(RtfParagraph("2. Ensure connection is tested (recommended)"));
            content.Append(RtfParagraph("3. Click the 'Scan' button in the toolbar"));
            content.Append(RtfParagraph("4. Wait for the scan to complete"));
            content.Append(RtfParagraph("5. A success message will show the number of permissions found"));
            content.Append(RtfHeading("During the Scan:"));
            content.Append(RtfParagraph("The application will:"));
            content.Append(RtfBullet("Connect to the cluster via ODBC"));
            content.Append(RtfBullet("Query system catalog tables"));
            content.Append(RtfBullet("Process and store results locally"));
            content.Append(RtfBullet("Update the 'Last Scan' timestamp"));
            content.Append(@"\par");
            content.Append(RtfHeading("Permissions Required:"));
            content.Append(RtfParagraph("The scanning user needs SELECT permission on:"));
            content.Append(RtfBullet("pg_user"));
            content.Append(RtfBullet("pg_group"));
            content.Append(RtfBullet("pg_namespace"));
            content.Append(RtfBullet("pg_class"));
            content.Append(@"\par");
            content.Append(RtfHeading("Troubleshooting:"));
            content.Append(RtfParagraph("If scan fails, check: Network connectivity, credentials validity, user permissions on system tables."));

            return CreateRtfDocument("Running a Scan", content.ToString());
        }

        private string GetScanResultsContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Viewing Scan Results"));
            content.Append(RtfParagraph("View cached permissions data from previous scans."));
            content.Append(RtfHeading("To View Results:"));
            content.Append(RtfParagraph("1. Select a cluster that has been scanned"));
            content.Append(RtfParagraph("2. Click 'View Permissions' button"));
            content.Append(RtfParagraph("3. Results window opens with permissions data"));
            content.Append(RtfHeading("Results Display:"));
            content.Append(RtfParagraph("The permissions view shows:"));
            content.Append(RtfBullet("Username - The user or role name"));
            content.Append(RtfBullet("Object Name - Schema or table name"));
            content.Append(RtfBullet("Object Type - Schema, Table, View"));
            content.Append(RtfBullet("Permission - SELECT, INSERT, UPDATE, DELETE, etc."));
            content.Append(RtfBullet("Grant Option - Whether user can grant to others"));
            content.Append(@"\par");
            content.Append(RtfHeading("Filtering:"));
            content.Append(RtfParagraph("Use the filter textbox to search for specific users, objects, or permissions."));
            content.Append(RtfHeading("Export:"));
            content.Append(RtfParagraph("Export filtered results to CSV for further analysis in Excel or other tools."));

            return CreateRtfDocument("Viewing Scan Results", content.ToString());
        }

        private string GetScanUnderstandingContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Understanding Permissions"));
            content.Append(RtfParagraph("Redshift uses a role-based access control (RBAC) model similar to PostgreSQL."));
            content.Append(RtfHeading("Permission Types:"));
            content.Append(RtfBold("Schema Permissions:"));
            content.Append(@"\par");
            content.Append(RtfBullet("CREATE - Create objects in schema"));
            content.Append(RtfBullet("USAGE - Access schema"));
            content.Append(@"\par");
            content.Append(RtfBold("Table/View Permissions:"));
            content.Append(@"\par");
            content.Append(RtfBullet("SELECT - Read data"));
            content.Append(RtfBullet("INSERT - Add new rows"));
            content.Append(RtfBullet("UPDATE - Modify existing rows"));
            content.Append(RtfBullet("DELETE - Remove rows"));
            content.Append(RtfBullet("REFERENCES - Create foreign keys"));
            content.Append(@"\par");
            content.Append(RtfHeading("Users vs Groups:"));
            content.Append(RtfParagraph("Users: Individual database accounts\nGroups (Roles): Collections of users with shared permissions"));
            content.Append(RtfHeading("Inheritance:"));
            content.Append(RtfParagraph("Users inherit permissions from all groups they belong to. This allows for easier permission management."));
            content.Append(RtfHeading("Superusers:"));
            content.Append(RtfParagraph("Users with usesuperuser=true bypass all permission checks and have full access."));

            return CreateRtfDocument("Understanding Permissions", content.ToString());
        }

        private string GetQueryEditorContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("SQL Query Editor"));
            content.Append(RtfParagraph("Execute SQL queries against your Redshift clusters with a built-in library of 17 queries."));
            content.Append(RtfHeading("Features:"));
            content.Append(RtfBullet("Query Library with 17 built-in queries"));
            content.Append(RtfBullet("Custom query editing"));
            content.Append(RtfBullet("Parameter support"));
            content.Append(RtfBullet("Query validation"));
            content.Append(RtfBullet("Explain plan analysis"));
            content.Append(RtfBullet("CSV export"));
            content.Append(RtfBullet("Message log"));
            content.Append(@"\par");
            content.Append(RtfHeading("Access:"));
            content.Append(RtfParagraph("Tools > SQL Query Editor"));
            content.Append(RtfHeading("Layout:"));
            content.Append(RtfParagraph("The editor has three panels:"));
            content.Append(RtfBullet("Left: Query Library with categories"));
            content.Append(RtfBullet("Top Right: SQL Editor"));
            content.Append(RtfBullet("Bottom Right: Results Grid and Message Log"));
            content.Append(@"\par");
            content.Append(RtfHeading("Keyboard Shortcuts:"));
            content.Append(RtfBullet("F5 - Execute query"));
            content.Append(RtfBullet("Ctrl+N - New query"));
            content.Append(RtfBullet("Ctrl+S - Save query"));

            return CreateRtfDocument("SQL Query Editor", content.ToString());
        }

        private string GetQueryOpenContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Opening Query Editor"));
            content.Append(RtfParagraph("To open the SQL Query Editor:"));
            content.Append(RtfHeading("Method 1: From Menu"));
            content.Append(RtfParagraph("1. Go to Tools menu\n2. Click 'SQL Query Editor'"));
            content.Append(RtfHeading("Prerequisites:"));
            content.Append(RtfBullet("A cluster must be selected in the dashboard"));
            content.Append(RtfBullet("Connection must be tested (recommended)"));
            content.Append(@"\par");
            content.Append(RtfHeading("First Time:"));
            content.Append(RtfParagraph("On first launch, the Query Editor will:"));
            content.Append(RtfBullet("Initialize the query library database table"));
            content.Append(RtfBullet("Load 17 built-in queries"));
            content.Append(RtfBullet("Display the query library"));
            content.Append(@"\par");
            content.Append(RtfHeading("Connection:"));
            content.Append(RtfParagraph("The editor uses the active cluster connection from the dashboard. If no connection exists, it will attempt to connect automatically."));

            return CreateRtfDocument("Opening Query Editor", content.ToString());
        }

        private string GetQueryLibraryContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Query Library"));
            content.Append(RtfParagraph("The Query Library contains 17 built-in queries organized in 6 categories."));
            content.Append(RtfHeading("Categories:"));
            content.Append(RtfBold("1. User Management (4 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("All Users - List all database users"));
            content.Append(RtfBullet("Superusers - Show users with superuser privilege"));
            content.Append(RtfBullet("User Details - Detailed info for specific user"));
            content.Append(RtfBullet("User Sessions - Active sessions by user"));
            content.Append(@"\par");
            content.Append(RtfBold("2. Group & Role Management (3 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("All Groups - List all groups"));
            content.Append(RtfBullet("Group Members - Users in specific group"));
            content.Append(RtfBullet("User Groups - Groups for specific user"));
            content.Append(@"\par");
            content.Append(RtfBold("3. Permission Analysis (4 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("Schema Permissions - Permissions by schema"));
            content.Append(RtfBullet("Table Permissions - Permissions by table"));
            content.Append(RtfBullet("User All Permissions - All permissions for user"));
            content.Append(RtfBullet("Role Lineage - Hierarchical role inheritance"));
            content.Append(@"\par");
            content.Append(RtfBold("4. Table & Schema Info (2 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("All Schemas - List all schemas"));
            content.Append(RtfBullet("Tables by Schema - Tables in specific schema"));
            content.Append(@"\par");
            content.Append(RtfBold("5. Access Patterns (2 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("Recent Queries - Query history"));
            content.Append(RtfBullet("Table Access Stats - Usage statistics"));
            content.Append(@"\par");
            content.Append(RtfBold("6. Security & Audit (2 queries)"));
            content.Append(@"\par");
            content.Append(RtfBullet("Grant History - Permission changes"));
            content.Append(RtfBullet("Password Expiry - Users with expiring passwords"));
            content.Append(@"\par");
            content.Append(RtfHeading("Using Library Queries:"));
            content.Append(RtfParagraph("1. Browse categories in left panel\n2. Select a query\n3. Click 'Load Query'\n4. Fill parameters if needed\n5. Click 'Execute' (F5)"));

            return CreateRtfDocument("Query Library", content.ToString());
        }

        private string GetQueryCustomContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Writing Custom Queries"));
            content.Append(RtfParagraph("Create and save your own SQL queries for reuse."));
            content.Append(RtfHeading("Creating a Custom Query:"));
            content.Append(RtfParagraph("1. Click 'New Query' button"));
            content.Append(RtfParagraph("2. Write your SQL in the editor"));
            content.Append(RtfParagraph("3. Click 'Save Query'"));
            content.Append(RtfParagraph("4. Enter name, category, and description"));
            content.Append(RtfParagraph("5. Query is saved to library"));
            content.Append(RtfHeading("Supported SQL:"));
            content.Append(RtfParagraph("Any valid Redshift SQL is supported:"));
            content.Append(RtfBullet("SELECT statements"));
            content.Append(RtfBullet("JOINs and subqueries"));
            content.Append(RtfBullet("CTEs (Common Table Expressions)"));
            content.Append(RtfBullet("Window functions"));
            content.Append(RtfBullet("Aggregations"));
            content.Append(@"\par");
            content.Append(RtfHeading("Dangerous Operations Blocked:"));
            content.Append(RtfParagraph("For safety, these operations are blocked:"));
            content.Append(RtfBullet("DROP DATABASE"));
            content.Append(RtfBullet("TRUNCATE ALL"));
            content.Append(RtfBullet("DELETE without WHERE"));
            content.Append(RtfBullet("UPDATE without WHERE"));
            content.Append(@"\par");
            content.Append(RtfHeading("Best Practices:"));
            content.Append(RtfBullet("Always use WHERE clauses to limit results"));
            content.Append(RtfBullet("Test queries before saving"));
            content.Append(RtfBullet("Add comments to document query purpose"));
            content.Append(RtfBullet("Use meaningful names for saved queries"));

            return CreateRtfDocument("Writing Custom Queries", content.ToString());
        }

        private string GetQueryParametersContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Using Parameters"));
            content.Append(RtfParagraph("Create dynamic queries with parameter placeholders."));
            content.Append(RtfHeading("Parameter Syntax:"));
            content.Append(RtfParagraph("Use @ParameterName as placeholder:"));
            content.Append(RtfCode("SELECT * FROM pg_user WHERE usename = @username"));
            content.Append(@"\par\par");
            content.Append(RtfHeading("Multiple Parameters:"));
            content.Append(RtfCode("SELECT * FROM pg_namespace WHERE nspname = @schema AND nspowner = @owner"));
            content.Append(@"\par\par");
            content.Append(RtfHeading("Execution with Parameters:"));
            content.Append(RtfParagraph("When you execute a query with parameters:"));
            content.Append(RtfParagraph("1. Click 'Execute' (F5)"));
            content.Append(RtfParagraph("2. A dialog appears for each parameter"));
            content.Append(RtfParagraph("3. Enter values"));
            content.Append(RtfParagraph("4. Query runs with your values"));
            content.Append(RtfHeading("Saving Parametrized Queries:"));
            content.Append(RtfParagraph("When saving a query with parameters:"));
            content.Append(RtfBullet("Check 'Has Parameters' checkbox"));
            content.Append(RtfBullet("System auto-detects @parameter names"));
            content.Append(RtfBullet("Parameters stored with query definition"));
            content.Append(@"\par");
            content.Append(RtfHeading("Example:"));
            content.Append(RtfCode("SELECT u.usename, g.groname FROM pg_user u JOIN pg_group g ON ... WHERE u.usename = @username"));
            content.Append(@"\par\par");
            content.Append(RtfParagraph("This query will prompt for 'username' value on execution."));

            return CreateRtfDocument("Using Parameters", content.ToString());
        }

        private string GetQueryExportContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Exporting Results"));
            content.Append(RtfParagraph("Export query results to CSV format for analysis in Excel or other tools."));
            content.Append(RtfHeading("To Export:"));
            content.Append(RtfParagraph("1. Execute a query"));
            content.Append(RtfParagraph("2. View results in the grid"));
            content.Append(RtfParagraph("3. Click 'Export CSV' button"));
            content.Append(RtfParagraph("4. Choose location and filename"));
            content.Append(RtfParagraph("5. File is saved"));
            content.Append(RtfHeading("CSV Format:"));
            content.Append(RtfParagraph("The export includes:"));
            content.Append(RtfBullet("Header row with column names"));
            content.Append(RtfBullet("All result rows"));
            content.Append(RtfBullet("Proper CSV escaping for special characters"));
            content.Append(RtfBullet("UTF-8 encoding"));
            content.Append(@"\par");
            content.Append(RtfHeading("Opening in Excel:"));
            content.Append(RtfParagraph("To open exported CSV in Excel:"));
            content.Append(RtfParagraph("1. Open Excel"));
            content.Append(RtfParagraph("2. File > Open"));
            content.Append(RtfParagraph("3. Select the CSV file"));
            content.Append(RtfParagraph("4. Data appears in spreadsheet"));
            content.Append(RtfHeading("Large Result Sets:"));
            content.Append(RtfParagraph("For queries returning many rows:"));
            content.Append(RtfBullet("Add LIMIT clause to query"));
            content.Append(RtfBullet("Export may take time for 100K+ rows"));
            content.Append(RtfBullet("Consider filtering before export"));

            return CreateRtfDocument("Exporting Results", content.ToString());
        }

        private string GetQueryBuiltinContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Built-in Queries Reference"));
            content.Append(RtfParagraph("Complete reference for all 17 built-in queries."));

            content.Append(RtfHeading("USER MANAGEMENT"));
            content.Append(RtfBold("1. All Users"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Lists all database users with key attributes: username, user ID, superuser status, connection limits, and expiry dates."));

            content.Append(RtfBold("2. Superusers Only"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows users with superuser privileges. These users bypass all permission checks."));

            content.Append(RtfBold("3. User Details"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Detailed information for a specific user. Parameter: @username"));

            content.Append(RtfBold("4. Active User Sessions"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows currently active connections grouped by user."));

            content.Append(RtfHeading("GROUP & ROLE MANAGEMENT"));
            content.Append(RtfBold("5. All Groups"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Lists all groups/roles with member counts."));

            content.Append(RtfBold("6. Group Members"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows all users in a specific group. Parameter: @groupname"));

            content.Append(RtfBold("7. User's Groups"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Lists all groups a user belongs to. Parameter: @username"));

            content.Append(RtfHeading("PERMISSION ANALYSIS"));
            content.Append(RtfBold("8. Schema Permissions"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows CREATE and USAGE permissions on schemas. Parameter: @schema"));

            content.Append(RtfBold("9. Table Permissions"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows SELECT, INSERT, UPDATE, DELETE permissions on tables. Parameters: @schema, @table"));

            content.Append(RtfBold("10. User All Permissions"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Complete permissions list for a user across all objects. Parameter: @username"));

            content.Append(RtfBold("11. Role Lineage"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Hierarchical view of role inheritance using recursive CTE. Parameter: @username"));

            content.Append(RtfHeading("TABLE & SCHEMA INFO"));
            content.Append(RtfBold("12. All Schemas"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Lists all schemas with owner information."));

            content.Append(RtfBold("13. Tables by Schema"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows all tables in a schema. Parameter: @schema"));

            content.Append(RtfHeading("ACCESS PATTERNS"));
            content.Append(RtfBold("14. Recent Query History"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Shows recently executed queries from STL_QUERY."));

            content.Append(RtfBold("15. Table Access Statistics"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Usage statistics for tables showing access counts."));

            content.Append(RtfHeading("SECURITY & AUDIT"));
            content.Append(RtfBold("16. Permission Grant History"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Audit trail of permission changes from STL_DDLTEXT."));

            content.Append(RtfBold("17. Password Expiry Check"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Lists users with passwords expiring within 30 days."));

            return CreateRtfDocument("Built-in Queries Reference", content.ToString());
        }

        private string GetSqlReferenceContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("SQL Queries Reference"));
            content.Append(RtfParagraph("Comprehensive SQL reference for Redshift system catalog queries."));
            content.Append(RtfHeading("System Catalog Tables:"));
            content.Append(RtfBold("PG_USER"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Contains all database users and their attributes."));
            content.Append(RtfBold("PG_GROUP"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Contains all groups/roles."));
            content.Append(RtfBold("PG_NAMESPACE"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Contains all schemas."));
            content.Append(RtfBold("PG_CLASS"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Contains all tables, views, and relations."));
            content.Append(RtfBold("PG_DATABASE"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Database information."));
            content.Append(@"\par");
            content.Append(RtfHeading("System Functions:"));
            content.Append(RtfBold("has_schema_privilege(user, schema, privilege)"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Check if user has privilege on schema."));
            content.Append(RtfBold("has_table_privilege(user, table, privilege)"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Check if user has privilege on table."));
            content.Append(@"\par");
            content.Append(RtfHeading("System Logs:"));
            content.Append(RtfBold("STL_QUERY"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Query execution history."));
            content.Append(RtfBold("STL_DDLTEXT"));
            content.Append(@"\par");
            content.Append(RtfParagraph("DDL statement history (GRANTs, CREATEs, etc.)"));
            content.Append(RtfBold("SVV_TABLE_INFO"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Table statistics and metadata."));

            return CreateRtfDocument("SQL Queries Reference", content.ToString());
        }

        private string GetSqlUsersContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("User Queries"));
            content.Append(RtfParagraph("SQL queries for user management and analysis."));

            content.Append(RtfHeading("List All Users:"));
            content.Append(RtfCode("SELECT usename, usesysid, usecreatedb, usesuper, passwd, valuntil FROM pg_user ORDER BY usename"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Find Superusers:"));
            content.Append(RtfCode("SELECT usename FROM pg_user WHERE usesuper = true"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("User with Connection Limit:"));
            content.Append(RtfCode("SELECT usename, useconnlimit FROM pg_user WHERE useconnlimit <> -1"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Users with Password Expiry:"));
            content.Append(RtfCode("SELECT usename, valuntil FROM pg_user WHERE valuntil IS NOT NULL AND valuntil < CURRENT_DATE + 30"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("User Creation Privileges:"));
            content.Append(RtfCode("SELECT usename, usecreatedb, usecreateuser FROM pg_user"));
            content.Append(@"\par\par");

            return CreateRtfDocument("User Queries", content.ToString());
        }

        private string GetSqlRolesContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Role/Group Queries"));
            content.Append(RtfParagraph("SQL queries for group and role management."));

            content.Append(RtfHeading("List All Groups:"));
            content.Append(RtfCode("SELECT groname, grosysid FROM pg_group ORDER BY groname"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Group Members:"));
            content.Append(RtfCode("SELECT g.groname, u.usename FROM pg_group g, pg_user u WHERE u.usesysid = ANY(g.grolist) AND g.groname = 'groupname' ORDER BY u.usename"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("User's Group Memberships:"));
            content.Append(RtfCode("SELECT groname FROM pg_group WHERE grolist @> ARRAY(SELECT usesysid FROM pg_user WHERE usename = 'username')"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Groups with Member Count:"));
            content.Append(RtfCode("SELECT groname, ARRAY_LENGTH(grolist, 1) as member_count FROM pg_group ORDER BY member_count DESC"));
            content.Append(@"\par\par");

            return CreateRtfDocument("Role/Group Queries", content.ToString());
        }

        private string GetSqlPermissionsContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Permission Queries"));
            content.Append(RtfParagraph("SQL queries for permission analysis."));

            content.Append(RtfHeading("Schema Permissions:"));
            content.Append(RtfCode("SELECT nspname, has_schema_privilege(usename, nspname, 'CREATE') as can_create, has_schema_privilege(usename, nspname, 'USAGE') as can_use FROM pg_namespace, pg_user WHERE nspname NOT LIKE 'pg_%'"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Table Permissions:"));
            content.Append(RtfCode("SELECT schemaname, tablename, has_table_privilege(usename, schemaname||'.'||tablename, 'SELECT') as can_select FROM pg_tables, pg_user"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("All Permissions for User:"));
            content.Append(RtfCode("SELECT * FROM information_schema.table_privileges WHERE grantee = 'username'"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Grant Options:"));
            content.Append(RtfCode("SELECT * FROM information_schema.table_privileges WHERE is_grantable = 'YES'"));
            content.Append(@"\par\par");

            return CreateRtfDocument("Permission Queries", content.ToString());
        }

        private string GetSqlTablesContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Table Queries"));
            content.Append(RtfParagraph("SQL queries for table and view information."));

            content.Append(RtfHeading("All Tables:"));
            content.Append(RtfCode("SELECT schemaname, tablename, tableowner FROM pg_tables WHERE schemaname NOT IN ('pg_catalog', 'information_schema') ORDER BY schemaname, tablename"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Tables by Schema:"));
            content.Append(RtfCode("SELECT tablename, tableowner FROM pg_tables WHERE schemaname = 'public' ORDER BY tablename"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Table Size:"));
            content.Append(RtfCode("SELECT name, size FROM svv_table_info WHERE schema = 'public' ORDER BY size DESC"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("All Views:"));
            content.Append(RtfCode("SELECT schemaname, viewname, viewowner FROM pg_views WHERE schemaname NOT IN ('pg_catalog', 'information_schema') ORDER BY schemaname, viewname"));
            content.Append(@"\par\par");

            return CreateRtfDocument("Table Queries", content.ToString());
        }

        private string GetSqlSchemasContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Schema Queries"));
            content.Append(RtfParagraph("SQL queries for schema information."));

            content.Append(RtfHeading("All Schemas:"));
            content.Append(RtfCode("SELECT nspname, nspowner FROM pg_namespace WHERE nspname NOT LIKE 'pg_%' AND nspname <> 'information_schema' ORDER BY nspname"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Schemas with Owner Names:"));
            content.Append(RtfCode("SELECT n.nspname, u.usename as owner FROM pg_namespace n JOIN pg_user u ON n.nspowner = u.usesysid WHERE n.nspname NOT LIKE 'pg_%' ORDER BY n.nspname"));
            content.Append(@"\par\par");

            content.Append(RtfHeading("Schema Object Counts:"));
            content.Append(RtfCode("SELECT schemaname, COUNT(*) as table_count FROM pg_tables WHERE schemaname NOT IN ('pg_catalog', 'information_schema') GROUP BY schemaname ORDER BY table_count DESC"));
            content.Append(@"\par\par");

            return CreateRtfDocument("Schema Queries", content.ToString());
        }

        private string GetSqlCatalogContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("System Catalog Reference"));
            content.Append(RtfParagraph("Complete reference for Redshift system catalog tables."));

            content.Append(RtfHeading("Key System Tables:"));

            content.Append(RtfBold("PG_USER"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Columns: usename, usesysid, usecreatedb, usesuper, usecatupd, passwd, valuntil, useconfig, useconnlimit"));
            content.Append(@"\par");

            content.Append(RtfBold("PG_GROUP"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Columns: groname, grosysid, grolist (array of user IDs)"));
            content.Append(@"\par");

            content.Append(RtfBold("PG_DATABASE"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Columns: datname, datdba, encoding, datistemplate, datallowconn"));
            content.Append(@"\par");

            content.Append(RtfBold("PG_NAMESPACE"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Columns: nspname, nspowner, nspacl"));
            content.Append(@"\par");

            content.Append(RtfBold("PG_CLASS"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Columns: relname, relnamespace, reltype, relowner, relkind, reltablespace"));
            content.Append(@"\par");

            content.Append(RtfHeading("System Views:"));

            content.Append(RtfBold("PG_TABLES"));
            content.Append(@"\par");
            content.Append(RtfParagraph("User-friendly view of all tables."));
            content.Append(@"\par");

            content.Append(RtfBold("PG_VIEWS"));
            content.Append(@"\par");
            content.Append(RtfParagraph("User-friendly view of all views."));
            content.Append(@"\par");

            content.Append(RtfBold("SVV_TABLE_INFO"));
            content.Append(@"\par");
            content.Append(RtfParagraph("Detailed table statistics including size, sort keys, distribution."));

            return CreateRtfDocument("System Catalog Reference", content.ToString());
        }

        private string GetTroubleshootingContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Troubleshooting"));
            content.Append(RtfParagraph("Common issues and solutions for Redshift Guardian."));
            content.Append(RtfHeading("Common Issues:"));
            content.Append(RtfBullet("Connection failures"));
            content.Append(RtfBullet("Build errors"));
            content.Append(RtfBullet("Query execution errors"));
            content.Append(RtfBullet("Database initialization issues"));
            content.Append(@"\par");
            content.Append(RtfHeading("Quick Diagnostics:"));
            content.Append(RtfParagraph("1. Check ODBC Driver installation"));
            content.Append(RtfParagraph("2. Verify network connectivity to cluster"));
            content.Append(RtfParagraph("3. Validate credentials"));
            content.Append(RtfParagraph("4. Check local database file permissions"));
            content.Append(RtfParagraph("5. Review application logs"));
            content.Append(RtfHeading("Getting Help:"));
            content.Append(RtfParagraph("Browse specific troubleshooting topics in the left panel for detailed solutions."));

            return CreateRtfDocument("Troubleshooting", content.ToString());
        }

        private string GetTroubleConnectionContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Connection Issues"));
            content.Append(RtfParagraph("Solutions for connection problems."));

            content.Append(RtfHeading("Error: 'Invalid DSN'"));
            content.Append(RtfParagraph("Cause: ODBC driver not found"));
            content.Append(RtfParagraph("Solution: Install Amazon Redshift ODBC Driver (64-bit)"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Connection timeout'"));
            content.Append(RtfParagraph("Cause: Network connectivity issue"));
            content.Append(RtfParagraph("Solution: Check firewall, security groups, and network routing"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Authentication failed'"));
            content.Append(RtfParagraph("Cause: Invalid credentials"));
            content.Append(RtfParagraph("Solution: Verify username and password"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Database does not exist'"));
            content.Append(RtfParagraph("Cause: Wrong database name"));
            content.Append(RtfParagraph("Solution: Check database name (case-sensitive)"));
            content.Append(@"\par");

            content.Append(RtfHeading("Verification Steps:"));
            content.Append(RtfParagraph("1. Open ODBC Data Source Administrator"));
            content.Append(RtfParagraph("2. Go to Drivers tab"));
            content.Append(RtfParagraph("3. Verify 'Amazon Redshift (x64)' is listed"));
            content.Append(RtfParagraph("4. Test cluster endpoint with telnet: telnet <host> 5439"));
            content.Append(RtfParagraph("5. Verify security group allows inbound on port 5439"));

            return CreateRtfDocument("Connection Issues", content.ToString());
        }

        private string GetTroubleBuildContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Build Errors"));
            content.Append(RtfParagraph("Solutions for compilation and build issues."));

            content.Append(RtfHeading("Error: 'Type or namespace not found'"));
            content.Append(RtfParagraph("Cause: Missing assembly reference"));
            content.Append(RtfParagraph("Solution: Add required references in project properties"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'SqlServerCe namespace not found'"));
            content.Append(RtfParagraph("Cause: SQL Server Compact not referenced"));
            content.Append(RtfParagraph("Solution: Add reference to System.Data.SqlServerCe.dll"));
            content.Append(RtfParagraph("Location: C:\\Program Files\\Microsoft SQL Server Compact Edition\\v4.0\\Desktop\\"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Invalid expression term' (C# syntax)"));
            content.Append(RtfParagraph("Cause: C# 6.0 syntax in C# 4.0 project"));
            content.Append(RtfParagraph("Solution: Replace ?. with null checks, avoid $\"\" strings"));
            content.Append(@"\par");

            content.Append(RtfHeading("Build Steps:"));
            content.Append(RtfParagraph("1. Clean Solution"));
            content.Append(RtfParagraph("2. Rebuild Solution"));
            content.Append(RtfParagraph("3. Check Error List for details"));
            content.Append(RtfParagraph("4. Fix errors from top to bottom"));
            content.Append(RtfParagraph("5. Rebuild and verify"));

            return CreateRtfDocument("Build Errors", content.ToString());
        }

        private string GetTroubleQueryContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Query Execution Errors"));
            content.Append(RtfParagraph("Solutions for SQL query errors."));

            content.Append(RtfHeading("Error: 'Permission denied for relation'"));
            content.Append(RtfParagraph("Cause: User lacks SELECT permission"));
            content.Append(RtfParagraph("Solution: Grant SELECT permission or use a user with appropriate privileges"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Relation does not exist'"));
            content.Append(RtfParagraph("Cause: Table/view name incorrect or missing schema"));
            content.Append(RtfParagraph("Solution: Use fully-qualified names: schema.table"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Query validation failed'"));
            content.Append(RtfParagraph("Cause: Dangerous operation detected"));
            content.Append(RtfParagraph("Solution: Query contains blocked keywords (DROP DATABASE, TRUNCATE ALL, etc.)"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Syntax error at or near'"));
            content.Append(RtfParagraph("Cause: SQL syntax error"));
            content.Append(RtfParagraph("Solution: Review SQL syntax, check for typos, validate with EXPLAIN"));
            content.Append(@"\par");

            content.Append(RtfHeading("Tips:"));
            content.Append(RtfBullet("Use Explain Plan to debug complex queries"));
            content.Append(RtfBullet("Test queries incrementally (start simple, add complexity)"));
            content.Append(RtfBullet("Check message log for detailed error info"));

            return CreateRtfDocument("Query Execution Errors", content.ToString());
        }

        private string GetTroubleDatabaseContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Database Issues"));
            content.Append(RtfParagraph("Solutions for local database problems."));

            content.Append(RtfHeading("Error: 'Table QueryTemplates does not exist'"));
            content.Append(RtfParagraph("Cause: Database schema not initialized"));
            content.Append(RtfParagraph("Solution: Delete RedshiftGuardian.sdf and restart application"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Database file is locked'"));
            content.Append(RtfParagraph("Cause: Multiple instances running or improper shutdown"));
            content.Append(RtfParagraph("Solution: Close all instances, delete .lock files"));
            content.Append(@"\par");

            content.Append(RtfHeading("Error: 'Cannot write to database'"));
            content.Append(RtfParagraph("Cause: File permissions issue"));
            content.Append(RtfParagraph("Solution: Run as administrator or fix folder permissions"));
            content.Append(@"\par");

            content.Append(RtfHeading("Database Location:"));
            content.Append(RtfParagraph("Default: Application directory\\RedshiftGuardian.sdf"));
            content.Append(RtfHeading("Backup:"));
            content.Append(RtfParagraph("To backup: Copy RedshiftGuardian.sdf to safe location"));
            content.Append(RtfParagraph("To restore: Replace RedshiftGuardian.sdf with backup"));
            content.Append(RtfHeading("Reset:"));
            content.Append(RtfParagraph("To reset completely: Delete RedshiftGuardian.sdf, restart application"));

            return CreateRtfDocument("Database Issues", content.ToString());
        }

        private string GetAboutContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("About Redshift Guardian"));
            content.Append(RtfParagraph("Redshift Guardian is a comprehensive management tool for Amazon Redshift permissions and access control."));
            content.Append(RtfHeading("Version:"));
            content.Append(RtfParagraph("Version: 1.0.0\nBuild: .NET Framework 4.0\nPlatform: Windows (x64)"));
            content.Append(RtfHeading("Components:"));
            content.Append(RtfBullet("Core Application: 18 C# files, ~4,200 LOC"));
            content.Append(RtfBullet("Query Library: 17 built-in queries"));
            content.Append(RtfBullet("Documentation: 120+ KB"));
            content.Append(@"\par");
            content.Append(RtfHeading("Technology Stack:"));
            content.Append(RtfBullet(".NET Framework 4.0"));
            content.Append(RtfBullet("C# 4.0"));
            content.Append(RtfBullet("WinForms UI"));
            content.Append(RtfBullet("SQL Server Compact 4.0"));
            content.Append(RtfBullet("Amazon Redshift ODBC Driver"));
            content.Append(@"\par");
            content.Append(RtfHeading("Copyright:"));
            content.Append(RtfParagraph("(C) 2026 Redshift Guardian Project"));
            content.Append(RtfHeading("Support:"));
            content.Append(RtfParagraph("For help and support, refer to the documentation in this Help system."));

            return CreateRtfDocument("About", content.ToString());
        }

        private string GetAboutVersionContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Version Information"));
            content.Append(RtfParagraph("Detailed version and build information."));
            content.Append(RtfHeading("Application:"));
            content.Append(RtfParagraph("Name: Redshift Guardian .NET\nVersion: 1.0.0\nRelease Date: February 16, 2026"));
            content.Append(RtfHeading("Framework:"));
            content.Append(RtfParagraph(".NET Framework: 4.0\nC# Language: 4.0\nTarget Platform: Windows x64"));
            content.Append(RtfHeading("Dependencies:"));
            content.Append(RtfBullet("System.Data.dll"));
            content.Append(RtfBullet("System.Data.SqlServerCe.dll (4.0.8876.1)"));
            content.Append(RtfBullet("System.Windows.Forms.dll"));
            content.Append(RtfBullet("System.Drawing.dll"));
            content.Append(RtfBullet("Amazon Redshift ODBC Driver"));
            content.Append(@"\par");
            content.Append(RtfHeading("Build Info:"));
            content.Append(RtfParagraph("Compiler: Visual Studio 2010\nConfiguration: Release\nPlatform Target: Any CPU"));
            content.Append(RtfHeading("File Structure:"));
            content.Append(RtfParagraph("Assemblies: 1 main EXE\nSize: ~150 KB\nDatabase: RedshiftGuardian.sdf"));

            return CreateRtfDocument("Version Information", content.ToString());
        }

        private string GetAboutCapabilitiesContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("System Capabilities"));
            content.Append(RtfParagraph("Complete list of features and capabilities."));

            content.Append(RtfHeading("Core Features:"));
            content.Append(RtfBullet("Cluster Management (Add, Edit, Delete)"));
            content.Append(RtfBullet("ODBC Connection Management"));
            content.Append(RtfBullet("Permissions Scanning"));
            content.Append(RtfBullet("Local Result Caching"));
            content.Append(RtfBullet("SQL Query Editor with Library"));
            content.Append(RtfBullet("CSV Export"));
            content.Append(@"\par");

            content.Append(RtfHeading("Query Library:"));
            content.Append(RtfParagraph("17 built-in queries in 6 categories:"));
            content.Append(RtfBullet("User Management (4 queries)"));
            content.Append(RtfBullet("Group & Role Management (3 queries)"));
            content.Append(RtfBullet("Permission Analysis (4 queries)"));
            content.Append(RtfBullet("Table & Schema Info (2 queries)"));
            content.Append(RtfBullet("Access Patterns (2 queries)"));
            content.Append(RtfBullet("Security & Audit (2 queries)"));
            content.Append(@"\par");

            content.Append(RtfHeading("Query Editor Features:"));
            content.Append(RtfBullet("Dynamic parameter support"));
            content.Append(RtfBullet("Query validation"));
            content.Append(RtfBullet("Explain plan analysis"));
            content.Append(RtfBullet("Custom query creation"));
            content.Append(RtfBullet("Query library management"));
            content.Append(RtfBullet("Message logging"));
            content.Append(@"\par");

            content.Append(RtfHeading("Security:"));
            content.Append(RtfBullet("Encrypted password storage"));
            content.Append(RtfBullet("Dangerous query detection"));
            content.Append(RtfBullet("IAM authentication support"));
            content.Append(RtfBullet("Connection validation"));

            return CreateRtfDocument("System Capabilities", content.ToString());
        }

        private string GetAboutTechnicalContent()
        {
            StringBuilder content = new StringBuilder();
            content.Append(RtfHeading("Technical Details"));
            content.Append(RtfParagraph("Architecture and implementation details."));

            content.Append(RtfHeading("Architecture:"));
            content.Append(RtfBold("Layered Design:"));
            content.Append(@"\par");
            content.Append(RtfBullet("Presentation Layer: WinForms UI (5 forms)"));
            content.Append(RtfBullet("Business Logic Layer: Service classes (5 services)"));
            content.Append(RtfBullet("Data Access Layer: Repository pattern (3 repositories)"));
            content.Append(RtfBullet("Data Layer: SQL Server Compact database"));
            content.Append(@"\par");

            content.Append(RtfHeading("Design Patterns:"));
            content.Append(RtfBullet("Repository Pattern for data access"));
            content.Append(RtfBullet("Service Layer for business logic"));
            content.Append(RtfBullet("Dependency Injection (manual)"));
            content.Append(RtfBullet("Factory Pattern for queries"));
            content.Append(@"\par");

            content.Append(RtfHeading("Database Schema:"));
            content.Append(RtfBold("Tables:"));
            content.Append(@"\par");
            content.Append(RtfBullet("Clusters - Store cluster configurations"));
            content.Append(RtfBullet("Users - Cache Redshift users"));
            content.Append(RtfBullet("Permissions - Cache permission scans"));
            content.Append(RtfBullet("QueryTemplates - Store query library"));
            content.Append(@"\par");

            content.Append(RtfHeading("Connection Management:"));
            content.Append(RtfParagraph("ODBC connection string format:\nDriver={Amazon Redshift (x64)};Server=<host>;Port=<port>;Database=<db>;UID=<user>;PWD=<pwd>"));
            content.Append(@"\par");

            content.Append(RtfHeading("Statistics:"));
            content.Append(RtfParagraph("Total Files: 18 (.cs files)\nLines of Code: ~4,200\nDocumentation: 120+ KB\nBuilt-in Queries: 17\nForms: 5\nServices: 5\nRepositories: 3\nModels: 4"));

            return CreateRtfDocument("Technical Details", content.ToString());
        }
    }
}

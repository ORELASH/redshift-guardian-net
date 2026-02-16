using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.Services;

namespace RedshiftGuardianNET.CLI
{
    /// <summary>
    /// Command line interface executor
    /// </summary>
    public class CommandExecutor
    {
        private ClusterService _clusterService;
        private ScannerService _scannerService;
        private QueryExecutorService _queryService;

        public CommandExecutor()
        {
            _clusterService = new ClusterService();
            _scannerService = new ScannerService();
        }

        /// <summary>
        /// Execute CLI command
        /// </summary>
        public int Execute(CommandLineParser parser)
        {
            try
            {
                switch (parser.Command)
                {
                    case "help":
                        return ShowHelp();

                    case "list":
                        return ListClusters();

                    case "scan":
                        return ScanCluster(parser);

                    case "query":
                        return ExecuteQuery(parser);

                    case "export":
                        return ExportData(parser);

                    case "add":
                        return AddCluster(parser);

                    case "test":
                        return TestConnection(parser);

                    case "version":
                        return ShowVersion();

                    default:
                        Console.WriteLine("Unknown command: " + parser.Command);
                        Console.WriteLine("Use 'RedshiftGuardian.exe help' for usage information");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                if (parser.HasFlag("verbose") || parser.HasFlag("v"))
                {
                    Console.WriteLine();
                    Console.WriteLine("Stack Trace:");
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        private int ShowHelp()
        {
            Console.WriteLine("Redshift Guardian - Command Line Interface");
            Console.WriteLine("============================================");
            Console.WriteLine();
            Console.WriteLine("Usage: RedshiftGuardian.exe <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  help                     Show this help message");
            Console.WriteLine("  version                  Show version information");
            Console.WriteLine("  list                     List all configured clusters");
            Console.WriteLine("  scan                     Scan cluster for permissions");
            Console.WriteLine("  query                    Execute SQL query");
            Console.WriteLine("  export                   Export data to file");
            Console.WriteLine("  add                      Add new cluster");
            Console.WriteLine("  test                     Test cluster connection");
            Console.WriteLine();
            Console.WriteLine("Scan Options:");
            Console.WriteLine("  --cluster <name>         Cluster name to scan");
            Console.WriteLine("  --all                    Scan all clusters");
            Console.WriteLine("  --silent                 Silent mode (no output)");
            Console.WriteLine();
            Console.WriteLine("Query Options:");
            Console.WriteLine("  --cluster <name>         Cluster name");
            Console.WriteLine("  --sql <query>            SQL query to execute");
            Console.WriteLine("  --file <path>            Read SQL from file");
            Console.WriteLine("  --output <path>          Save results to file");
            Console.WriteLine("  --format <format>        Output format (table|csv|json)");
            Console.WriteLine();
            Console.WriteLine("Export Options:");
            Console.WriteLine("  --cluster <name>         Cluster name");
            Console.WriteLine("  --output <path>          Output file path");
            Console.WriteLine("  --format <format>        Export format (csv|excel|json)");
            Console.WriteLine("  --filter <text>          Filter results");
            Console.WriteLine();
            Console.WriteLine("Add Cluster Options:");
            Console.WriteLine("  --name <name>            Cluster name");
            Console.WriteLine("  --host <hostname>        Cluster hostname");
            Console.WriteLine("  --port <port>            Port (default: 5439)");
            Console.WriteLine("  --database <name>        Database name");
            Console.WriteLine("  --username <user>        Username");
            Console.WriteLine("  --password <pass>        Password");
            Console.WriteLine();
            Console.WriteLine("Test Options:");
            Console.WriteLine("  --cluster <name>         Cluster name to test");
            Console.WriteLine();
            Console.WriteLine("Global Options:");
            Console.WriteLine("  --verbose, -v            Verbose output");
            Console.WriteLine("  --quiet, -q              Quiet mode");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  RedshiftGuardian.exe scan --cluster production");
            Console.WriteLine("  RedshiftGuardian.exe query --cluster dev --sql \"SELECT * FROM pg_user\"");
            Console.WriteLine("  RedshiftGuardian.exe export --cluster prod --output report.csv");
            Console.WriteLine("  RedshiftGuardian.exe scan --all");
            Console.WriteLine();

            return 0;
        }

        private int ShowVersion()
        {
            Console.WriteLine("Redshift Guardian .NET");
            Console.WriteLine("Version: 1.0.0");
            Console.WriteLine("Build: .NET Framework 4.0");
            Console.WriteLine("Platform: Windows");
            Console.WriteLine();
            return 0;
        }

        private int ListClusters()
        {
            var clusters = _clusterService.GetAllClusters();

            if (clusters.Count == 0)
            {
                Console.WriteLine("No clusters configured.");
                return 0;
            }

            Console.WriteLine("Configured Clusters:");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            foreach (var cluster in clusters)
            {
                Console.WriteLine("Name:     " + cluster.Name);
                Console.WriteLine("Host:     " + cluster.Host + ":" + cluster.Port);
                Console.WriteLine("Database: " + cluster.Database);
                Console.WriteLine("Username: " + cluster.Username);

                if (cluster.LastScanTime.HasValue)
                {
                    Console.WriteLine("Last Scan: " + cluster.LastScanTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    Console.WriteLine("Status:    " + cluster.LastScanStatus);
                }
                else
                {
                    Console.WriteLine("Last Scan: Never");
                }

                Console.WriteLine("------------------------------------------");
            }

            Console.WriteLine();
            Console.WriteLine("Total: " + clusters.Count + " cluster(s)");

            return 0;
        }

        private int ScanCluster(CommandLineParser parser)
        {
            if (parser.HasFlag("all"))
            {
                return ScanAllClusters(parser);
            }

            string clusterName = parser.GetArgument("cluster");
            if (string.IsNullOrEmpty(clusterName))
            {
                Console.WriteLine("ERROR: --cluster parameter is required");
                return 1;
            }

            var cluster = FindCluster(clusterName);
            if (cluster == null)
            {
                Console.WriteLine("ERROR: Cluster not found: " + clusterName);
                return 1;
            }

            bool silent = parser.HasFlag("silent") || parser.HasFlag("quiet") || parser.HasFlag("q");

            if (!silent)
            {
                Console.WriteLine("Scanning cluster: " + cluster.Name);
            }

            var result = _scannerService.ScanCluster(cluster);

            if (result.Success)
            {
                if (!silent)
                {
                    Console.WriteLine("SUCCESS: Scan completed");
                    Console.WriteLine("Users found: " + result.UserCount);
                    Console.WriteLine("Permissions found: " + result.PermissionCount);
                    Console.WriteLine("Duration: " + result.Duration.TotalSeconds.ToString("F1") + "s");
                }
                return 0;
            }
            else
            {
                Console.WriteLine("ERROR: " + result.Message);
                return 1;
            }
        }

        private int ScanAllClusters(CommandLineParser parser)
        {
            var clusters = _clusterService.GetAllClusters();

            if (clusters.Count == 0)
            {
                Console.WriteLine("No clusters to scan.");
                return 0;
            }

            bool silent = parser.HasFlag("silent") || parser.HasFlag("quiet") || parser.HasFlag("q");

            if (!silent)
            {
                Console.WriteLine("Scanning " + clusters.Count + " cluster(s)...");
                Console.WriteLine();
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var cluster in clusters)
            {
                if (!silent)
                {
                    Console.Write("Scanning: " + cluster.Name + "... ");
                }

                var result = _scannerService.ScanCluster(cluster);

                if (result.Success)
                {
                    successCount++;
                    if (!silent)
                    {
                        Console.WriteLine("OK (" + result.UserCount + " users, " +
                            result.PermissionCount + " permissions)");
                    }
                }
                else
                {
                    failCount++;
                    if (!silent)
                    {
                        Console.WriteLine("FAILED");
                        Console.WriteLine("  Error: " + result.Message);
                    }
                }
            }

            if (!silent)
            {
                Console.WriteLine();
                Console.WriteLine("Results: " + successCount + " successful, " + failCount + " failed");
            }

            return failCount > 0 ? 1 : 0;
        }

        private int ExecuteQuery(CommandLineParser parser)
        {
            string clusterName = parser.GetArgument("cluster");
            if (string.IsNullOrEmpty(clusterName))
            {
                Console.WriteLine("ERROR: --cluster parameter is required");
                return 1;
            }

            var cluster = FindCluster(clusterName);
            if (cluster == null)
            {
                Console.WriteLine("ERROR: Cluster not found: " + clusterName);
                return 1;
            }

            string sql = parser.GetArgument("sql");
            if (string.IsNullOrEmpty(sql))
            {
                string sqlFile = parser.GetArgument("file");
                if (!string.IsNullOrEmpty(sqlFile))
                {
                    if (!File.Exists(sqlFile))
                    {
                        Console.WriteLine("ERROR: SQL file not found: " + sqlFile);
                        return 1;
                    }
                    sql = File.ReadAllText(sqlFile);
                }
                else
                {
                    Console.WriteLine("ERROR: --sql or --file parameter is required");
                    return 1;
                }
            }

            try
            {
                using (RedshiftOdbcService odbcService = new RedshiftOdbcService())
                {
                    odbcService.Connect(cluster);

                    _queryService = new QueryExecutorService(odbcService);
                    DataTable results = _queryService.ExecuteQuery(sql, new Dictionary<string, object>());

                    string outputPath = parser.GetArgument("output");
                    string format = parser.GetArgument("format", "table");

                    if (!string.IsNullOrEmpty(outputPath))
                    {
                        // Save to file
                        _queryService.ExportToCsv(results, outputPath);
                        Console.WriteLine("Results saved to: " + outputPath);
                    }
                    else
                    {
                        // Display in console
                        if (format == "csv")
                        {
                            PrintCsv(results);
                        }
                        else if (format == "json")
                        {
                            PrintJson(results);
                        }
                        else
                        {
                            PrintTable(results);
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Rows returned: " + results.Rows.Count);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Query execution failed");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private int ExportData(CommandLineParser parser)
        {
            string clusterName = parser.GetArgument("cluster");
            if (string.IsNullOrEmpty(clusterName))
            {
                Console.WriteLine("ERROR: --cluster parameter is required");
                return 1;
            }

            var cluster = FindCluster(clusterName);
            if (cluster == null)
            {
                Console.WriteLine("ERROR: Cluster not found: " + clusterName);
                return 1;
            }

            string outputPath = parser.GetArgument("output");
            if (string.IsNullOrEmpty(outputPath))
            {
                Console.WriteLine("ERROR: --output parameter is required");
                return 1;
            }

            string format = parser.GetArgument("format", "csv").ToLower();
            string filter = parser.GetArgument("filter", "");

            try
            {
                var permissions = _clusterService.GetCachedPermissions(cluster.Id);

                if (permissions.Count == 0)
                {
                    Console.WriteLine("No cached permissions found. Run scan first.");
                    return 1;
                }

                // Apply filter if specified
                if (!string.IsNullOrEmpty(filter))
                {
                    permissions = permissions.FindAll(p =>
                        p.Username.ToLower().Contains(filter.ToLower()) ||
                        p.ObjectName.ToLower().Contains(filter.ToLower()) ||
                        p.PermissionType.ToLower().Contains(filter.ToLower())
                    );
                }

                // Create DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("Username");
                dt.Columns.Add("ObjectName");
                dt.Columns.Add("ObjectType");
                dt.Columns.Add("PermissionType");
                dt.Columns.Add("GrantOption");

                foreach (var perm in permissions)
                {
                    dt.Rows.Add(
                        perm.Username,
                        perm.ObjectName,
                        perm.ObjectType,
                        perm.PermissionType,
                        perm.GrantOption
                    );
                }

                if (format == "csv")
                {
                    _queryService = new QueryExecutorService(null);
                    _queryService.ExportToCsv(dt, outputPath);
                }
                else
                {
                    Console.WriteLine("ERROR: Unsupported format: " + format);
                    Console.WriteLine("Supported formats: csv");
                    return 1;
                }

                Console.WriteLine("Exported " + dt.Rows.Count + " permissions to: " + outputPath);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Export failed");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private int AddCluster(CommandLineParser parser)
        {
            string name = parser.GetArgument("name");
            string host = parser.GetArgument("host");
            string database = parser.GetArgument("database");
            string username = parser.GetArgument("username");
            string password = parser.GetArgument("password");

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(database) || string.IsNullOrEmpty(username))
            {
                Console.WriteLine("ERROR: Missing required parameters");
                Console.WriteLine("Required: --name, --host, --database, --username");
                return 1;
            }

            int port = 5439;
            string portStr = parser.GetArgument("port");
            if (!string.IsNullOrEmpty(portStr))
            {
                if (!int.TryParse(portStr, out port))
                {
                    Console.WriteLine("ERROR: Invalid port number");
                    return 1;
                }
            }

            Cluster cluster = new Cluster
            {
                Name = name,
                Host = host,
                Port = port,
                Database = database,
                Username = username,
                Password = password,
                Region = "us-east-1"
            };

            string error;
            if (_clusterService.AddCluster(cluster, out error))
            {
                Console.WriteLine("Cluster added successfully: " + name);
                return 0;
            }
            else
            {
                Console.WriteLine("ERROR: Failed to add cluster");
                Console.WriteLine(error);
                return 1;
            }
        }

        private int TestConnection(CommandLineParser parser)
        {
            string clusterName = parser.GetArgument("cluster");
            if (string.IsNullOrEmpty(clusterName))
            {
                Console.WriteLine("ERROR: --cluster parameter is required");
                return 1;
            }

            var cluster = FindCluster(clusterName);
            if (cluster == null)
            {
                Console.WriteLine("ERROR: Cluster not found: " + clusterName);
                return 1;
            }

            Console.Write("Testing connection to: " + cluster.Name + "... ");

            string error;
            bool connected = _clusterService.TestConnection(cluster, out error);

            if (connected)
            {
                Console.WriteLine("SUCCESS");
                return 0;
            }
            else
            {
                Console.WriteLine("FAILED");
                Console.WriteLine("Error: " + error);
                return 1;
            }
        }

        private Cluster FindCluster(string name)
        {
            var clusters = _clusterService.GetAllClusters();
            return clusters.Find(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private void PrintTable(DataTable dt)
        {
            // Calculate column widths
            int[] widths = new int[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                widths[i] = dt.Columns[i].ColumnName.Length;
                foreach (DataRow row in dt.Rows)
                {
                    int length = row[i].ToString().Length;
                    if (length > widths[i])
                    {
                        widths[i] = length;
                    }
                }
                widths[i] = Math.Min(widths[i], 50); // Max width 50
            }

            // Print header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Console.Write(dt.Columns[i].ColumnName.PadRight(widths[i] + 2));
            }
            Console.WriteLine();

            // Print separator
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Console.Write(new string('-', widths[i] + 2));
            }
            Console.WriteLine();

            // Print rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = row[i].ToString();
                    if (value.Length > 50)
                    {
                        value = value.Substring(0, 47) + "...";
                    }
                    Console.Write(value.PadRight(widths[i] + 2));
                }
                Console.WriteLine();
            }
        }

        private void PrintCsv(DataTable dt)
        {
            // Header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) Console.Write(",");
                Console.Write("\"" + dt.Columns[i].ColumnName + "\"");
            }
            Console.WriteLine();

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) Console.Write(",");
                    Console.Write("\"" + row[i].ToString().Replace("\"", "\"\"") + "\"");
                }
                Console.WriteLine();
            }
        }

        private void PrintJson(DataTable dt)
        {
            Console.WriteLine("[");
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                Console.Write("  {");
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (c > 0) Console.Write(", ");
                    Console.Write("\"" + dt.Columns[c].ColumnName + "\": \"" +
                        dt.Rows[r][c].ToString().Replace("\"", "\\\"") + "\"");
                }
                Console.Write("}");
                if (r < dt.Rows.Count - 1) Console.WriteLine(",");
                else Console.WriteLine();
            }
            Console.WriteLine("]");
        }
    }
}

using System;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;

namespace RedshiftGuardianNET.DataAccess
{
    /// <summary>
    /// Database context for local SQL Server Compact database
    /// Manages cluster metadata and cached scan results
    /// </summary>
    public class DatabaseContext : IDisposable
    {
        private SqlCeConnection _connection;
        private bool _disposed = false;

        public DatabaseContext()
        {
            string connectionString = GetConnectionString();
            _connection = new SqlCeConnection(connectionString);
        }

        /// <summary>
        /// Gets the database connection (opens if not already open)
        /// </summary>
        public SqlCeConnection GetConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        /// <summary>
        /// Gets connection string from config with DataDirectory substitution
        /// </summary>
        private static string GetConnectionString()
        {
            string connectionString = null;
            if (ConfigurationManager.ConnectionStrings["LocalDB"] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to default
                connectionString = "Data Source=|DataDirectory|\\RedshiftGuardian.sdf;Persist Security Info=False;";
            }

            // Replace |DataDirectory| with actual path
            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(dataDirectory))
            {
                dataDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RedshiftGuardian"
                );

                // Ensure directory exists
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            }

            connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);

            return connectionString;
        }

        /// <summary>
        /// Initializes the database - creates if doesn't exist and runs schema
        /// </summary>
        public static void InitializeDatabase()
        {
            string connectionString = GetConnectionString();

            // Extract file path from connection string
            string dbFilePath = ExtractDatabasePath(connectionString);

            // Create database file if it doesn't exist
            if (!File.Exists(dbFilePath))
            {
                Console.WriteLine("Creating database: " + dbFilePath);

                using (SqlCeEngine engine = new SqlCeEngine(connectionString))
                {
                    engine.CreateDatabase();
                }

                Console.WriteLine("Database created successfully");

                // Run schema creation
                CreateSchema(connectionString);
            }
            else
            {
                Console.WriteLine("Database already exists: " + dbFilePath);
            }
        }

        private static string ExtractDatabasePath(string connectionString)
        {
            // Parse "Data Source=C:\path\to\file.sdf"
            string[] parts = connectionString.Split(';');
            foreach (string part in parts)
            {
                if (part.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    return part.Substring(part.IndexOf('=') + 1).Trim();
                }
            }
            throw new InvalidOperationException("Could not extract database path from connection string");
        }

        /// <summary>
        /// Creates database schema (tables, indexes)
        /// </summary>
        private static void CreateSchema(string connectionString)
        {
            Console.WriteLine("Creating database schema...");

            using (var context = new DatabaseContext())
            {
                var conn = context.GetConnection();

                // Clusters table
                ExecuteNonQuery(conn, @"
                    CREATE TABLE Clusters (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL UNIQUE,
                        Host NVARCHAR(255) NOT NULL,
                        Port INT NOT NULL DEFAULT 5439,
                        [Database] NVARCHAR(255) NOT NULL,
                        ClusterType NVARCHAR(50) NOT NULL DEFAULT 'Provisioned',
                        Region NVARCHAR(50) NOT NULL,
                        AwsProfile NVARCHAR(100) NULL,
                        UseIAM BIT NOT NULL DEFAULT 1,
                        LastScanTime DATETIME NULL,
                        LastScanStatus NVARCHAR(50) NULL,
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    )
                ");

                // Users table (cached from Redshift)
                ExecuteNonQuery(conn, @"
                    CREATE TABLE Users (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        ClusterId INT NOT NULL,
                        Username NVARCHAR(255) NOT NULL,
                        UserId INT NOT NULL,
                        IsSuperuser BIT NOT NULL,
                        CanCreateDB BIT NOT NULL,
                        CanCreateUser BIT NOT NULL,
                        ValidUntil DATETIME NULL,
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT FK_Users_Clusters FOREIGN KEY (ClusterId)
                            REFERENCES Clusters(Id) ON DELETE CASCADE
                    )
                ");

                // Permissions table (cached from Redshift)
                ExecuteNonQuery(conn, @"
                    CREATE TABLE Permissions (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        ClusterId INT NOT NULL,
                        Username NVARCHAR(255) NOT NULL,
                        SchemaName NVARCHAR(255) NULL,
                        TableName NVARCHAR(255) NULL,
                        PermissionType NVARCHAR(50) NOT NULL,
                        GrantedBy NVARCHAR(255) NULL,
                        GrantedAt DATETIME NULL,
                        CONSTRAINT FK_Permissions_Clusters FOREIGN KEY (ClusterId)
                            REFERENCES Clusters(Id) ON DELETE CASCADE
                    )
                ");

                // Create indexes for performance
                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_Users_ClusterId ON Users(ClusterId)
                ");

                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_Users_Username ON Users(Username)
                ");

                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_Permissions_ClusterId ON Permissions(ClusterId)
                ");

                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_Permissions_Username ON Permissions(Username)
                ");

                // QueryTemplates table (for SQL query library)
                ExecuteNonQuery(conn, @"
                    CREATE TABLE QueryTemplates (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL,
                        Category NVARCHAR(100) NOT NULL,
                        Description NVARCHAR(500) NULL,
                        SqlQuery NTEXT NOT NULL,
                        HasParameters BIT NOT NULL DEFAULT 0,
                        ParameterNames NVARCHAR(500) NULL,
                        IsBuiltIn BIT NOT NULL DEFAULT 0,
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                        ModifiedAt DATETIME NULL
                    )
                ");

                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_QueryTemplates_Category ON QueryTemplates(Category)
                ");

                ExecuteNonQuery(conn, @"
                    CREATE INDEX IX_QueryTemplates_IsBuiltIn ON QueryTemplates(IsBuiltIn)
                ");

                Console.WriteLine("Schema created successfully");
            }
        }

        private static void ExecuteNonQuery(SqlCeConnection conn, string sql)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if database exists and is valid
        /// </summary>
        public static bool DatabaseExists()
        {
            try
            {
                string connectionString = GetConnectionString();
                string dbFilePath = ExtractDatabasePath(connectionString);
                return File.Exists(dbFilePath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the database file (for testing/reset)
        /// </summary>
        public static void DeleteDatabase()
        {
            string connectionString = GetConnectionString();
            string dbFilePath = ExtractDatabasePath(connectionString);

            if (File.Exists(dbFilePath))
            {
                File.Delete(dbFilePath);
                Console.WriteLine("Database deleted: " + dbFilePath);
            }
        }

        /// <summary>
        /// Gets database file size in bytes
        /// </summary>
        public static long GetDatabaseSize()
        {
            string connectionString = GetConnectionString();
            string dbFilePath = ExtractDatabasePath(connectionString);

            if (File.Exists(dbFilePath))
            {
                FileInfo fileInfo = new FileInfo(dbFilePath);
                return fileInfo.Length;
            }

            return 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == System.Data.ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                        _connection.Dispose();
                        _connection = null;
                    }
                }

                _disposed = true;
            }
        }

        ~DatabaseContext()
        {
            Dispose(false);
        }
    }
}

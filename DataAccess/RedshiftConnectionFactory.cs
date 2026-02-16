using System;
using System.Configuration;
using System.Data.Odbc;

namespace RedshiftGuardianNET.DataAccess
{
    /// <summary>
    /// Factory for creating ODBC connections to Amazon Redshift
    /// Supports both IAM authentication and username/password
    /// </summary>
    public static class RedshiftConnectionFactory
    {
        /// <summary>
        /// Creates an ODBC connection to Redshift with IAM authentication
        /// </summary>
        /// <param name="host">Redshift endpoint (e.g., my-cluster.abc123.us-east-1.redshift.amazonaws.com)</param>
        /// <param name="port">Port number (default 5439)</param>
        /// <param name="database">Database name</param>
        /// <param name="region">AWS region (e.g., us-east-1)</param>
        /// <param name="awsProfile">AWS credentials profile name (default "default")</param>
        /// <returns>OdbcConnection configured for Redshift</returns>
        public static OdbcConnection CreateConnection(
            string host,
            int port,
            string database,
            string region,
            string awsProfile = "default")
        {
            // Get ODBC driver name from config (fallback to default)
            string driverName = ConfigurationManager.AppSettings["RedshiftOdbcDriver"]
                ?? "Amazon Redshift (x64)";

            // Build ODBC connection string with IAM authentication
            string connectionString = string.Format(
                "Driver={{{0}}};" +
                "Server={1};" +
                "Port={2};" +
                "Database={3};" +
                "Region={4};" +
                "IAM=1;" +                          // Enable IAM authentication
                "Profile={5};" +                    // AWS profile name from ~/.aws/credentials
                "SSL=1;" +                          // Force SSL
                "SSLMode=require;" +                // Require SSL connection
                "AutoCreate=0",                     // Don't auto-create database user
                driverName, host, port, database, region, awsProfile
            );

            return new OdbcConnection(connectionString);
        }

        /// <summary>
        /// Creates an ODBC connection to Redshift with username/password authentication
        /// </summary>
        /// <param name="host">Redshift endpoint</param>
        /// <param name="port">Port number</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Database username</param>
        /// <param name="password">Database password</param>
        /// <returns>OdbcConnection configured for Redshift</returns>
        public static OdbcConnection CreateConnectionWithPassword(
            string host,
            int port,
            string database,
            string username,
            string password)
        {
            string driverName = ConfigurationManager.AppSettings["RedshiftOdbcDriver"]
                ?? "Amazon Redshift (x64)";

            // Build ODBC connection string with username/password
            string connectionString = string.Format(
                "Driver={{{0}}};" +
                "Server={1};" +
                "Port={2};" +
                "Database={3};" +
                "UID={4};" +                        // Username
                "PWD={5};" +                        // Password
                "SSL=1;" +                          // Force SSL
                "SSLMode=require",                  // Require SSL connection
                driverName, host, port, database, username, password
            );

            return new OdbcConnection(connectionString);
        }

        /// <summary>
        /// Tests if a connection can be established successfully
        /// </summary>
        /// <param name="conn">The ODBC connection to test</param>
        /// <returns>True if connection successful, false otherwise</returns>
        public static bool TestConnection(OdbcConnection conn)
        {
            try
            {
                conn.Open();

                // Execute a simple query to verify connection
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT 1";
                    var result = cmd.ExecuteScalar();

                    if (result == null || Convert.ToInt32(result) != 1)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (OdbcException ex)
            {
                // Log ODBC-specific errors
                Console.WriteLine("ODBC Error: " + ex.Message);
                foreach (OdbcError error in ex.Errors)
                {
                    Console.WriteLine("  SQLState: {0}, NativeError: {1}, Message: {2}",
                        error.SQLState, error.NativeError, error.Message);
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection test failed: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Gets connection string for debugging (with credentials masked)
        /// </summary>
        public static string GetMaskedConnectionString(OdbcConnection conn)
        {
            string connStr = conn.ConnectionString;

            // Mask password
            connStr = System.Text.RegularExpressions.Regex.Replace(
                connStr, @"PWD=[^;]+", "PWD=****");

            return connStr;
        }
    }
}

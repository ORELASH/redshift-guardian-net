using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.DataAccess;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Service for scanning Redshift clusters and caching results
    /// Fetches permissions from Redshift and saves to local database
    /// </summary>
    public class ScannerService : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly ClusterRepository _clusterRepository;
        private bool _disposed = false;

        public ScannerService()
        {
            _context = new DatabaseContext();
            _clusterRepository = new ClusterRepository(_context);
        }

        /// <summary>
        /// Scans a cluster and saves results to local database
        /// </summary>
        public ScanResult ScanCluster(Cluster cluster)
        {
            var result = new ScanResult
            {
                ClusterId = cluster.Id,
                ClusterName = cluster.Name,
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("Starting scan of cluster: " + cluster.Name);

                // Create Redshift repository for ODBC queries
                var redshiftRepo = new RedshiftRepository(cluster);

                // Fetch users
                Console.WriteLine("Fetching users...");
                result.Users = redshiftRepo.GetAllUsers();
                result.UserCount = result.Users.Count;
                Console.WriteLine("Found {0} users", result.UserCount);

                // Fetch table permissions
                Console.WriteLine("Fetching table permissions...");
                result.TablePermissions = redshiftRepo.GetTablePermissions();
                result.PermissionCount = result.TablePermissions.Count;
                Console.WriteLine("Found {0} permissions", result.PermissionCount);

                // Fetch role lineage
                Console.WriteLine("Fetching role lineage...");
                result.RoleLineages = redshiftRepo.GetRoleLineage();
                Console.WriteLine("Found {0} role grants", result.RoleLineages.Count);

                result.Success = true;
                result.Message = string.Format(
                    "Scan completed successfully: {0} users, {1} permissions",
                    result.UserCount,
                    result.PermissionCount
                );
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Scan failed: " + ex.Message;
                result.Exception = ex;

                Console.WriteLine("Scan failed: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
            finally
            {
                result.EndTime = DateTime.Now;
                result.Duration = result.EndTime - result.StartTime;

                Console.WriteLine("Scan duration: " + result.Duration.TotalSeconds + " seconds");
            }

            // Save scan results to local database
            if (result.Success)
            {
                SaveScanResults(result);
            }

            // Update cluster scan status
            _clusterRepository.UpdateScanStatus(
                cluster.Id,
                result.EndTime,
                result.Success ? "Success" : "Failed"
            );

            return result;
        }

        /// <summary>
        /// Saves scan results to local database
        /// Clears old data and inserts new data
        /// </summary>
        private void SaveScanResults(ScanResult result)
        {
            Console.WriteLine("Saving scan results to local database...");

            var conn = _context.GetConnection();

            // Delete old data for this cluster
            DeleteOldData(conn, result.ClusterId);

            // Insert users
            InsertUsers(conn, result);

            // Insert permissions
            InsertPermissions(conn, result);

            Console.WriteLine("Scan results saved successfully");
        }

        private void DeleteOldData(SqlCeConnection conn, int clusterId)
        {
            // Delete old users
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Users WHERE ClusterId = @clusterId";
                cmd.Parameters.AddWithValue("@clusterId", clusterId);
                int usersDeleted = cmd.ExecuteNonQuery();
                Console.WriteLine("Deleted {0} old users", usersDeleted);
            }

            // Delete old permissions
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Permissions WHERE ClusterId = @clusterId";
                cmd.Parameters.AddWithValue("@clusterId", clusterId);
                int permsDeleted = cmd.ExecuteNonQuery();
                Console.WriteLine("Deleted {0} old permissions", permsDeleted);
            }
        }

        private void InsertUsers(SqlCeConnection conn, ScanResult result)
        {
            Console.WriteLine("Inserting {0} users...", result.Users.Count);

            foreach (var user in result.Users)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Users (ClusterId, Username, UserId, IsSuperuser,
                                          CanCreateDB, CanCreateUser, ValidUntil, CreatedAt)
                        VALUES (@clusterId, @username, @userId, @isSuperuser,
                               @canCreateDB, @canCreateUser, @validUntil, @createdAt)
                    ";

                    cmd.Parameters.AddWithValue("@clusterId", result.ClusterId);
                    cmd.Parameters.AddWithValue("@username", user.Username ?? "");
                    cmd.Parameters.AddWithValue("@userId", user.UserId);
                    cmd.Parameters.AddWithValue("@isSuperuser", user.IsSuperuser);
                    cmd.Parameters.AddWithValue("@canCreateDB", user.CanCreateDB);
                    cmd.Parameters.AddWithValue("@canCreateUser", user.CanCreateUser);
                    cmd.Parameters.AddWithValue("@validUntil",
                        user.ValidUntil.HasValue ? (object)user.ValidUntil.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Users inserted successfully");
        }

        private void InsertPermissions(SqlCeConnection conn, ScanResult result)
        {
            Console.WriteLine("Inserting {0} permissions...", result.TablePermissions.Count);

            foreach (var perm in result.TablePermissions)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Permissions (ClusterId, Username, SchemaName, TableName,
                                                PermissionType, GrantedAt)
                        VALUES (@clusterId, @username, @schemaName, @tableName,
                               @permissionType, @grantedAt)
                    ";

                    cmd.Parameters.AddWithValue("@clusterId", result.ClusterId);
                    cmd.Parameters.AddWithValue("@username", perm.Username ?? "");
                    cmd.Parameters.AddWithValue("@schemaName", perm.SchemaName ?? "");
                    cmd.Parameters.AddWithValue("@tableName", perm.TableName ?? "");
                    cmd.Parameters.AddWithValue("@permissionType", perm.PermissionType ?? "");
                    cmd.Parameters.AddWithValue("@grantedAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Permissions inserted successfully");
        }

        /// <summary>
        /// Gets cached users for a cluster
        /// </summary>
        public List<RedshiftUser> GetCachedUsers(int clusterId)
        {
            var users = new List<RedshiftUser>();

            var conn = _context.GetConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Username, UserId, IsSuperuser, CanCreateDB, CanCreateUser, ValidUntil
                    FROM Users
                    WHERE ClusterId = @clusterId
                    ORDER BY Username
                ";

                cmd.Parameters.AddWithValue("@clusterId", clusterId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new RedshiftUser
                        {
                            Username = reader["Username"].ToString(),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            IsSuperuser = Convert.ToBoolean(reader["IsSuperuser"]),
                            CanCreateDB = Convert.ToBoolean(reader["CanCreateDB"]),
                            CanCreateUser = Convert.ToBoolean(reader["CanCreateUser"]),
                            ValidUntil = reader["ValidUntil"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["ValidUntil"])
                        });
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Gets cached permissions for a cluster
        /// </summary>
        public List<TablePermission> GetCachedPermissions(int clusterId)
        {
            var permissions = new List<TablePermission>();

            var conn = _context.GetConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Username, SchemaName, TableName, PermissionType
                    FROM Permissions
                    WHERE ClusterId = @clusterId
                    ORDER BY Username, SchemaName, TableName
                ";

                cmd.Parameters.AddWithValue("@clusterId", clusterId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new TablePermission
                        {
                            Username = reader["Username"].ToString(),
                            SchemaName = reader["SchemaName"].ToString(),
                            TableName = reader["TableName"].ToString(),
                            PermissionType = reader["PermissionType"].ToString()
                        });
                    }
                }
            }

            return permissions;
        }

        /// <summary>
        /// Gets count of cached users for a cluster
        /// </summary>
        public int GetCachedUserCount(int clusterId)
        {
            var conn = _context.GetConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE ClusterId = @clusterId";
                cmd.Parameters.AddWithValue("@clusterId", clusterId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Gets count of cached permissions for a cluster
        /// </summary>
        public int GetCachedPermissionCount(int clusterId)
        {
            var conn = _context.GetConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Permissions WHERE ClusterId = @clusterId";
                cmd.Parameters.AddWithValue("@clusterId", clusterId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
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
                    if (_context != null)
                    {
                        _context.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~ScannerService()
        {
            Dispose(false);
        }
    }
}

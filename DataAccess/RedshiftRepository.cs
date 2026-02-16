using System;
using System.Collections.Generic;
using System.Data.Odbc;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.DataAccess
{
    /// <summary>
    /// Repository for querying Redshift permissions via ODBC
    /// Executes SQL queries against Redshift system tables
    /// </summary>
    public class RedshiftRepository
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _database;
        private readonly string _region;
        private readonly string _awsProfile;
        private readonly bool _useIAM;

        public RedshiftRepository(Cluster cluster)
        {
            _host = cluster.Host;
            _port = cluster.Port;
            _database = cluster.Database;
            _region = cluster.Region;
            _awsProfile = cluster.AwsProfile ?? "default";
            _useIAM = cluster.UseIAM;
        }

        public RedshiftRepository(string host, int port, string database,
            string region, string awsProfile = "default", bool useIAM = true)
        {
            _host = host;
            _port = port;
            _database = database;
            _region = region;
            _awsProfile = awsProfile;
            _useIAM = useIAM;
        }

        /// <summary>
        /// Gets all users from Redshift
        /// </summary>
        public List<RedshiftUser> GetAllUsers()
        {
            var users = new List<RedshiftUser>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT
                        usename AS username,
                        usesysid AS user_id,
                        usecreatedb AS can_create_db,
                        usesuper AS is_superuser,
                        usecatupd AS can_create_user,
                        valuntil AS valid_until
                    FROM pg_user
                    ORDER BY usename;
                ";

                using (var cmd = new OdbcCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new RedshiftUser
                        {
                            Username = reader["username"].ToString(),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            CanCreateDB = Convert.ToBoolean(reader["can_create_db"]),
                            IsSuperuser = Convert.ToBoolean(reader["is_superuser"]),
                            CanCreateUser = Convert.ToBoolean(reader["can_create_user"]),
                            ValidUntil = reader["valid_until"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["valid_until"])
                        });
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Gets table permissions for all users
        /// </summary>
        public List<TablePermission> GetTablePermissions()
        {
            var permissions = new List<TablePermission>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                // Query for SELECT permissions
                AddPermissionsForType(conn, permissions, "SELECT");

                // Query for INSERT permissions
                AddPermissionsForType(conn, permissions, "INSERT");

                // Query for UPDATE permissions
                AddPermissionsForType(conn, permissions, "UPDATE");

                // Query for DELETE permissions
                AddPermissionsForType(conn, permissions, "DELETE");
            }

            return permissions;
        }

        private void AddPermissionsForType(OdbcConnection conn, List<TablePermission> permissions, string permType)
        {
            string sql = string.Format(@"
                SELECT DISTINCT
                    u.usename AS username,
                    n.nspname AS schema_name,
                    c.relname AS table_name,
                    '{0}' AS permission_type
                FROM pg_user u
                CROSS JOIN pg_class c
                JOIN pg_namespace n ON c.relnamespace = n.oid
                WHERE c.relkind = 'r'
                  AND n.nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')
                  AND has_table_privilege(u.usename, c.oid, '{0}')
                ORDER BY username, schema_name, table_name
            ", permType);

            using (var cmd = new OdbcCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    permissions.Add(new TablePermission
                    {
                        Username = reader["username"].ToString(),
                        SchemaName = reader["schema_name"].ToString(),
                        TableName = reader["table_name"].ToString(),
                        PermissionType = reader["permission_type"].ToString()
                    });
                }
            }
        }

        /// <summary>
        /// Gets role lineage (who granted roles to whom)
        /// </summary>
        public List<RoleLineage> GetRoleLineage()
        {
            var lineages = new List<RoleLineage>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT
                        granted.usename AS granted_role,
                        grantor.usename AS grantor,
                        m.admin_option
                    FROM pg_auth_members m
                    JOIN pg_user granted ON m.member = granted.usesysid
                    JOIN pg_user grantor ON m.grantor = grantor.usesysid
                    ORDER BY granted.usename, grantor.usename;
                ";

                using (var cmd = new OdbcCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lineages.Add(new RoleLineage
                        {
                            GrantedRole = reader["granted_role"].ToString(),
                            Grantor = reader["grantor"].ToString(),
                            AdminOption = Convert.ToBoolean(reader["admin_option"])
                        });
                    }
                }
            }

            return lineages;
        }

        /// <summary>
        /// Gets schema permissions
        /// </summary>
        public List<string> GetSchemaPermissions(string username)
        {
            var schemas = new List<string>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT DISTINCT nspname
                    FROM pg_namespace
                    WHERE has_schema_privilege(?, nspname, 'USAGE')
                      AND nspname NOT IN ('pg_catalog', 'information_schema', 'pg_toast')
                    ORDER BY nspname;
                ";

                using (var cmd = new OdbcCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schemas.Add(reader["nspname"].ToString());
                        }
                    }
                }
            }

            return schemas;
        }

        /// <summary>
        /// Tests if connection can be established
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    return RedshiftConnectionFactory.TestConnection(conn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test connection failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Executes a custom SQL query and returns raw results
        /// </summary>
        public List<Dictionary<string, object>> ExecuteQuery(string sql)
        {
            var results = new List<Dictionary<string, object>>();

            using (var conn = CreateConnection())
            {
                conn.Open();

                using (var cmd = new OdbcCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.GetValue(i);
                            row[columnName] = value == DBNull.Value ? null : value;
                        }

                        results.Add(row);
                    }
                }
            }

            return results;
        }

        private OdbcConnection CreateConnection()
        {
            if (_useIAM)
            {
                return RedshiftConnectionFactory.CreateConnection(
                    _host, _port, _database, _region, _awsProfile);
            }
            else
            {
                // For username/password, you'd need to add username/password fields to Cluster model
                throw new NotImplementedException("Username/password authentication not yet implemented in UI");
            }
        }
    }
}

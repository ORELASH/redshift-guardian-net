using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Service for managing permissions (GRANT/REVOKE)
    /// </summary>
    public class PermissionManagementService : IDisposable
    {
        private RedshiftOdbcService _odbcService;
        private bool _readOnlyMode;

        public PermissionManagementService(RedshiftOdbcService odbcService, bool readOnlyMode = false)
        {
            _odbcService = odbcService;
            _readOnlyMode = readOnlyMode;
        }

        #region Grant Operations

        /// <summary>
        /// Grant table permissions to user
        /// </summary>
        public PermissionOperationResult GrantTablePermission(
            string schemaName,
            string tableName,
            string username,
            string[] permissions,
            bool withGrantOption = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, tableName, username, permissions);

                string permissionList = string.Join(", ", permissions);
                string grantOption = withGrantOption ? " WITH GRANT OPTION" : "";

                string sql = string.Format(
                    "GRANT {0} ON {1}.{2} TO {3}{4}",
                    permissionList,
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(tableName),
                    QuoteIdentifier(username),
                    grantOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Granted {0} on {1}.{2} to {3}",
                        permissionList, schemaName, tableName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Grant operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Grant schema permissions to user
        /// </summary>
        public PermissionOperationResult GrantSchemaPermission(
            string schemaName,
            string username,
            string[] permissions,
            bool withGrantOption = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, null, username, permissions);

                string permissionList = string.Join(", ", permissions);
                string grantOption = withGrantOption ? " WITH GRANT OPTION" : "";

                string sql = string.Format(
                    "GRANT {0} ON SCHEMA {1} TO {2}{3}",
                    permissionList,
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(username),
                    grantOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Granted {0} on schema {1} to {2}",
                        permissionList, schemaName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Grant schema operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Grant all tables in schema to user
        /// </summary>
        public PermissionOperationResult GrantAllTablesInSchema(
            string schemaName,
            string username,
            string[] permissions,
            bool withGrantOption = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, null, username, permissions);

                string permissionList = string.Join(", ", permissions);
                string grantOption = withGrantOption ? " WITH GRANT OPTION" : "";

                string sql = string.Format(
                    "GRANT {0} ON ALL TABLES IN SCHEMA {1} TO {2}{3}",
                    permissionList,
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(username),
                    grantOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Granted {0} on all tables in schema {1} to {2}",
                        permissionList, schemaName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Grant all tables operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        #endregion

        #region Revoke Operations

        /// <summary>
        /// Revoke table permissions from user
        /// </summary>
        public PermissionOperationResult RevokeTablePermission(
            string schemaName,
            string tableName,
            string username,
            string[] permissions,
            bool cascade = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, tableName, username, permissions);

                string permissionList = string.Join(", ", permissions);
                string cascadeOption = cascade ? " CASCADE" : "";

                string sql = string.Format(
                    "REVOKE {0} ON {1}.{2} FROM {3}{4}",
                    permissionList,
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(tableName),
                    QuoteIdentifier(username),
                    cascadeOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Revoked {0} on {1}.{2} from {3}",
                        permissionList, schemaName, tableName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Revoke operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Revoke schema permissions from user
        /// </summary>
        public PermissionOperationResult RevokeSchemaPermission(
            string schemaName,
            string username,
            string[] permissions,
            bool cascade = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, null, username, permissions);

                string permissionList = string.Join(", ", permissions);
                string cascadeOption = cascade ? " CASCADE" : "";

                string sql = string.Format(
                    "REVOKE {0} ON SCHEMA {1} FROM {2}{3}",
                    permissionList,
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(username),
                    cascadeOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Revoked {0} on schema {1} from {2}",
                        permissionList, schemaName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Revoke schema operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Revoke all permissions on table from user
        /// </summary>
        public PermissionOperationResult RevokeAllPermissions(
            string schemaName,
            string tableName,
            string username,
            bool cascade = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                ValidateInputs(schemaName, tableName, username, null);

                string cascadeOption = cascade ? " CASCADE" : "";

                string sql = string.Format(
                    "REVOKE ALL ON {0}.{1} FROM {2}{3}",
                    QuoteIdentifier(schemaName),
                    QuoteIdentifier(tableName),
                    QuoteIdentifier(username),
                    cascadeOption
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Revoked all permissions on {0}.{1} from {2}",
                        schemaName, tableName, username),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Revoke all operation failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        #endregion

        #region User Management

        /// <summary>
        /// Create new user
        /// </summary>
        public PermissionOperationResult CreateUser(
            string username,
            string password,
            int connectionLimit = -1,
            DateTime? validUntil = null,
            bool createDb = false,
            bool createUser = false)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Password cannot be empty");
                }

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("CREATE USER {0} WITH PASSWORD '{1}'",
                    QuoteIdentifier(username),
                    password.Replace("'", "''"));

                if (connectionLimit >= 0)
                {
                    sql.AppendFormat(" CONNECTION LIMIT {0}", connectionLimit);
                }

                if (validUntil.HasValue)
                {
                    sql.AppendFormat(" VALID UNTIL '{0}'",
                        validUntil.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (createDb)
                {
                    sql.Append(" CREATEDB");
                }

                if (createUser)
                {
                    sql.Append(" CREATEUSER");
                }

                ExecuteCommand(sql.ToString());

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = "User created successfully: " + username,
                    SqlExecuted = sql.ToString().Replace(password, "***")
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Create user failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Alter user password
        /// </summary>
        public PermissionOperationResult AlterUserPassword(string username, string newPassword)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    throw new ArgumentException("Password cannot be empty");
                }

                string sql = string.Format(
                    "ALTER USER {0} WITH PASSWORD '{1}'",
                    QuoteIdentifier(username),
                    newPassword.Replace("'", "''")
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = "Password changed successfully for user: " + username,
                    SqlExecuted = sql.Replace(newPassword, "***")
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Alter user password failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Drop user
        /// </summary>
        public PermissionOperationResult DropUser(string username)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }

                string sql = string.Format("DROP USER {0}", QuoteIdentifier(username));
                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = "User dropped successfully: " + username,
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Drop user failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        #endregion

        #region Group/Role Management

        /// <summary>
        /// Create group
        /// </summary>
        public PermissionOperationResult CreateGroup(string groupName)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    throw new ArgumentException("Group name cannot be empty");
                }

                string sql = string.Format("CREATE GROUP {0}", QuoteIdentifier(groupName));
                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = "Group created successfully: " + groupName,
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Create group failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Add user to group
        /// </summary>
        public PermissionOperationResult AddUserToGroup(string username, string groupName)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(groupName))
                {
                    throw new ArgumentException("Username and group name cannot be empty");
                }

                string sql = string.Format(
                    "ALTER GROUP {0} ADD USER {1}",
                    QuoteIdentifier(groupName),
                    QuoteIdentifier(username)
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Added user {0} to group {1}", username, groupName),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Add user to group failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Remove user from group
        /// </summary>
        public PermissionOperationResult RemoveUserFromGroup(string username, string groupName)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(groupName))
                {
                    throw new ArgumentException("Username and group name cannot be empty");
                }

                string sql = string.Format(
                    "ALTER GROUP {0} DROP USER {1}",
                    QuoteIdentifier(groupName),
                    QuoteIdentifier(username)
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Removed user {0} from group {1}", username, groupName),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Remove user from group failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        /// <summary>
        /// Drop group
        /// </summary>
        public PermissionOperationResult DropGroup(string groupName)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    throw new ArgumentException("Group name cannot be empty");
                }

                string sql = string.Format("DROP GROUP {0}", QuoteIdentifier(groupName));
                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = "Group dropped successfully: " + groupName,
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Drop group failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        #endregion

        #region Default Privileges

        /// <summary>
        /// Alter default privileges for future tables
        /// </summary>
        public PermissionOperationResult AlterDefaultPrivileges(
            string schemaName,
            string targetUser,
            string[] permissions,
            string forUser = null)
        {
            if (_readOnlyMode)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Service is in read-only mode"
                };
            }

            try
            {
                if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(targetUser))
                {
                    throw new ArgumentException("Schema name and target user cannot be empty");
                }

                ValidatePermissions(permissions);

                string permissionList = string.Join(", ", permissions);
                string forUserClause = string.IsNullOrEmpty(forUser) ? "" :
                    " FOR USER " + QuoteIdentifier(forUser);

                string sql = string.Format(
                    "ALTER DEFAULT PRIVILEGES{0} IN SCHEMA {1} GRANT {2} ON TABLES TO {3}",
                    forUserClause,
                    QuoteIdentifier(schemaName),
                    permissionList,
                    QuoteIdentifier(targetUser)
                );

                ExecuteCommand(sql);

                return new PermissionOperationResult
                {
                    Success = true,
                    Message = string.Format("Set default privileges in schema {0} for {1}",
                        schemaName, targetUser),
                    SqlExecuted = sql
                };
            }
            catch (Exception ex)
            {
                return new PermissionOperationResult
                {
                    Success = false,
                    Message = "Alter default privileges failed: " + ex.Message,
                    Error = ex
                };
            }
        }

        #endregion

        #region Helper Methods

        private void ExecuteCommand(string sql)
        {
            using (OdbcCommand command = new OdbcCommand(sql, _odbcService.GetConnection()))
            {
                command.ExecuteNonQuery();
            }
        }

        private void ValidateInputs(string schemaName, string tableName, string username, string[] permissions)
        {
            if (string.IsNullOrEmpty(schemaName))
            {
                throw new ArgumentException("Schema name cannot be empty");
            }

            if (!string.IsNullOrEmpty(tableName) && string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name cannot be empty");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be empty");
            }

            if (permissions != null && permissions.Length > 0)
            {
                ValidatePermissions(permissions);
            }
        }

        private void ValidatePermissions(string[] permissions)
        {
            string[] validPermissions = new string[]
            {
                "SELECT", "INSERT", "UPDATE", "DELETE", "REFERENCES",
                "USAGE", "CREATE", "TEMPORARY", "TEMP", "ALL", "ALL PRIVILEGES"
            };

            foreach (string perm in permissions)
            {
                bool found = false;
                foreach (string valid in validPermissions)
                {
                    if (perm.ToUpper() == valid)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ArgumentException("Invalid permission: " + perm);
                }
            }
        }

        private string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return identifier;
            }

            // Escape double quotes
            string escaped = identifier.Replace("\"", "\"\"");

            // Quote identifier
            return "\"" + escaped + "\"";
        }

        #endregion

        public void Dispose()
        {
            // ODBC service is managed externally, don't dispose
        }
    }

    /// <summary>
    /// Result of permission operation
    /// </summary>
    public class PermissionOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string SqlExecuted { get; set; }
        public Exception Error { get; set; }
    }
}

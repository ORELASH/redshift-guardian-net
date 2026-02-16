using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using RedshiftGuardianNET.DataAccess;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Service for managing query templates library
    /// </summary>
    public class QueryLibraryService
    {
        private readonly DatabaseContext _dbContext;

        public QueryLibraryService()
        {
            _dbContext = new DatabaseContext();
            InitializeBuiltInQueries();
        }

        /// <summary>
        /// Get all query templates
        /// </summary>
        public List<QueryTemplate> GetAllQueries()
        {
            List<QueryTemplate> queries = new List<QueryTemplate>();

            string sql = @"
                SELECT Id, Name, Category, Description, SqlQuery,
                       HasParameters, ParameterNames, IsBuiltIn,
                       CreatedAt, ModifiedAt
                FROM QueryTemplates
                ORDER BY Category, Name";

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            using (SqlCeDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    queries.Add(new QueryTemplate
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Category = reader.GetString(2),
                        Description = reader.GetString(3),
                        SqlQuery = reader.GetString(4),
                        HasParameters = reader.GetBoolean(5),
                        ParameterNames = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsBuiltIn = reader.GetBoolean(7),
                        CreatedAt = reader.GetDateTime(8),
                        ModifiedAt = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                    });
                }
            }

            return queries;
        }

        /// <summary>
        /// Get queries by category
        /// </summary>
        public List<QueryTemplate> GetQueriesByCategory(string category)
        {
            return GetAllQueries().Where(q => q.Category == category).ToList();
        }

        /// <summary>
        /// Get query template by ID
        /// </summary>
        public QueryTemplate GetQueryById(int id)
        {
            string sql = @"
                SELECT Id, Name, Category, Description, SqlQuery,
                       HasParameters, ParameterNames, IsBuiltIn,
                       CreatedAt, ModifiedAt
                FROM QueryTemplates
                WHERE Id = @id";

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new QueryTemplate
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            Description = reader.GetString(3),
                            SqlQuery = reader.GetString(4),
                            HasParameters = reader.GetBoolean(5),
                            ParameterNames = reader.IsDBNull(6) ? null : reader.GetString(6),
                            IsBuiltIn = reader.GetBoolean(7),
                            CreatedAt = reader.GetDateTime(8),
                            ModifiedAt = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Save query template (insert or update)
        /// </summary>
        public void SaveQuery(QueryTemplate query)
        {
            if (query.Id == 0)
            {
                InsertQuery(query);
            }
            else
            {
                UpdateQuery(query);
            }
        }

        private void InsertQuery(QueryTemplate query)
        {
            string sql = @"
                INSERT INTO QueryTemplates
                (Name, Category, Description, SqlQuery, HasParameters, ParameterNames, IsBuiltIn, CreatedAt)
                VALUES (@name, @category, @description, @sqlQuery, @hasParams, @paramNames, @isBuiltIn, @createdAt)";

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", query.Name);
                cmd.Parameters.AddWithValue("@category", query.Category);
                cmd.Parameters.AddWithValue("@description", query.Description);
                cmd.Parameters.AddWithValue("@sqlQuery", query.SqlQuery);
                cmd.Parameters.AddWithValue("@hasParams", query.HasParameters);
                cmd.Parameters.AddWithValue("@paramNames", (object)query.ParameterNames ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@isBuiltIn", query.IsBuiltIn);
                cmd.Parameters.AddWithValue("@createdAt", query.CreatedAt);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateQuery(QueryTemplate query)
        {
            string sql = @"
                UPDATE QueryTemplates
                SET Name = @name,
                    Category = @category,
                    Description = @description,
                    SqlQuery = @sqlQuery,
                    HasParameters = @hasParams,
                    ParameterNames = @paramNames,
                    ModifiedAt = @modifiedAt
                WHERE Id = @id AND IsBuiltIn = 0";  // Don't update built-in queries

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", query.Name);
                cmd.Parameters.AddWithValue("@category", query.Category);
                cmd.Parameters.AddWithValue("@description", query.Description);
                cmd.Parameters.AddWithValue("@sqlQuery", query.SqlQuery);
                cmd.Parameters.AddWithValue("@hasParams", query.HasParameters);
                cmd.Parameters.AddWithValue("@paramNames", (object)query.ParameterNames ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@modifiedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", query.Id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete query template (only custom queries)
        /// </summary>
        public void DeleteQuery(int id)
        {
            string sql = "DELETE FROM QueryTemplates WHERE Id = @id AND IsBuiltIn = 0";

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        public List<string> GetCategories()
        {
            List<string> categories = new List<string>();

            string sql = "SELECT DISTINCT Category FROM QueryTemplates ORDER BY Category";

            using (SqlCeConnection conn = _dbContext.GetConnection())
            using (SqlCeCommand cmd = new SqlCeCommand(sql, conn))
            using (SqlCeDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(reader.GetString(0));
                }
            }

            return categories;
        }

        /// <summary>
        /// Initialize built-in queries from the library
        /// </summary>
        private void InitializeBuiltInQueries()
        {
            // Check if built-in queries already exist
            int count = 0;
            using (SqlCeConnection conn = _dbContext.GetConnection())
            {
                string countSql = "SELECT COUNT(*) FROM QueryTemplates WHERE IsBuiltIn = 1";
                using (SqlCeCommand cmd = new SqlCeCommand(countSql, conn))
                {
                    count = (int)cmd.ExecuteScalar();
                }
            }

            if (count > 0)
            {
                return;  // Already initialized
            }

            // Insert built-in queries
            List<QueryTemplate> builtInQueries = GetBuiltInQueryTemplates();
            foreach (var query in builtInQueries)
            {
                InsertQuery(query);
            }
        }

        /// <summary>
        /// Get the list of built-in query templates
        /// </summary>
        private List<QueryTemplate> GetBuiltInQueryTemplates()
        {
            return new List<QueryTemplate>
            {
                // Category: Users
                new QueryTemplate
                {
                    Name = "All Users",
                    Category = "Users",
                    Description = "Get all database users with their attributes",
                    SqlQuery = "SELECT usename, usesysid, usesuper FROM pg_user",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Last Login (Provisioned)",
                    Category = "Users",
                    Description = "Get last login time for each user (Provisioned clusters)",
                    SqlQuery = @"SELECT username, MAX(recordtime) as last_login
FROM stl_connection_log
WHERE event = 'authenticated'
GROUP BY username",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Last Login (Serverless)",
                    Category = "Users",
                    Description = "Get last login time for each user (Serverless clusters)",
                    SqlQuery = @"SELECT user_name, MAX(start_time) as last_login
FROM sys_connection_log
WHERE action = 'connect'
GROUP BY user_name",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Unused Users",
                    Category = "Users",
                    Description = "Find users who haven't logged in for 90+ days",
                    SqlQuery = @"SELECT u.usename, u.usesysid, u.usesuper, MAX(c.recordtime) as last_login
FROM pg_user u
LEFT JOIN stl_connection_log c ON u.usename = c.username AND c.event = 'authenticated'
GROUP BY u.usename, u.usesysid, u.usesuper
HAVING MAX(c.recordtime) < DATEADD(day, -90, GETDATE()) OR MAX(c.recordtime) IS NULL
ORDER BY last_login",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Superusers",
                    Category = "Users",
                    Description = "List all superusers with activity",
                    SqlQuery = @"SELECT u.usename, u.usesysid, u.usecreatedb, MAX(c.recordtime) as last_login,
       COUNT(DISTINCT q.query) as query_count_last_7days
FROM pg_user u
LEFT JOIN stl_connection_log c ON u.usename = c.username AND c.event = 'authenticated'
LEFT JOIN stl_query q ON u.usesysid = q.userid AND q.starttime > DATEADD(day, -7, GETDATE())
WHERE u.usesuper = true
GROUP BY u.usename, u.usesysid, u.usecreatedb
ORDER BY last_login DESC",
                    HasParameters = false,
                    IsBuiltIn = true
                },

                // Category: Roles
                new QueryTemplate
                {
                    Name = "All Roles",
                    Category = "Roles",
                    Description = "Get all database roles and their owners",
                    SqlQuery = "SELECT role_name, role_owner FROM svv_roles",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Role Lineage",
                    Category = "Roles",
                    Description = "Build complete role inheritance tree",
                    SqlQuery = @"WITH RECURSIVE role_hierarchy AS (
    SELECT member_name, member_type, role_name AS parent_role, 1 AS depth
    FROM svv_role_grants
    UNION ALL
    SELECT rh.member_name, rh.member_type, rg.role_name, rh.depth + 1
    FROM role_hierarchy rh
    JOIN svv_role_grants rg ON rh.parent_role = rg.member_name
    WHERE rh.depth < 10
)
SELECT member_name, member_type, parent_role, depth
FROM role_hierarchy
ORDER BY depth, member_name",
                    HasParameters = false,
                    IsBuiltIn = true
                },

                // Category: Permissions
                new QueryTemplate
                {
                    Name = "All Table Permissions",
                    Category = "Permissions",
                    Description = "Get all table-level permissions (grants)",
                    SqlQuery = @"SELECT grantor, grantee, table_schema, table_name, privilege_type, is_grantable
FROM information_schema.table_privileges",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Effective Permissions for User",
                    Category = "Permissions",
                    Description = "Get all effective permissions for a specific user (direct + via roles)",
                    SqlQuery = @"WITH user_roles AS (
    WITH RECURSIVE role_tree AS (
        SELECT role_name, 1 as depth
        FROM svv_role_grants
        WHERE member_name = ?
        UNION ALL
        SELECT rg.role_name, rt.depth + 1
        FROM role_tree rt
        JOIN svv_role_grants rg ON rt.role_name = rg.member_name
        WHERE rt.depth < 10
    )
    SELECT DISTINCT role_name FROM role_tree
)
SELECT DISTINCT
    tp.table_schema,
    tp.table_name,
    tp.privilege_type,
    CASE
        WHEN tp.grantee = ? THEN 'Direct'
        ELSE 'Via Role: ' || tp.grantee
    END as grant_type
FROM information_schema.table_privileges tp
WHERE tp.grantee = ? OR tp.grantee IN (SELECT role_name FROM user_roles)
ORDER BY tp.table_schema, tp.table_name, tp.privilege_type",
                    HasParameters = true,
                    ParameterNames = "username,username,username",
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Schema Permissions",
                    Category = "Permissions",
                    Description = "Get schema-level permissions",
                    SqlQuery = @"SELECT schema_name, schema_owner, grantee, privilege_type
FROM information_schema.schema_privileges",
                    HasParameters = false,
                    IsBuiltIn = true
                },

                // Category: Tables
                new QueryTemplate
                {
                    Name = "All Tables and Views",
                    Category = "Tables",
                    Description = "Get all tables and views in database",
                    SqlQuery = @"SELECT table_schema, table_name, table_type
FROM information_schema.tables
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
ORDER BY table_schema, table_name",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Table Columns",
                    Category = "Tables",
                    Description = "Get all columns for all tables",
                    SqlQuery = @"SELECT table_schema, table_name, column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
ORDER BY table_schema, table_name, ordinal_position",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Columns for Specific Table",
                    Category = "Tables",
                    Description = "Get columns for a specific table",
                    SqlQuery = @"SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = ?
ORDER BY ordinal_position",
                    HasParameters = true,
                    ParameterNames = "table_name",
                    IsBuiltIn = true
                },

                // Category: Audit
                new QueryTemplate
                {
                    Name = "Orphaned Permissions",
                    Category = "Audit",
                    Description = "Find permissions granted to non-existent users/roles",
                    SqlQuery = @"SELECT DISTINCT tp.grantee, COUNT(*) as permission_count
FROM information_schema.table_privileges tp
WHERE tp.grantee NOT IN (SELECT usename FROM pg_user)
  AND tp.grantee NOT IN (SELECT role_name FROM svv_roles)
GROUP BY tp.grantee
ORDER BY permission_count DESC",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Permission Explosion Detection",
                    Category = "Audit",
                    Description = "Find users/roles with excessive permissions",
                    SqlQuery = @"SELECT grantee,
       COUNT(DISTINCT table_schema) as schema_count,
       COUNT(DISTINCT table_name) as table_count,
       COUNT(*) as total_permissions
FROM information_schema.table_privileges
GROUP BY grantee
HAVING COUNT(*) > 100
ORDER BY total_permissions DESC",
                    HasParameters = false,
                    IsBuiltIn = true
                },

                // Category: Activity
                new QueryTemplate
                {
                    Name = "Active Queries",
                    Category = "Activity",
                    Description = "See currently running queries by user",
                    SqlQuery = @"SELECT userid, query, starttime, substring(text, 1, 100) as query_text
FROM stv_inflight
ORDER BY starttime DESC",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Query History (Provisioned)",
                    Category = "Activity",
                    Description = "Historical query execution log (Provisioned)",
                    SqlQuery = @"SELECT userid, query, starttime, endtime, aborted, substring(querytxt, 1, 100) as query_text
FROM stl_query
WHERE userid > 1
ORDER BY starttime DESC
LIMIT 100",
                    HasParameters = false,
                    IsBuiltIn = true
                },
                new QueryTemplate
                {
                    Name = "Query History (Serverless)",
                    Category = "Activity",
                    Description = "Historical query execution log (Serverless)",
                    SqlQuery = @"SELECT user_id, query_id, start_time, end_time, status, query_text
FROM sys_query_history
ORDER BY start_time DESC
LIMIT 100",
                    HasParameters = false,
                    IsBuiltIn = true
                }
            };
        }
    }
}

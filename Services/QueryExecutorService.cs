using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Service for executing dynamic SQL queries
    /// </summary>
    public class QueryExecutorService
    {
        private readonly RedshiftOdbcService _odbcService;

        public QueryExecutorService(RedshiftOdbcService odbcService)
        {
            _odbcService = odbcService;
        }

        /// <summary>
        /// Execute a SQL query and return results as DataTable
        /// </summary>
        public DataTable ExecuteQuery(string sql)
        {
            return ExecuteQuery(sql, null);
        }

        /// <summary>
        /// Execute a SQL query with parameters and return results as DataTable
        /// </summary>
        public DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                OdbcConnection connection = _odbcService.GetConnection();

                using (OdbcCommand cmd = new OdbcCommand(sql, connection))
                {
                    // Set command timeout (5 minutes)
                    cmd.CommandTimeout = 300;

                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (OdbcException ex)
            {
                throw new Exception("Query execution failed: " + ex.Message +
                    " (SQL State: " + ex.Errors[0].SQLState + ")", ex);
            }

            return dt;
        }

        /// <summary>
        /// Execute a non-query SQL command (INSERT, UPDATE, DELETE)
        /// </summary>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }

        /// <summary>
        /// Execute a non-query SQL command with parameters
        /// </summary>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            int rowsAffected = 0;

            try
            {
                OdbcConnection connection = _odbcService.GetConnection();

                using (OdbcCommand cmd = new OdbcCommand(sql, connection))
                {
                    cmd.CommandTimeout = 300;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                throw new Exception("Command execution failed: " + ex.Message +
                    " (SQL State: " + ex.Errors[0].SQLState + ")", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Execute scalar query (returns single value)
        /// </summary>
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }

        /// <summary>
        /// Execute scalar query with parameters
        /// </summary>
        public object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            object result = null;

            try
            {
                OdbcConnection connection = _odbcService.GetConnection();

                using (OdbcCommand cmd = new OdbcCommand(sql, connection))
                {
                    cmd.CommandTimeout = 300;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    result = cmd.ExecuteScalar();
                }
            }
            catch (OdbcException ex)
            {
                throw new Exception("Scalar query execution failed: " + ex.Message +
                    " (SQL State: " + ex.Errors[0].SQLState + ")", ex);
            }

            return result;
        }

        /// <summary>
        /// Export query results to CSV
        /// </summary>
        public void ExportToCsv(DataTable dt, string filePath)
        {
            StringBuilder csv = new StringBuilder();

            // Header
            List<string> columnNames = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                columnNames.Add(EscapeCsvValue(column.ColumnName));
            }
            csv.AppendLine(string.Join(",", columnNames.ToArray()));

            // Data
            foreach (DataRow row in dt.Rows)
            {
                List<string> values = new List<string>();
                foreach (object item in row.ItemArray)
                {
                    values.Add(EscapeCsvValue(item == null || item == DBNull.Value ? "" : item.ToString()));
                }
                csv.AppendLine(string.Join(",", values.ToArray()));
            }

            System.IO.File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }

        private string EscapeCsvValue(string value)
        {
            if (value == null)
            {
                return "";
            }

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        /// <summary>
        /// Validate SQL query syntax (basic validation)
        /// </summary>
        public QueryValidationResult ValidateQuery(string sql)
        {
            QueryValidationResult result = new QueryValidationResult
            {
                IsValid = true,
                Warnings = new List<string>()
            };

            if (string.IsNullOrWhiteSpace(sql))
            {
                result.IsValid = false;
                result.ErrorMessage = "Query cannot be empty";
                return result;
            }

            string upperSql = sql.ToUpper().Trim();

            // Check for dangerous commands
            string[] dangerousCommands = new string[]
            {
                "DROP DATABASE",
                "DROP CLUSTER",
                "TRUNCATE ALL",
                "DELETE FROM pg_",
                "UPDATE pg_"
            };

            foreach (string dangerous in dangerousCommands)
            {
                if (upperSql.Contains(dangerous))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Dangerous command detected: " + dangerous;
                    return result;
                }
            }

            // Warnings for potentially expensive operations
            if (upperSql.Contains("SELECT *") && !upperSql.Contains("LIMIT"))
            {
                result.Warnings.Add("SELECT * without LIMIT can return large result sets");
            }

            if ((upperSql.Contains("DELETE") || upperSql.Contains("UPDATE")) && !upperSql.Contains("WHERE"))
            {
                result.Warnings.Add("DELETE/UPDATE without WHERE clause will affect all rows");
            }

            if (upperSql.Contains("CROSS JOIN"))
            {
                result.Warnings.Add("CROSS JOIN can produce very large result sets");
            }

            return result;
        }

        /// <summary>
        /// Get query execution plan (EXPLAIN)
        /// </summary>
        public DataTable GetQueryPlan(string sql)
        {
            string explainQuery = "EXPLAIN " + sql;
            return ExecuteQuery(explainQuery);
        }

        /// <summary>
        /// Execute query template with parameter substitution
        /// </summary>
        public DataTable ExecuteQueryTemplate(QueryTemplate template, Dictionary<string, object> parameters)
        {
            if (template.HasParameters && (parameters == null || parameters.Count == 0))
            {
                throw new Exception("This query requires parameters: " + template.ParameterNames);
            }

            return ExecuteQuery(template.SqlQuery, parameters);
        }
    }

    /// <summary>
    /// Query validation result
    /// </summary>
    public class QueryValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Warnings { get; set; }
    }
}

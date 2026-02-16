using System;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Represents a table-level permission
    /// </summary>
    public class TablePermission
    {
        public string Username { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string PermissionType { get; set; } // SELECT, INSERT, UPDATE, DELETE

        public string GetFullTableName()
        {
            return string.Format("{0}.{1}", SchemaName, TableName);
        }

        public override string ToString()
        {
            return string.Format("{0} on {1}.{2}: {3}",
                Username, SchemaName, TableName, PermissionType);
        }
    }
}

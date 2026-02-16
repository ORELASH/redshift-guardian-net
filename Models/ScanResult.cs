using System;
using System.Collections.Generic;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Result of a cluster scan operation
    /// </summary>
    public class ScanResult
    {
        public int ClusterId { get; set; }
        public string ClusterName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public List<RedshiftUser> Users { get; set; }
        public int UserCount { get; set; }

        public List<TablePermission> TablePermissions { get; set; }
        public int PermissionCount { get; set; }

        public List<RoleLineage> RoleLineages { get; set; }

        public ScanResult()
        {
            Users = new List<RedshiftUser>();
            TablePermissions = new List<TablePermission>();
            RoleLineages = new List<RoleLineage>();
        }

        public override string ToString()
        {
            return string.Format("Scan of {0}: {1} ({2} users, {3} permissions)",
                ClusterName,
                Success ? "Success" : "Failed",
                UserCount,
                PermissionCount);
        }
    }
}

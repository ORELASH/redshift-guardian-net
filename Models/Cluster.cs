using System;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Represents a Redshift cluster configuration
    /// </summary>
    public class Cluster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string ClusterType { get; set; } // "Provisioned" or "Serverless"
        public string Region { get; set; }
        public string AwsProfile { get; set; }
        public bool UseIAM { get; set; }
        public DateTime? LastScanTime { get; set; }
        public string LastScanStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Cluster()
        {
            Port = 5439;
            ClusterType = "Provisioned";
            UseIAM = true;
            AwsProfile = "default";
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public string GetDisplayName()
        {
            return string.Format("{0} ({1})", Name, ClusterType);
        }

        public override string ToString()
        {
            return GetDisplayName();
        }
    }
}

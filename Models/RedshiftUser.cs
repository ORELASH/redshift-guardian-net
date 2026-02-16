using System;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Represents a Redshift database user
    /// </summary>
    public class RedshiftUser
    {
        public string Username { get; set; }
        public int UserId { get; set; }
        public bool CanCreateDB { get; set; }
        public bool IsSuperuser { get; set; }
        public bool CanCreateUser { get; set; }
        public DateTime? ValidUntil { get; set; }

        public override string ToString()
        {
            return Username;
        }
    }
}

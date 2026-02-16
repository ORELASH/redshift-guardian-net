using System;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Represents role inheritance/lineage
    /// </summary>
    public class RoleLineage
    {
        public string GrantedRole { get; set; }
        public string Grantor { get; set; }
        public bool AdminOption { get; set; }

        public override string ToString()
        {
            return string.Format("{0} granted by {1}{2}",
                GrantedRole, Grantor,
                AdminOption ? " (with admin)" : "");
        }
    }
}

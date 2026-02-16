using System;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Represents a SQL query template
    /// </summary>
    public class QueryTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string SqlQuery { get; set; }
        public bool HasParameters { get; set; }
        public string ParameterNames { get; set; }  // Comma-separated
        public bool IsBuiltIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public QueryTemplate()
        {
            CreatedAt = DateTime.Now;
        }
    }
}

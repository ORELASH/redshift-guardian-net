using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using RedshiftGuardianNET.Models;

namespace RedshiftGuardianNET.DataAccess
{
    /// <summary>
    /// Repository for managing Cluster entities in local database
    /// Provides CRUD operations for cluster metadata
    /// </summary>
    public class ClusterRepository
    {
        private readonly DatabaseContext _context;

        public ClusterRepository(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all clusters from local database
        /// </summary>
        public List<Cluster> FindAll()
        {
            var clusters = new List<Cluster>();

            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Id, Name, Host, Port, [Database], ClusterType, Region,
                           AwsProfile, UseIAM, LastScanTime, LastScanStatus,
                           CreatedAt, UpdatedAt
                    FROM Clusters
                    ORDER BY Name
                ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clusters.Add(MapFromReader(reader));
                    }
                }
            }

            return clusters;
        }

        /// <summary>
        /// Gets a cluster by ID
        /// </summary>
        public Cluster FindById(int id)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Id, Name, Host, Port, [Database], ClusterType, Region,
                           AwsProfile, UseIAM, LastScanTime, LastScanStatus,
                           CreatedAt, UpdatedAt
                    FROM Clusters
                    WHERE Id = @id
                ";

                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapFromReader(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a cluster by name
        /// </summary>
        public Cluster FindByName(string name)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Id, Name, Host, Port, [Database], ClusterType, Region,
                           AwsProfile, UseIAM, LastScanTime, LastScanStatus,
                           CreatedAt, UpdatedAt
                    FROM Clusters
                    WHERE Name = @name
                ";

                cmd.Parameters.AddWithValue("@name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapFromReader(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Inserts a new cluster
        /// </summary>
        public void Insert(Cluster cluster)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Clusters (Name, Host, Port, [Database], ClusterType, Region,
                                         AwsProfile, UseIAM, CreatedAt, UpdatedAt)
                    VALUES (@name, @host, @port, @database, @clusterType, @region,
                           @awsProfile, @useIAM, @createdAt, @updatedAt);

                    SELECT @@IDENTITY
                ";

                AddParameters(cmd, cluster);

                object result = cmd.ExecuteScalar();
                cluster.Id = Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Updates an existing cluster
        /// </summary>
        public void Update(Cluster cluster)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Clusters
                    SET Name = @name,
                        Host = @host,
                        Port = @port,
                        [Database] = @database,
                        ClusterType = @clusterType,
                        Region = @region,
                        AwsProfile = @awsProfile,
                        UseIAM = @useIAM,
                        UpdatedAt = @updatedAt
                    WHERE Id = @id
                ";

                AddParameters(cmd, cluster);
                cmd.Parameters.AddWithValue("@id", cluster.Id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException(
                        string.Format("Cluster with ID {0} not found", cluster.Id));
                }
            }
        }

        /// <summary>
        /// Saves a cluster (insert if new, update if existing)
        /// </summary>
        public void Save(Cluster cluster)
        {
            if (cluster.Id == 0)
            {
                Insert(cluster);
            }
            else
            {
                Update(cluster);
            }
        }

        /// <summary>
        /// Deletes a cluster by ID
        /// </summary>
        public void Delete(int id)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Clusters WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException(
                        string.Format("Cluster with ID {0} not found", id));
                }
            }
        }

        /// <summary>
        /// Deletes a cluster
        /// </summary>
        public void Delete(Cluster cluster)
        {
            Delete(cluster.Id);
        }

        /// <summary>
        /// Updates last scan time and status for a cluster
        /// </summary>
        public void UpdateScanStatus(int clusterId, DateTime scanTime, string status)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Clusters
                    SET LastScanTime = @scanTime,
                        LastScanStatus = @status,
                        UpdatedAt = @updatedAt
                    WHERE Id = @id
                ";

                cmd.Parameters.AddWithValue("@scanTime", scanTime);
                cmd.Parameters.AddWithValue("@status", status ?? "");
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", clusterId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if a cluster with given name exists
        /// </summary>
        public bool ExistsByName(string name)
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Clusters WHERE Name = @name";
                cmd.Parameters.AddWithValue("@name", name);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        /// <summary>
        /// Gets count of all clusters
        /// </summary>
        public int Count()
        {
            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Clusters";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Gets clusters that haven't been scanned yet
        /// </summary>
        public List<Cluster> FindUnscanned()
        {
            var clusters = new List<Cluster>();

            using (var cmd = _context.GetConnection().CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT Id, Name, Host, Port, [Database], ClusterType, Region,
                           AwsProfile, UseIAM, LastScanTime, LastScanStatus,
                           CreatedAt, UpdatedAt
                    FROM Clusters
                    WHERE LastScanTime IS NULL
                    ORDER BY Name
                ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clusters.Add(MapFromReader(reader));
                    }
                }
            }

            return clusters;
        }

        /// <summary>
        /// Maps a data reader row to a Cluster object
        /// </summary>
        private Cluster MapFromReader(SqlCeDataReader reader)
        {
            return new Cluster
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString(),
                Host = reader["Host"].ToString(),
                Port = Convert.ToInt32(reader["Port"]),
                Database = reader["Database"].ToString(),
                ClusterType = reader["ClusterType"].ToString(),
                Region = reader["Region"].ToString(),
                AwsProfile = reader["AwsProfile"] == DBNull.Value
                    ? null
                    : reader["AwsProfile"].ToString(),
                UseIAM = Convert.ToBoolean(reader["UseIAM"]),
                LastScanTime = reader["LastScanTime"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["LastScanTime"]),
                LastScanStatus = reader["LastScanStatus"] == DBNull.Value
                    ? null
                    : reader["LastScanStatus"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        /// <summary>
        /// Adds parameters to a command for insert/update
        /// </summary>
        private void AddParameters(SqlCeCommand cmd, Cluster cluster)
        {
            cmd.Parameters.AddWithValue("@name", cluster.Name ?? "");
            cmd.Parameters.AddWithValue("@host", cluster.Host ?? "");
            cmd.Parameters.AddWithValue("@port", cluster.Port);
            cmd.Parameters.AddWithValue("@database", cluster.Database ?? "");
            cmd.Parameters.AddWithValue("@clusterType", cluster.ClusterType ?? "Provisioned");
            cmd.Parameters.AddWithValue("@region", cluster.Region ?? "");
            cmd.Parameters.AddWithValue("@awsProfile",
                string.IsNullOrEmpty(cluster.AwsProfile) ? (object)DBNull.Value : cluster.AwsProfile);
            cmd.Parameters.AddWithValue("@useIAM", cluster.UseIAM);
            cmd.Parameters.AddWithValue("@createdAt", cluster.CreatedAt == default(DateTime)
                ? DateTime.Now
                : cluster.CreatedAt);
            cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
        }
    }
}

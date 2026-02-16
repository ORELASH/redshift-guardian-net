using System;
using System.Collections.Generic;
using RedshiftGuardianNET.Models;
using RedshiftGuardianNET.DataAccess;

namespace RedshiftGuardianNET.Services
{
    /// <summary>
    /// Service for managing Redshift clusters
    /// Provides business logic layer between UI and data access
    /// </summary>
    public class ClusterService : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly ClusterRepository _repository;
        private bool _disposed = false;

        public ClusterService()
        {
            _context = new DatabaseContext();
            _repository = new ClusterRepository(_context);
        }

        /// <summary>
        /// Gets all clusters
        /// </summary>
        public List<Cluster> GetAllClusters()
        {
            return _repository.FindAll();
        }

        /// <summary>
        /// Gets a cluster by ID
        /// </summary>
        public Cluster GetClusterById(int id)
        {
            return _repository.FindById(id);
        }

        /// <summary>
        /// Gets a cluster by name
        /// </summary>
        public Cluster GetClusterByName(string name)
        {
            return _repository.FindByName(name);
        }

        /// <summary>
        /// Saves a cluster (insert or update)
        /// </summary>
        /// <param name="cluster">The cluster to save</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if saved successfully, false otherwise</returns>
        public bool SaveCluster(Cluster cluster, out string errorMessage)
        {
            errorMessage = null;

            // Validation
            if (string.IsNullOrWhiteSpace(cluster.Name))
            {
                errorMessage = "Cluster name is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cluster.Host))
            {
                errorMessage = "Host is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cluster.Database))
            {
                errorMessage = "Database name is required";
                return false;
            }

            if (cluster.Port <= 0 || cluster.Port > 65535)
            {
                errorMessage = "Port must be between 1 and 65535";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cluster.Region))
            {
                errorMessage = "AWS region is required";
                return false;
            }

            // Check for duplicate name (only if new or name changed)
            if (cluster.Id == 0 || HasNameChanged(cluster))
            {
                if (_repository.ExistsByName(cluster.Name))
                {
                    errorMessage = string.Format("A cluster with name '{0}' already exists", cluster.Name);
                    return false;
                }
            }

            try
            {
                _repository.Save(cluster);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to save cluster: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Deletes a cluster
        /// </summary>
        public bool DeleteCluster(int clusterId, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                var cluster = _repository.FindById(clusterId);
                if (cluster == null)
                {
                    errorMessage = "Cluster not found";
                    return false;
                }

                _repository.Delete(clusterId);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to delete cluster: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Tests connection to a Redshift cluster
        /// </summary>
        public bool TestConnection(Cluster cluster, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                var repo = new RedshiftRepository(cluster);
                bool connected = repo.TestConnection();

                if (!connected)
                {
                    errorMessage = "Connection test failed. Check cluster settings and AWS credentials.";
                }

                return connected;
            }
            catch (Exception ex)
            {
                errorMessage = "Connection test failed: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Updates the last scan status for a cluster
        /// </summary>
        public void UpdateScanStatus(int clusterId, DateTime scanTime, string status)
        {
            _repository.UpdateScanStatus(clusterId, scanTime, status);
        }

        /// <summary>
        /// Gets clusters that have never been scanned
        /// </summary>
        public List<Cluster> GetUnscannedClusters()
        {
            return _repository.FindUnscanned();
        }

        /// <summary>
        /// Gets total count of clusters
        /// </summary>
        public int GetClusterCount()
        {
            return _repository.Count();
        }

        /// <summary>
        /// Creates a sample cluster for testing
        /// </summary>
        public Cluster CreateSampleCluster()
        {
            return new Cluster
            {
                Name = "sample-cluster",
                Host = "my-cluster.abc123.us-east-1.redshift.amazonaws.com",
                Port = 5439,
                Database = "dev",
                ClusterType = "Provisioned",
                Region = "us-east-1",
                AwsProfile = "default",
                UseIAM = true
            };
        }

        private bool HasNameChanged(Cluster cluster)
        {
            if (cluster.Id == 0)
            {
                return false; // New cluster
            }

            var existing = _repository.FindById(cluster.Id);
            if (existing == null)
            {
                return false;
            }

            return existing.Name != cluster.Name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~ClusterService()
        {
            Dispose(false);
        }
    }
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace RedshiftGuardianNET.Models
{
    /// <summary>
    /// Application settings and preferences
    /// </summary>
    [Serializable]
    public class AppSettings
    {
        // Connection Settings
        public int ConnectionTimeoutSeconds { get; set; }
        public int QueryTimeoutSeconds { get; set; }
        public bool AutoConnectLastCluster { get; set; }

        // UI Settings
        public int FontSizePoints { get; set; }
        public string Theme { get; set; }
        public bool ShowStatusBar { get; set; }
        public bool ShowToolBar { get; set; }

        // Export Settings
        public string DefaultExportPath { get; set; }
        public string DefaultExportFormat { get; set; }
        public bool IncludeTimestampInFilename { get; set; }
        public bool OpenFileAfterExport { get; set; }

        // Scan Settings
        public int AutoRefreshMinutes { get; set; }
        public bool EnableAutoRefresh { get; set; }
        public bool ScanOnStartup { get; set; }
        public bool CacheResults { get; set; }

        // Query Editor Settings
        public int QueryHistorySize { get; set; }
        public bool SaveQueryHistory { get; set; }
        public bool AutoCompleteEnabled { get; set; }
        public int QueryResultsMaxRows { get; set; }

        // Advanced Settings
        public string LogLevel { get; set; }
        public bool EnableDetailedLogging { get; set; }
        public int MaxConcurrentScans { get; set; }
        public string Language { get; set; }

        /// <summary>
        /// Constructor with default values
        /// </summary>
        public AppSettings()
        {
            // Connection defaults
            ConnectionTimeoutSeconds = 30;
            QueryTimeoutSeconds = 60;
            AutoConnectLastCluster = false;

            // UI defaults
            FontSizePoints = 9;
            Theme = "Default";
            ShowStatusBar = true;
            ShowToolBar = true;

            // Export defaults
            DefaultExportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DefaultExportFormat = "CSV";
            IncludeTimestampInFilename = true;
            OpenFileAfterExport = false;

            // Scan defaults
            AutoRefreshMinutes = 0; // Disabled
            EnableAutoRefresh = false;
            ScanOnStartup = false;
            CacheResults = true;

            // Query Editor defaults
            QueryHistorySize = 50;
            SaveQueryHistory = true;
            AutoCompleteEnabled = true;
            QueryResultsMaxRows = 10000;

            // Advanced defaults
            LogLevel = "Info";
            EnableDetailedLogging = false;
            MaxConcurrentScans = 3;
            Language = "English";
        }

        /// <summary>
        /// Save settings to XML file
        /// </summary>
        public void Save()
        {
            Save(GetSettingsFilePath());
        }

        /// <summary>
        /// Save settings to specified file
        /// </summary>
        public void Save(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save settings: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Load settings from XML file
        /// </summary>
        public static AppSettings Load()
        {
            return Load(GetSettingsFilePath());
        }

        /// <summary>
        /// Load settings from specified file
        /// </summary>
        public static AppSettings Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    // Return defaults if file doesn't exist
                    AppSettings defaults = new AppSettings();
                    defaults.Save(); // Create default settings file
                    return defaults;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    AppSettings settings = (AppSettings)serializer.Deserialize(reader);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load settings: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        public void Reset()
        {
            AppSettings defaults = new AppSettings();

            // Copy all properties
            this.ConnectionTimeoutSeconds = defaults.ConnectionTimeoutSeconds;
            this.QueryTimeoutSeconds = defaults.QueryTimeoutSeconds;
            this.AutoConnectLastCluster = defaults.AutoConnectLastCluster;

            this.FontSizePoints = defaults.FontSizePoints;
            this.Theme = defaults.Theme;
            this.ShowStatusBar = defaults.ShowStatusBar;
            this.ShowToolBar = defaults.ShowToolBar;

            this.DefaultExportPath = defaults.DefaultExportPath;
            this.DefaultExportFormat = defaults.DefaultExportFormat;
            this.IncludeTimestampInFilename = defaults.IncludeTimestampInFilename;
            this.OpenFileAfterExport = defaults.OpenFileAfterExport;

            this.AutoRefreshMinutes = defaults.AutoRefreshMinutes;
            this.EnableAutoRefresh = defaults.EnableAutoRefresh;
            this.ScanOnStartup = defaults.ScanOnStartup;
            this.CacheResults = defaults.CacheResults;

            this.QueryHistorySize = defaults.QueryHistorySize;
            this.SaveQueryHistory = defaults.SaveQueryHistory;
            this.AutoCompleteEnabled = defaults.AutoCompleteEnabled;
            this.QueryResultsMaxRows = defaults.QueryResultsMaxRows;

            this.LogLevel = defaults.LogLevel;
            this.EnableDetailedLogging = defaults.EnableDetailedLogging;
            this.MaxConcurrentScans = defaults.MaxConcurrentScans;
            this.Language = defaults.Language;
        }

        /// <summary>
        /// Get settings file path
        /// </summary>
        private static string GetSettingsFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "RedshiftGuardian");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return Path.Combine(appFolder, "settings.xml");
        }

        /// <summary>
        /// Check if settings file exists
        /// </summary>
        public static bool Exists()
        {
            return File.Exists(GetSettingsFilePath());
        }

        /// <summary>
        /// Delete settings file
        /// </summary>
        public static void Delete()
        {
            string filePath = GetSettingsFilePath();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Validate settings
        /// </summary>
        public bool Validate(out string errorMessage)
        {
            errorMessage = "";

            if (ConnectionTimeoutSeconds < 5 || ConnectionTimeoutSeconds > 300)
            {
                errorMessage = "Connection timeout must be between 5 and 300 seconds";
                return false;
            }

            if (QueryTimeoutSeconds < 10 || QueryTimeoutSeconds > 600)
            {
                errorMessage = "Query timeout must be between 10 and 600 seconds";
                return false;
            }

            if (FontSizePoints < 6 || FontSizePoints > 20)
            {
                errorMessage = "Font size must be between 6 and 20 points";
                return false;
            }

            if (AutoRefreshMinutes < 0 || AutoRefreshMinutes > 1440)
            {
                errorMessage = "Auto refresh must be between 0 and 1440 minutes (24 hours)";
                return false;
            }

            if (QueryHistorySize < 0 || QueryHistorySize > 1000)
            {
                errorMessage = "Query history size must be between 0 and 1000";
                return false;
            }

            if (QueryResultsMaxRows < 100 || QueryResultsMaxRows > 100000)
            {
                errorMessage = "Query results max rows must be between 100 and 100000";
                return false;
            }

            if (MaxConcurrentScans < 1 || MaxConcurrentScans > 10)
            {
                errorMessage = "Max concurrent scans must be between 1 and 10";
                return false;
            }

            if (!Directory.Exists(DefaultExportPath))
            {
                try
                {
                    Directory.CreateDirectory(DefaultExportPath);
                }
                catch
                {
                    errorMessage = "Default export path is invalid or cannot be created";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Export settings to text file for backup
        /// </summary>
        public void ExportToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("# Redshift Guardian Settings Export");
                writer.WriteLine("# Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine();

                writer.WriteLine("[Connection]");
                writer.WriteLine("ConnectionTimeout=" + ConnectionTimeoutSeconds);
                writer.WriteLine("QueryTimeout=" + QueryTimeoutSeconds);
                writer.WriteLine("AutoConnectLastCluster=" + AutoConnectLastCluster);
                writer.WriteLine();

                writer.WriteLine("[UI]");
                writer.WriteLine("FontSize=" + FontSizePoints);
                writer.WriteLine("Theme=" + Theme);
                writer.WriteLine("ShowStatusBar=" + ShowStatusBar);
                writer.WriteLine("ShowToolBar=" + ShowToolBar);
                writer.WriteLine();

                writer.WriteLine("[Export]");
                writer.WriteLine("DefaultPath=" + DefaultExportPath);
                writer.WriteLine("DefaultFormat=" + DefaultExportFormat);
                writer.WriteLine("IncludeTimestamp=" + IncludeTimestampInFilename);
                writer.WriteLine("OpenAfterExport=" + OpenFileAfterExport);
                writer.WriteLine();

                writer.WriteLine("[Scan]");
                writer.WriteLine("AutoRefreshMinutes=" + AutoRefreshMinutes);
                writer.WriteLine("EnableAutoRefresh=" + EnableAutoRefresh);
                writer.WriteLine("ScanOnStartup=" + ScanOnStartup);
                writer.WriteLine("CacheResults=" + CacheResults);
                writer.WriteLine();

                writer.WriteLine("[QueryEditor]");
                writer.WriteLine("QueryHistorySize=" + QueryHistorySize);
                writer.WriteLine("SaveQueryHistory=" + SaveQueryHistory);
                writer.WriteLine("AutoComplete=" + AutoCompleteEnabled);
                writer.WriteLine("MaxRows=" + QueryResultsMaxRows);
                writer.WriteLine();

                writer.WriteLine("[Advanced]");
                writer.WriteLine("LogLevel=" + LogLevel);
                writer.WriteLine("DetailedLogging=" + EnableDetailedLogging);
                writer.WriteLine("MaxConcurrentScans=" + MaxConcurrentScans);
                writer.WriteLine("Language=" + Language);
            }
        }
    }
}

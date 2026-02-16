using System;
using System.Collections.Generic;
using System.Linq;

namespace RedshiftGuardianNET.CLI
{
    /// <summary>
    /// Command line argument parser
    /// </summary>
    public class CommandLineParser
    {
        public string Command { get; private set; }
        public Dictionary<string, string> Arguments { get; private set; }
        public List<string> Flags { get; private set; }

        public CommandLineParser()
        {
            Command = "";
            Arguments = new Dictionary<string, string>();
            Flags = new List<string>();
        }

        /// <summary>
        /// Parse command line arguments
        /// </summary>
        public bool Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false; // No CLI mode
            }

            // First argument is the command
            Command = args[0].ToLower();

            // Parse remaining arguments
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith("--"))
                {
                    // Long argument (--key value or --flag)
                    string key = arg.Substring(2);

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        // Has value
                        Arguments[key] = args[i + 1];
                        i++; // Skip next arg
                    }
                    else
                    {
                        // Flag only
                        Flags.Add(key);
                    }
                }
                else if (arg.StartsWith("-"))
                {
                    // Short argument (-k value or -f)
                    string key = arg.Substring(1);

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        // Has value
                        Arguments[key] = args[i + 1];
                        i++; // Skip next arg
                    }
                    else
                    {
                        // Flag only
                        Flags.Add(key);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get argument value
        /// </summary>
        public string GetArgument(string key, string defaultValue = "")
        {
            if (Arguments.ContainsKey(key))
            {
                return Arguments[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Check if flag exists
        /// </summary>
        public bool HasFlag(string flag)
        {
            return Flags.Contains(flag);
        }

        /// <summary>
        /// Check if argument exists
        /// </summary>
        public bool HasArgument(string key)
        {
            return Arguments.ContainsKey(key);
        }

        /// <summary>
        /// Get all argument keys
        /// </summary>
        public string[] GetArgumentKeys()
        {
            return Arguments.Keys.ToArray();
        }
    }
}

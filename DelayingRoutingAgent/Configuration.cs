using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Configuration;
using System.Diagnostics;

namespace DelayAgentConfig
{

    public interface IConfigurationProvider
    {
        // Categorizer OnCategorizedMessage delay
        int CatCMdelay { get; set; }
        // Categorizer OnResolvedMessage delay
        int CATRSdelay { get; set; }
        // Categorizer OnSubmittedMessage delay
        int CATSMdelay { get; set; }
        // Categorizer OnRoutedMessage delay
        int CATRTdelay { get; set; }
        // Should raise an ThreadAbort call
        bool ThreadAbort { get; set; }
    }


    public class AgentConfig : IConfigurationProvider
    {

        // Categorizer OnCategorizedMessage delay
        public int CatCMdelay { get; set; }

        // Categorizer OnResolvedMessage delay
        public int CATRSdelay { get; set; }

        // Categorizer OnSubmittedMessage delay
        public int CATSMdelay { get; set; }

        // Categorizer OnRoutedMessage delay
        public int CATRTdelay { get; set; }

        // Should raise an ThreadAbort call
        public bool ThreadAbort { get; set; }

        // the name of the configuration file.
        private static readonly string configFileName = "DelayRoutingAgent.config";

        // the directory with the configuration file.
        private string configDirectory;

        // the filesystem watcher to monitor configuration file updates.
        private FileSystemWatcher configFileWatcher;


        // Constructor
        public AgentConfig() {
            // Setup a file system watcher to monitor the configuration file
            this.configDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.configFileWatcher = new FileSystemWatcher(this.configDirectory);
            this.configFileWatcher.NotifyFilter= NotifyFilters.LastWrite;
            this.configFileWatcher.Filter = configFileName;
            this.configFileWatcher.Changed += new FileSystemEventHandler(this.ConfigFileWatcher_Changed);
            
            // Load the configuration
            this.Load();

            this.configFileWatcher.EnableRaisingEvents = true;
        }

        private void ConfigFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.Load();
        }

        /// <summary>
        /// Load the configuration file. If any errors occur, does nothing.
        /// </summary>
        private void Load() 
        {
            // not yet implemented
            // this should read the xml? config file
            // using hardcoded values for now.
            ConfigXmlDocument configDoc = new ConfigXmlDocument();
            bool docLoaded = false;
            string filename = Path.Combine(this.configDirectory, configFileName);

            try
            {
                // Get the current configuration file.
                configDoc.Load(filename);
                docLoaded = true;

                // Get elements
                XmlNodeList elemList = configDoc.GetElementsByTagName("add");
                for (int i = 0; i < elemList.Count; i++)
                {
                    int intValue;
                    bool boolValue;

                    //Console.WriteLine("Key {0}, Value {1}", elemList[i].Attributes[0].Value, elemList[i].Attributes[1].Value);
                    switch (elemList[i].Attributes[0].Value)
                    {
                        case "CatCMdelay":
                            {
                                int.TryParse(elemList[i].Attributes[1].Value, out intValue);
                                CatCMdelay = intValue;
                                break;
                            }
                        case "CATRSdelay":
                            {
                                int.TryParse(elemList[i].Attributes[1].Value, out intValue);
                                CATRSdelay = intValue;
                                break;
                            }
                        case "CATSMdelay":
                            {
                                int.TryParse(elemList[i].Attributes[1].Value, out intValue);
                                CATSMdelay = intValue;
                                break;
                            }
                        case "CATRTdelay":
                            {
                                int.TryParse(elemList[i].Attributes[1].Value, out intValue);
                                CATRTdelay = intValue;
                                break;
                            }
                        case "ThreadAbort":
                            {
                                bool.TryParse(elemList[i].Attributes[1].Value, out boolValue);
                                ThreadAbort = boolValue;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Trace.WriteLine("File not found: {0}", filename);
            }
            catch (XmlException ex)
            {
                Trace.WriteLine("XML error: {0}", ex.Message);
            }
            catch (IOException ex)
            {
                Trace.WriteLine("IO error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("unhandled exception: {0}", ex.InnerException.ToString());
                throw;
            }

            if (!docLoaded)
            {
                // If we had issues loading configuration from file.
                // load static config below.
                CatCMdelay = 0;
                CATRSdelay = 0;
                CATRTdelay = 0;
                CATSMdelay = 0;
                ThreadAbort = false;
            }
        }
    }
}
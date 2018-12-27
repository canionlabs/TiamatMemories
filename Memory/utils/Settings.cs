using System;
using System.Configuration;

namespace Memory.utils
{
    public static class Settings
    {

        public static string Host
        {
            get { return Read("broker.ip"); }
        }

        public static string Port
        {
            get { return Read("broker.port"); }
        }

        public static string Entrypoint
        {
            get { return Read("entry_point"); }
        }

        public static string MainEntrypoint
        {
            get { return Read("main_entrypoint"); }
        }

        public static string MaxTopics
        {
            get { return Read("max_topics"); }
        }

        public static String Read(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            return null;
        }
    }
}


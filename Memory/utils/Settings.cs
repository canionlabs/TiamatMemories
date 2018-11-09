using System;
using System.Configuration;

namespace Memory.utils
{
    public static class Settings
    {
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

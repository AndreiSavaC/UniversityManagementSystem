namespace Logging
{
    using System.Reflection;
    using System.IO;
    using log4net;
    using log4net.Config;
    public static class LogInitializer
    {
        private static bool _isConfigured;

        public static void ConfigureLogging()
        {
            if (_isConfigured)
                return;

            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("The assembly location could not be determined.");

            var configPath = Path.Combine(directory, "log4net.config");

            XmlConfigurator.Configure(logRepository, new FileInfo(configPath));

            _isConfigured = true;
        }
    }

}



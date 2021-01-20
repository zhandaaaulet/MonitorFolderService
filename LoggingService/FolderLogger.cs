using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggingService
{
    public class FolderLogger : IFolderLogger
    {
        private readonly FolderLoggerConfiguration _loggerConfiguration;

        public FolderLogger(FolderLoggerConfiguration loggerConfiguration, string logMessage)
        {
            _loggerConfiguration = loggerConfiguration;
            LogWrite(logMessage);
        }

        public void Log(string logMessage, StreamWriter streamWriter)
        {
            streamWriter.WriteLine(logMessage);
            streamWriter.Close();
        }

        public void LogWrite(string logMessage)
        {
            try
            {
                using StreamWriter w = File.AppendText(_loggerConfiguration.Directory);
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveLogData()
        {

            if (File.Exists(_loggerConfiguration.Directory))
            {
                File.WriteAllText(_loggerConfiguration.Directory, String.Empty);
            }
        }
    }
}

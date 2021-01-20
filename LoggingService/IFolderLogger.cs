using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggingService
{
    public interface IFolderLogger
    {
        public void LogWrite(string logMessage);
        public void Log(string logMessage, StreamWriter streamWriter);
        public void RemoveLogData();
    }
}

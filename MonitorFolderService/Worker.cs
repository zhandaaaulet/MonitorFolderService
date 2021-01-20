using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using LoggingService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MonitorFolderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IFolderLogger _folderLogger;
        private FileSystemWatcher watcher;
        private readonly string directory = @"C:\Users\77071\OneDrive\Рабочий стол\MyFolder";
        private readonly string logPath = @"C:\Users\77071\OneDrive\Рабочий стол\Logger.txt";

        public Worker(ILogger<Worker> logger, IEmailSender emailSender, IFolderLogger folderLogger)
        {
            _logger = logger;
            _emailSender = emailSender;
            _folderLogger = folderLogger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher
            {
                Path = directory
            };
            watcher.Created += OnChanged;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            _folderLogger.RemoveLogData();
            _folderLogger.LogWrite($"Worker running at: {DateTimeOffset.Now}");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _folderLogger.LogWrite($"Worker stopped: {DateTimeOffset.Now}");
            var message = new Message(new string[] { "z.myrzatayev@astanait.edu.kz" }, "", "", logPath);
            _emailSender.SendEmail(message).Wait();
            return base.StopAsync(cancellationToken);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _logger.LogInformation("A new message about the renaming to be sent at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "z.myrzatayev@astanait.edu.kz" }, $"The file {e.OldName} renamed to {e.Name}", "Simple test", null);
            _emailSender.SendEmail(message);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("A new message about the deletion to be sent at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "z.myrzatayev@astanait.edu.kz" }, "Deleted file on the folder", null, null);
            _emailSender.SendEmail(message);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SendEmail(e.FullPath);
        }

        private void SendEmail(string fullPath)
        {
            _logger.LogInformation("A new message about to be sent at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "z.myrzatayev@astanait.edu.kz" }, "SE-1908 Test Program", "This is the content for the SE-1908", fullPath);
            _emailSender.SendEmail(message);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                watcher.EnableRaisingEvents = true; //starts listening 
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }



    }
}

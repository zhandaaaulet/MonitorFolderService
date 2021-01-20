using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using LoggingService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MonitorFolderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
                    var emailConfig = hostContext
                    .Configuration
                    .GetSection("EmailConfiguration")
                    .Get<EmailConfiguration>();

                    var fileLoggerConfig = hostContext
                    .Configuration.GetSection("FolderLoggerConfiguration")
                    .Get<FolderLoggerConfiguration>();

                    services.AddSingleton(fileLoggerConfig);
                    services.AddSingleton(emailConfig);
                    services.AddSingleton<IEmailSender, EmailSender>();
                    services.AddSingleton<IFolderLogger, FolderLogger>();
                    services.AddHostedService<Worker>();
                });
    }
}

using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace MyCompany.MyApp.DbMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        LoadDotEnv();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("MyCompany.MyApp", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("MyCompany.MyApp", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .AddAppSettingsSecretsJson()
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });

    private static void LoadDotEnv()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current != null)
        {
            var envFilePath = Path.Combine(current.FullName, ".env");
            if (File.Exists(envFilePath))
            {
                foreach (var rawLine in File.ReadAllLines(envFilePath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || !line.Contains('='))
                    {
                        continue;
                    }

                    var index = line.IndexOf('=');
                    var key = line[..index].Trim();
                    var value = line[(index + 1)..].Trim().Trim('"');
                    if (string.IsNullOrWhiteSpace(key) || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                    {
                        continue;
                    }

                    Environment.SetEnvironmentVariable(key, value);
                }

                return;
            }

            current = current.Parent;
        }
    }
}

using System;
using System.IO;
using DesafioAgibank.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DesafioAgibank.ConsoleApp
{
    class Program
    {
        public static IConfigurationRoot configuration;
        
        public static void Main()
        {
            Console.WriteLine("Test started...");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

            try 
            {
                Log.Information("Creating service collection...");
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                Log.Information("Building service provider...");
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Log.Information("Starting service....");
                serviceProvider.GetService<FileService>().Run().Wait();
                Log.Information("Ending service...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, GetInnerException(ex));
            }
            finally
            {
                Log.CloseAndFlush();
            }
            Console.WriteLine("Test finished!");
        }

        private static string GetInnerException(Exception ex)
        {
            return ex.InnerException != null
                ? $"{ex.InnerException.Message} > {GetInnerException(ex.InnerException)} "
                : string.Empty;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(LoggerFactory.Create(builder => 
            {
                builder.AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.AddTransient<FileService>();
        }
    }
}

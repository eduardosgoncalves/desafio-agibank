using System.Threading.Tasks;
using DesafioAgibank.Core;
using DesafioAgibank.Core.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DesafioAgibank.ConsoleApp.Services
{
    public class FileService
    {
        private readonly IConfigurationRoot _config;
        private readonly ILogger<FileService> _logger;

        public FileService(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FileService>();
            _config = config;
        }

        public async Task Run()
        {
            var settings = new CoreSettings
            {
                EnvironmentPath = _config["EnvironmentPath"]
            };
            _config.GetSection("DataIn").Bind(settings.DataIn);
            _config.GetSection("DataOut").Bind(settings.DataOut);

            var results = await FileHelpers.Process(settings);

            foreach(var (description, value) in results) 
            {
                _logger.LogInformation($"{description}: {value}");
            }
        }
    }

}
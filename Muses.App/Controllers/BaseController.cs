using Microsoft.AspNetCore.Mvc;
using Muses.Domain.ViewModels;
using Newtonsoft.Json;

namespace Muses.App.Controllers
{
    public class BaseController : Controller
    {
        IConfiguration _configuration;

        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Notify(string message,string title="Sweet alert Demo",
            NotificationType notificationType = NotificationType.success)
        {
            var msg = new
            {
                message = message,
                title = title,
                icon=notificationType.ToString(),
                Type=notificationType.ToString(),
                provider=_configuration["NotificationProvider"]
            };

            TempData["Message"] = JsonConvert.SerializeObject(msg);
        }

        private string GetProvider()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var value = configuration["NotificationProvider"];

            return value;
        }

    }
}

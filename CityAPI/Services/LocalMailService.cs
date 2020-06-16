using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CityAPI.Services
{
    public class LocalMailService : IMailService
    {
        private readonly ILogger<LocalMailService> _logger;

        private string _mailTo = "admin@company.com";
        private string _mailFrom = "noreply@company.com";

        public LocalMailService(ILogger<LocalMailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail sent from {_mailFrom} to {_mailTo} with MailService");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");

            _logger.LogInformation($"message sent to {_mailTo} on {DateTime.Now}.", subject, message);
        }
    }
}

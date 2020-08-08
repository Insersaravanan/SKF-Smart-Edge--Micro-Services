using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanenceMicroService.Services.Jobs
{
    public class EmailReminderJob : IJob
    {
        private readonly IServiceProvider _provider;
        public EmailReminderJob(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                var emailSender = scope.ServiceProvider.GetService<IEmailSender>();
                // fetch customers, send email, update DB
            }

            return Task.CompletedTask;
        }
    }
}

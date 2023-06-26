using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Specification.ClientBookingSpec;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundTasks
{
    public class CheckProecessingBookingTask : BackgroundService
    {
        private readonly ILogger<CheckProecessingBookingTask> logger;
        private readonly IServiceProvider serviceProvider;

        public CheckProecessingBookingTask(ILogger<CheckProecessingBookingTask> logger, IServiceProvider serviceProvider)
        {
            this.logger=logger;
            this.serviceProvider=serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("check proccessing booking task started.");

                using (var scope = serviceProvider.CreateScope())
                {
                    var clientBookingRepo = scope.ServiceProvider.GetRequiredService<IClientBookingRepo>();

                    var confimredBookings = await clientBookingRepo.FindAsync(e => e.Status == BookingStatus.Confirmed && e.Date.Date == DateTime.Now.Date);

                    foreach (var booking in confimredBookings)
                    {
                        if (booking.StartTime <=  DateTime.Now.TimeOfDay)
                        {
                            booking.Status = BookingStatus.InProcess;
                            await clientBookingRepo.EditAsync(booking.Id, booking, e => e.Id);

                        }
                    }

                }

                logger.LogInformation("check proccessing booking task Ended.");

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);

            }


        }
    }
}

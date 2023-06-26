using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks
{
    public class CheckCompletedBookingsTask : BackgroundService
    {
        private readonly ILogger<CheckCompletedBookingsTask> logger;
        private readonly IServiceProvider serviceProvider;

        public CheckCompletedBookingsTask(ILogger<CheckCompletedBookingsTask> logger, IServiceProvider serviceProvider)
        {
            this.logger=logger;
            this.serviceProvider=serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Check Completed Bookings Task Started");

                using (var scope = serviceProvider.CreateScope())
                {
                    var clientBookingRepo = scope.ServiceProvider.GetRequiredService<IClientBookingRepo>();

                    var processingBookings = await clientBookingRepo.FindAsync(e => e.Status == BookingStatus.InProcess);

                    foreach (var booking in processingBookings)
                    {
                        if (booking.Date == DateTime.Now.Date && booking.EndTime <= DateTime.Now.TimeOfDay)
                        {
                            booking.Status = BookingStatus.Completed;
                            await clientBookingRepo.EditAsync(booking.Id, booking, e => e.Id);
                        }
                    }

                    logger.LogInformation("Check Completed Bookings Task Ended");

                    await Task.Delay(TimeSpan.FromMinutes(30.0), stoppingToken);
                }

            }

        
        }
     }
}


namespace Application.Common.Interfaces.Repositories
{
    public interface IClientBookingRepo: IAsyncRepository<ClientBooking>
    {
        Task<bool> CheckServiceExistence(int serviceId);
    }
}

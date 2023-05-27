namespace Application.Common.Interfaces.Repositories
{
    public interface IServiceRepo: IAsyncRepository<Service>
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service> GetServiceById(int id);
        Task DeleteSoft(int id);
    }
}

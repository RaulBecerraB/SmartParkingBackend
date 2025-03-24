using System.Threading.Tasks;

namespace SmartParkingBackend.Repository
{
    public interface ISeedRepository
    {
        Task<bool> DatabaseHasDataAsync();
        Task InitializeTestDataAsync();
        Task ResetDatabaseAsync();
    }
} 
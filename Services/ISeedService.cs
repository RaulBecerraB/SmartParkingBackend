using System.Threading.Tasks;

namespace SmartParkingBackend.Services
{
    public interface ISeedService
    {
        Task<bool> InitializeAsync();
        Task<bool> ResetAsync();
    }
} 
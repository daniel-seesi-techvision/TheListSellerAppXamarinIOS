using System.Threading.Tasks;

namespace TheListSellerAppXamariniOS.Services
{
    public interface INetworkService
    {
        bool IsInternetConnected();
        Task<bool> IsRemoteReachable();
        bool IsWifiConnected();
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Essentials;

namespace TheListSellerAppXamariniOS.Services
{
    public class NetworkService : INetworkService
    {

        public bool IsInternetConnected()
        {
            return !(Connectivity.NetworkAccess == NetworkAccess.Unknown || Connectivity.NetworkAccess == NetworkAccess.None);
        }

        public async Task<bool> IsRemoteReachable()
        {
            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            return isReachable;
        }

        public bool IsWifiConnected()
        {
            return Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
        }
    }
}


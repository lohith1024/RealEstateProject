using System;
using System.Threading.Tasks;

namespace UserManagementService.Services
{
    public interface IConsulServiceClient
    {
        Task<bool> RegisterServiceAsync();
        Task<bool> DeregisterServiceAsync();
    }

    public class ConsulServiceClient : IConsulServiceClient
    {
        private readonly Uri _consulAddress;

        public ConsulServiceClient(Uri consulAddress)
        {
            _consulAddress = consulAddress;
        }

        public async Task<bool> RegisterServiceAsync()
        {
            // TODO: Implement actual Consul registration
            return true;
        }

        public async Task<bool> DeregisterServiceAsync()
        {
            // TODO: Implement actual Consul deregistration
            return true;
        }
    }
}
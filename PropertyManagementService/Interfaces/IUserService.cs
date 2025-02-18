using System.Threading.Tasks;
using PropertyManagementService.Models;

namespace PropertyManagementService.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int userId);
        Task<bool> ValidateUserAsync(int userId);
    }
}
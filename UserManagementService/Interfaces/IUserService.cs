using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementService.Models;

namespace UserManagementService.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterUser(UserDto userDto);
        Task<bool> DeleteUser(int userId);
        Task<ApplicationUser> UpdateUserDetails(int userId, UserDto userDto);
        Task<ApplicationUser> GetUserDetails(int userId);
        Task<List<ApplicationUser>> GetAllUsers();
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> VerifyUser(int userId);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPassword(string email);
    }
}
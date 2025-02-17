using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using UserManagementService.Models;
using UserManagementService.Interfaces;
using UserManagementService.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace UserManagementService.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, HttpClient httpClient, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpClient = httpClient;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> RegisterUser(UserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                Role = userDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicationUser> UpdateUserDetails(int userId, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            user.UserName = userDto.Username;
            user.Email = userDto.Email;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Role = userDto.Role;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task<ApplicationUser> GetUserDetails(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Properties)
                .Include(u => u.Bookings)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            if (user != null)
            {
                user.Properties ??= new List<Property>();
                user.Bookings ??= new List<Booking>();
                user.Reviews ??= new List<Review>();
            }

            return user;
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Properties)
                .Include(u => u.Bookings)
                .Include(u => u.Reviews)
                .ToListAsync();

            foreach (var user in users)
            {
                user.Properties ??= new List<Property>();
                user.Bookings ??= new List<Booking>();
                user.Reviews ??= new List<Review>();
            }

            return users;
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            var user = await _context.Users
                .Include(u => u.Properties)
                .Include(u => u.Bookings)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                user.Properties ??= new List<Property>();
                user.Bookings ??= new List<Booking>();
                user.Reviews ??= new List<Review>();
            }

            return user;
        }

        public async Task<bool> VerifyUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.IsVerified = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Here you would typically send this token to the user via email
            // For now, we'll just return true to indicate the token was generated
            return true;
        }

        public async Task<bool> UpdateUser(string userId, ApplicationUser updatedUser)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.ProfilePictureUrl = updatedUser.ProfilePictureUrl;
            user.Role = updatedUser.Role;
            user.IsActive = updatedUser.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
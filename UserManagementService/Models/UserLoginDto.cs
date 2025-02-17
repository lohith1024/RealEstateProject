using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models
{
    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public UserLoginDto()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
    }
}
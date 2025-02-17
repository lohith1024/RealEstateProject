using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagementService.Models;
using UserManagementService.Interfaces;

/// <summary>
/// User Controller for managing user operations.
/// </summary>
namespace UserManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var result = await _userService.RegisterUser(userDto);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("User registration failed.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserDetails(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var result = await _userService.UpdateUserDetails(id, userDto);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            var result = await _userService.VerifyUser(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
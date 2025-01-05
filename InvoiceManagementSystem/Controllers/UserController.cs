using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
     .WriteTo.File("user/auth-log.txt", rollingInterval: RollingInterval.Day)
     .CreateLogger();
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var tokenReplaced = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var users = await _userService.GetAllUsers();
                _logger.Information("Successfully retrieved all users.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving users.");
                return StatusCode(500, "Internal server error while retrieving users.");
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    Log.Warning("User with ID {UserId} not found.", id);
                    return NotFound();
                }
                _logger.Information("Successfully retrieved user with ID {UserId}.", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error while retrieving the user.");
            }
        }

        [AllowAnonymous]
        [HttpGet("{emailOrUsername}")]
        public async Task<IActionResult> GetUserByEmailOrUsername(string emailOrUsername)
        {
            try
            {
                var user = await _userService.GetUserByEmailOrUsername(emailOrUsername);
                if (user == null)
                {
                    _logger.Warning("User with email/username {EmailOrUsername} not found.", emailOrUsername);
                    return NotFound();
                }
                _logger.Information("Successfully retrieved user with email/username {EmailOrUsername}.", emailOrUsername);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving user with email/username {EmailOrUsername}.", emailOrUsername);
                return StatusCode(500, "Internal server error while retrieving user.");
            }
        }

        [AllowAnonymous]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    _logger.Warning("User with ID {UserId} not found.", id);
                    return NotFound();
                }

                await _userService.DeleteUser(id);
                _logger.Information("Successfully deleted user with ID {UserId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deleting user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error while deleting user.");
            }
        }

        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm] UserCreateDTO userCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warning("CreateUser - Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                string profilePicPath = null;

                // Check if a profile picture was uploaded
                if (userCreateDTO.ProfilePicture != null && userCreateDTO.ProfilePicture.Length > 0)
                {
                    // Define the upload folder
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files", "profile-pics");

                    // Check if the directory exists; if not, create it
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate a unique filename for the uploaded file
                    var fileName = $"{Guid.NewGuid()}_{userCreateDTO.ProfilePicture.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userCreateDTO.ProfilePicture.CopyToAsync(stream);
                    }

                    profilePicPath = $"/profile-pics/{fileName}";
                }

                // Create the user object with the provided data
                var user = new User()
                {
                    Username = userCreateDTO.Username,
                    PasswordHash = userCreateDTO.PasswordHash,
                    Email = userCreateDTO.Email,
                    IsActive = true,
                    RoleId = userCreateDTO.RoleId??1,
                    ProfilePictureUrl = profilePicPath
                };

                // Add the user to the service
                await _userService.AddUser(user);
                var createdUser = await _userService.GetUserById(user.UserId);

                if (createdUser == null)
                {
                    _logger.Error("User with ID {UserId} was not found after creation.", user.UserId);
                    throw new Exception($"User with ID {user.UserId} not found after creation.");
                }

                _logger.Information("Successfully created user with ID {UserId}.", createdUser.UserId);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating user.");
                return StatusCode(500, "Internal server error while creating user.");
            }
        }
       
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id, UserCreateDTO userCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warning("UpdateUser - Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                if (id != userCreateDTO.UserId)
                {
                    _logger.Warning("User ID mismatch: provided ID {Id}, expected ID {UserId}.", id, userCreateDTO.UserId);
                    return BadRequest();
                }

                var user = new User()
                {
                    UserId = (int)(userCreateDTO?.UserId),
                    Username = userCreateDTO.Username,
                    PasswordHash = userCreateDTO.PasswordHash,
                    Email = userCreateDTO.Email,
                    IsActive = userCreateDTO.IsActive,
                    RoleId = userCreateDTO.RoleId,
                };

                await _userService.UpdateUser(id, user);
                _logger.Information("Successfully updated user with ID {UserId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating user with ID {UserId}.", id);
                return StatusCode(500, "Internal server error while updating user.");
            }
        }
    }
}

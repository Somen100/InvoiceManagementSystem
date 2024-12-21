using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.BAL.Service;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
     .WriteTo.File("logs/auth-log.txt", rollingInterval: RollingInterval.Day)
     .CreateLogger();

        private readonly JwtTokenService _jwtTokenService;
        private readonly IUserService _userService;
        public readonly IConfiguration _configuration;

        public AuthController(JwtTokenService jwtTokenService, IUserService userService, IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _userService = userService;
            _configuration = configuration;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                // Validate user credentials
                var user = _userService.Authenticate(loginRequest.EmailOrUsername, loginRequest.Password);
                if (user == null) return Unauthorized("Invalid credentials.");

                var roleName = user.Role?.RoleName;

                // Generate JWT token
                var tokenGenerator = new JwtTokenGenerator(_configuration["JwtSettings:Key"], _configuration["JwtSettings:Issuer"]);
                var token = tokenGenerator.GenerateToken(user.Username, roleName);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while fetching invoice with ID {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}

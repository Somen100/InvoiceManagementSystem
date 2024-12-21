using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[Route("api/[controller]")]
[ApiController]
public class RoleMasterController : ControllerBase
{
    private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
        .WriteTo.File("rolemaster/auth-log.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    private readonly IRoleMasterService _roleMasterService;

    // Inject the logger into the constructor
    public RoleMasterController(IRoleMasterService roleMasterService)
    {
        _roleMasterService = roleMasterService;
    }

    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole([FromBody] RoleMasteCreateDTO roleDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new RoleMaster()
            {
                RoleName = roleDTO.RoleName,
                IsActive = true,
            };

            var createdRole = await _roleMasterService.CreateRoleAsync(role);
            if (createdRole == null)
            {
                _logger.Error($"Failed to create role with name: {roleDTO.RoleName}");
                return BadRequest($"Failed to create role with name: {roleDTO.RoleName}");
            }

            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.RoleId }, createdRole);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while creating the role.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }

    [HttpGet("GetRoleById/{id}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        try
        {
            var role = await _roleMasterService.GetRoleByIdAsync(id);
            if (role == null)
            {
                _logger.Warning($"Role with ID {id} not found.");
                return NotFound();
            }
            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An error occurred while retrieving the role with ID {id}.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }

    [HttpGet("GetAllRoles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _roleMasterService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while retrieving all roles.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }

    [HttpPut("UpdateRole/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleMasteCreateDTO roleDTO)
    {
        try
        {
            if (id != roleDTO.RoleId) return BadRequest();

            var role = new RoleMaster()
            {
                RoleId = id,
                RoleName = roleDTO.RoleName,
                IsActive = roleDTO.IsActive,
            };

            var updatedRole = await _roleMasterService.UpdateRoleAsync(role);
            if (updatedRole == null)
            {
                _logger.Warning($"Role with ID {id} not found for update.");
                return NotFound();
            }
            return Ok(updatedRole);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An error occurred while updating the role with ID {id}.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }

    [HttpDelete("DeleteRole/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        try
        {
            var role = await _roleMasterService.GetRoleByIdAsync(id);
            if (role == null)
            {
                _logger.Warning($"Role with ID {id} not found for deletion.");
                return NotFound();
            }

            await _roleMasterService.DeleteRoleAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An error occurred while deleting the role with ID {id}.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }
}

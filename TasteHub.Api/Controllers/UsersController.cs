using Microsoft.AspNetCore.Mvc;
using TasteHub.Api.DTOs;
using TasteHub.Services;

namespace TasteHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterUserDto dto)
    {
        try
        {
            var user = _userService.RegisterUser(
                dto.Name,
                dto.Email,
                dto.Password
            );

            return StatusCode(201, new
            {
                user.Id,
                user.Name,
                user.Email
            });
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
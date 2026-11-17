using Microsoft.AspNetCore.Mvc;
using TasteHub.Api.DTOs;
using TasteHub.Api.Services;
using TasteHub.Services;

namespace TasteHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public UsersController(
        UserService userService,
        JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
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

    [HttpPost("login")]
    public IActionResult Login(LoginUserDto dto)
    {
        var user = _userService.ValidateUser(dto.Email, dto.Password);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Email ou palavra-passe incorretos."
            });
        }

        string token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role
            }
        });
    }
}
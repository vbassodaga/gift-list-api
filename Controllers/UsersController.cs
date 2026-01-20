using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HousewarmingRegistry.API.Data;
using HousewarmingRegistry.API.DTOs;
using HousewarmingRegistry.API.Models;

namespace HousewarmingRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegistryDbContext _context;

    public UsersController(RegistryDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerDto)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(registerDto.FirstName))
        {
            return BadRequest("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.LastName))
        {
            return BadRequest("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.PhoneNumber))
        {
            return BadRequest("Phone number is required.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
        {
            return BadRequest("Password must be at least 6 characters long.");
        }

        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            return BadRequest("Password and confirm password do not match.");
        }

        // Check if phone number already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);

        if (existingUser != null)
        {
            return BadRequest("A user with this phone number already exists.");
        }

        // Create new user
        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            PhoneNumber = registerDto.PhoneNumber.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Role = user.Role,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.PhoneNumber) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest("Phone number and password are required.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.PhoneNumber.Trim());

        if (user == null)
        {
            return Unauthorized("Invalid phone number or password.");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid phone number or password.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Role = user.Role,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt
        };

        return Ok(userDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Role = user.Role,
            IsAdmin = user.IsAdmin,
            CreatedAt = user.CreatedAt
        };

        return Ok(userDto);
    }
}


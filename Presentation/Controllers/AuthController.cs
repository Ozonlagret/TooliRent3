using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TooliRentDbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TooliRentDbContext dbContext,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Optionally assign default role
            await _userManager.AddToRoleAsync(user, "Member");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials.");

            var accessToken = await GenerateJwtToken(user);
            var refreshToken = await CreateAndSaveRefreshTokenAsync(user.Id);

            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

            if (storedToken == null)
                return Unauthorized("Invalid or expired refresh token.");

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return Unauthorized();

            // Revoke old token
            storedToken.IsRevoked = true;
            await _dbContext.SaveChangesAsync();

            // Issue new tokens
            var accessToken = await GenerateJwtToken(user);
            var newRefreshToken = await CreateAndSaveRefreshTokenAsync(user.Id);

            return Ok(new { accessToken, refreshToken = newRefreshToken });
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        {
        }
    }
}

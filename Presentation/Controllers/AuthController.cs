using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto.UserName, dto.Email, dto.Password);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto.UserName, dto.Password);

            if (!result.IsSuccess)
                return Unauthorized(new { errors = result.Errors });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest dto)
        {
            var success = await _authService.LogoutAsync(dto.RefreshToken);
            if (!success)
                return BadRequest(new { message = "Invalid refresh token." });

            return Ok(new { message = "Logged out successfully." });
        }

        [Authorize(Policy = "ActiveValidUser")]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if (!result.IsSuccess)
                return Unauthorized(new { errors = result.Errors });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser([FromRoute] string userId)
        {
            var success = await _authService.DeactivateUserAsync(userId);
            if (!success)
                return BadRequest(new { message = "Could not deactivate user." });
            return Ok(new { message = "User deactivated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("activate/{userId}")]
        public async Task<IActionResult> ActivateUser([FromRoute] string userId)
        {
                        var success = await _authService.ActivateUserAsync(userId);
            if (!success)
                return BadRequest(new { message = "Could not activate user." });
            return Ok(new { message = "User activated successfully." });
        }
    }
}

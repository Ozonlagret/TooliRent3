using Application.Services;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("public/auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            { 
                return BadRequest(new { message = "UserName, Email, and Password are required." }); 
            }

            var result = await _authService.RegisterAsync(dto.UserName, dto.Email, dto.Password);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Errors?.FirstOrDefault() ?? "Registration failed." });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("public/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "UserName and Password are required." });
            

            var result = await _authService.LoginAsync(dto.UserName, dto.Password);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Errors?.FirstOrDefault() ?? "Login failed." });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [Authorize]
        [HttpPost("member/auth/logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Refresh token is required." });
            

            var success = await _authService.LogoutAsync(dto.RefreshToken);
            if (!success)
                return BadRequest(new { message = "Invalid refresh token." });

            return Ok(new { message = "Logged out successfully." });
        }

        [AllowAnonymous]
        [HttpPost("public/auth/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Refresh token is required." });

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Errors?.FirstOrDefault() ?? "Refresh token failed." });

            return Ok(new
            {
                token = result.Token,
                refreshToken = result.RefreshToken
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/users/by-username/{userName}/deactivate")]
        public async Task<IActionResult> DeactivateUserByUserName([FromRoute] string userName)
        {
            var success = await _authService.DeactivateUserByUserNameAsync(userName);

            if (!success)
                return NotFound(new { message = "User not found or could not be deactivated." });

            return Ok(new { message = "User deactivated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/users/by-username/{userName}/activate")]
        public async Task<IActionResult> ActivateUserByUserName([FromRoute] string userName)
        {
            var success = await _authService.ActivateUserByUserNameAsync(userName);
            if (!success)
                return NotFound(new { message = "User not found or could not be activated." });

            return Ok(new { message = "User activated successfully." });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/by-username/{userName}")]
        public async Task<IActionResult> DeleteUserByUserName([FromRoute] string userName)
        {
            var success = await _authService.DeleteUserByUserNameAsync(userName);
            if (!success)
                return NotFound(new { message = "User not found or could not be deleted." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}

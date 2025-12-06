using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Responses;
using Application.Interfaces.Service;
using Application.Interfaces.Repository;

namespace Application.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponse> RegisterAsync(string userName, string email, string password)
        {
            var user = new IdentityUser { UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new AuthResponse { IsSuccess = false, Errors = errors };
            }
            await _userManager.AddToRoleAsync(user, "Member");
            return new AuthResponse { IsSuccess = true };
        }

        public async Task<AuthResponse> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "Invalid username or password." } };
            }
            var token = await GenerateJwtToken(user);
            return new AuthResponse { IsSuccess = true, Token = token };
        }

        public async Task<AuthResponse> RevokeRefreshToken(string userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByUserIdAsync(userId);
            if (refreshToken == null)
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "No valid refresh token found." } };
            }
            refreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
            return new AuthResponse { IsSuccess = true };
        }
    }


}

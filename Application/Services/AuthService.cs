using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Responses;
using Application.Interfaces.Service;
using Application.Interfaces.Repository;
using Domain.Models;

namespace Application.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _config = config;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
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
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new AuthResponse { IsSuccess = false, Errors = errors };
            }
            await _userManager.AddToRoleAsync(user, "Member");

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResponse
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "Invalid username or password." } };
            }

            if (!user.IsActive)
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "Account is deactivated." } };
            }

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new AuthResponse
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked)
                return false;

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.Expires <= DateTime.UtcNow)
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "Invalid or expired refresh token." } };
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "User not found." } };
            }

            if (!user.IsActive)
            {
                return new AuthResponse { IsSuccess = false, Errors = new List<string> { "Account is deactivated." } };
            }

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user.Id, newRefreshToken);

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                IsSuccess = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(token);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

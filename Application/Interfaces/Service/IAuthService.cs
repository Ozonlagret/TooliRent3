using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string userName, string email, string password);
        Task<AuthResponse> LoginAsync(string userName, string password);
        Task<bool> LogoutAsync(string refreshToken);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
    }
}

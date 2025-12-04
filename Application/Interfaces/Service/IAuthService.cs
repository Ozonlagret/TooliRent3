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
    }
}

using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Mappers;

public static class UserMapper
{
    public static AdminUserSummaryResponse ToAdminSummary(ApplicationUser user) => new()
    {
        Id = user.Id,
        UserName = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        IsActive = user.IsActive
    };
}
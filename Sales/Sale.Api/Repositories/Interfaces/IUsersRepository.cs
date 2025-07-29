using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
        Task<User> GetUserAsync(string email);
        Task<User> GetUserAsync(Guid userId);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(LoginDTO model);
        Task LogoutAsync();
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task SaveRefreshTokenAsync(User user, TokenDTO tokenDTO);
        Task<ActionResponse<string?>> ValidateRefreshTokenAsync(string refreshToken);
        Task<ActionResponse<bool>> RevokeRefreshTokenAsync(TokenDTO tokenDTO);
        Task RevokeAllRefreshTokensAsync(User user);
    }
}

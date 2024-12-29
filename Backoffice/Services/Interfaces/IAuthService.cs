using Shared.Models;
using System.Security.Claims;

namespace Backoffice.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateLoginAsync(string email, string password);
        Task<bool> RegisterUserAsync(User user);
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal userClaims);
        Task SignInAsync(User user, HttpContext httpContext);
        Task SignOutAsync(HttpContext httpContext);
    }
}

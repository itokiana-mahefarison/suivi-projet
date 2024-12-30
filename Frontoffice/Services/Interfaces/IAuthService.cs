using Shared.Models;

namespace Frontoffice.Services.Interfaces
{
    public interface IAuthService
    {
        User? ValidateUser(string email, string password);
    }
}

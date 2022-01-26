using System.Threading.Tasks;

namespace IceSync.Infrastructure.Services
{
    /// <summary>The authentication service interface.</summary>
    public interface IAuthenticationService
    {
        /// <summary>Gets the token.</summary>
        Task<string> GetTokenAsync();
    }
}

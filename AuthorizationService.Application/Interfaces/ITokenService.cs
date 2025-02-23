using AuthorizationService.Domain.Models;

namespace AuthorizationService.Application.Interfaces
{
	public interface ITokenService
	{
        string GenerateJwtToken(User user);
    }
}


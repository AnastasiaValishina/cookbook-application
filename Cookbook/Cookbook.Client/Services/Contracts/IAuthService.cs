using Cookbook.Models.Dtos;

namespace Cookbook.Client.Services.Contracts
{
	public interface IAuthService
	{
		ValueTask<string> GetJwtAsync();
		Task LogoutAsync();
		Task RegisterUser(UserForRegistrationDto userForRegistration);
		Task<DateTime> LoginAsync(UserForLoginDto userForLogin);
	}
}

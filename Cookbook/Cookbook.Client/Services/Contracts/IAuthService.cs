using Cookbook.Models.Dtos;

namespace Cookbook.Client.Services.Contracts
{
	public interface IAuthService
	{
		Task RegisterUser(UserForRegistrationDto userForRegistration);
		Task LoginAsync(UserForLoginDto userForLogin);
	}
}

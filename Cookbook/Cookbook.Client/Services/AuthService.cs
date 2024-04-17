using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using System.Net.Http;
using System.Net.Http.Json;

namespace Cookbook.Client.Services
{
    public class AuthService : IAuthService
    {
		private readonly HttpClient _httpClient;

		public AuthService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task RegisterUser(UserForRegistrationDto userForRegistration)
        {
			try
			{
				var response = await _httpClient.PostAsJsonAsync<UserForRegistrationDto>("Auth/Register", userForRegistration);

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return;
					}
					throw new Exception($"Failed to create user. Status code: {response.StatusCode}");
				}
				else
				{
					var message = await response.Content.ReadAsStringAsync();
					throw new Exception($"Http status: {response.StatusCode} Message - {message}");
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}
    }
}

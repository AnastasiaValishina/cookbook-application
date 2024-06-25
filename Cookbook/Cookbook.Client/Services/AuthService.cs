using Blazored.SessionStorage;
using Cookbook.Api.Models;
using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace Cookbook.Client.Services
{
	public class AuthService : IAuthService
	{
		private readonly IHttpClientFactory _factory;
		private readonly ISessionStorageService _sessionStorageService;
		private const string JWT_KEY = nameof(JWT_KEY);
		private string? _jwtCache;

		public event Action<string?>? LoginChange; // username or null

		public AuthService(IHttpClientFactory factory, ISessionStorageService sessionStorageService)
		{
			_factory = factory;			
			_sessionStorageService = sessionStorageService;
		}

		public async ValueTask<string> GetJwtAsync()
		{
			if (string.IsNullOrEmpty(_jwtCache))
				_jwtCache = await _sessionStorageService.GetItemAsync<string>(JWT_KEY);

			return _jwtCache;
		}

		public async Task LogoutAsync()
		{
			await _sessionStorageService.RemoveItemAsync(JWT_KEY);

			_jwtCache = null;

			LoginChange?.Invoke(null);
		}

		public async Task RegisterUser(UserForRegistrationDto userForRegistration)
		{
			try
			{
				var response = await _factory.CreateClient("ServerApi")
					.PostAsJsonAsync("Auth/Register", userForRegistration);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				throw new Exception(ex.Message);
			}
		}

		public async Task<DateTime> LoginAsync(UserForLoginDto userForLogin)
		{			
			var response = await _factory.CreateClient("ServerApi").PostAsync("Auth/Login", JsonContent.Create(userForLogin));

			if (!response.IsSuccessStatusCode)
				throw new UnauthorizedAccessException("Login failed.");

			var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

			if (content == null)
				throw new InvalidDataException();

			await _sessionStorageService.SetItemAsync(JWT_KEY, content.JwtToken);

			var userId = GetUserId(content.JwtToken);

			var user = await GetUser(userId);

			LoginChange?.Invoke(user.UserName);

			return content.Expiration;
		}

        public async Task<bool> RefreshTokenAsync()
        {
            var response = await _factory.CreateClient("ServerApi").GetAsync("Auth/RefreshToken");

			if (!response.IsSuccessStatusCode)
			{
				await LogoutAsync();
				return false;
			}

            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (content == null)
                throw new InvalidDataException();

            await _sessionStorageService.SetItemAsync(JWT_KEY, content.JwtToken);

			_jwtCache = content.JwtToken;

            return true;
        }

        private static string? GetUserId(string token)
		{
			var jwt = new JwtSecurityToken(token);

			var userIdClaim = jwt.Claims.FirstOrDefault(claim => claim.Type == "userId");

			if (userIdClaim != null)
				return userIdClaim.Value;

			return null;
		}

		private async Task<User>? GetUser(string id)
		{
			try
			{
				var response = await _factory.CreateClient("ServerApi")
					.GetAsync($"User/GetUser/{id}");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
					{
						return default;
					}

					return await response.Content.ReadFromJsonAsync<User>();
				}
				else
				{
					var message = await response.Content.ReadAsStringAsync();
					throw new Exception(message);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				throw new Exception(ex.Message);
			}

		}
	}
}

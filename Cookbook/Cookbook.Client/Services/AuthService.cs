using Blazored.SessionStorage;
using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Cookbook.Client.Services
{
	public class AuthService : IAuthService
	{
		private readonly HttpClient _httpClient;
		private ISessionStorageService _sessionStorageService;
		private const string JWT_KEY = nameof(JWT_KEY);
		private string? _jwtCache;

		public event Action<string?>? LoginChange; // username or null

		public AuthService(HttpClient httpClient, ISessionStorageService sessionStorageService)
		{
			_httpClient = httpClient;
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
				var response = await _httpClient.PostAsJsonAsync<UserForRegistrationDto>("Auth/Register", userForRegistration);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				throw new Exception(ex.Message);
			}
		}

		public async Task<DateTime> LoginAsync(UserForLoginDto userForLogin)
		{
			//var response = await _httpClient.PostAsJsonAsync<UserForLoginDto>("Auth/Login", userForLogin);
			var response = await _httpClient.PostAsync("Auth/Login", JsonContent.Create(userForLogin));

			if (!response.IsSuccessStatusCode)
				throw new UnauthorizedAccessException("Login failed.");

			var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

			if (content == null)
				throw new InvalidDataException();

			await _sessionStorageService.SetItemAsync(JWT_KEY, content.JwtToken);

			LoginChange?.Invoke(GetUsername(content.JwtToken));

			return content.Expiration;
		}

		private static string GetUsername(string token)
		{
			var jwt = new JwtSecurityToken(token);

			return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
		}
	}
}

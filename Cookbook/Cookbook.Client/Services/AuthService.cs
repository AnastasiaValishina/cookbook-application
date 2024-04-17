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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                throw new Exception(ex.Message);
            }
        }

        public async Task LoginAsync(UserForLoginDto userForLogin)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<UserForLoginDto>("Auth/Login", userForLogin);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                throw new Exception(ex.Message);
            }
        }
    }
}

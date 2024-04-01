using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using System.Net.Http.Json;

namespace Cookbook.Client.Services
{
	public class RecipeService: IRecipeService
	{
		private readonly HttpClient _httpClient;

		public RecipeService(HttpClient httpClient)
        {
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<RecipeDto>> GetRecipesAsync()
		{
			try
			{
				var responce = await _httpClient.GetAsync("Recipe/RecipesAsync");

				if (responce.IsSuccessStatusCode)
				{
					if(responce.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return Enumerable.Empty<RecipeDto>();
					}

					return await responce.Content.ReadFromJsonAsync<IEnumerable<RecipeDto>>();
				}
				else
				{
					var message = await responce.Content.ReadAsStringAsync();
					throw new Exception(message);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				throw new Exception(ex.Message);
			}
		}

		public async Task<RecipeDto> GetRecipeByIdAsync(int id)
		{
			try
			{
				var response = await _httpClient.GetAsync($"Recipe/RecipeByIdAsync/{id}");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return default(RecipeDto);
					}

					return await response.Content.ReadFromJsonAsync<RecipeDto>();
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

		public async Task<RecipeDto> AddRecipeAsync(RecipeToAddDto recipeToAddDto)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync<RecipeToAddDto>("Recipe/AddRecipeAsync", recipeToAddDto);

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						return default(RecipeDto);
					}

					return await response.Content.ReadFromJsonAsync<RecipeDto>();
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

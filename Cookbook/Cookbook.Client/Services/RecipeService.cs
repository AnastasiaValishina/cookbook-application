using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace Cookbook.Client.Services
{
	public class RecipeService: IRecipeService
	{
		private readonly HttpClient _httpClient;

		public RecipeService(IHttpClientFactory factory)
        {
			_httpClient = factory.CreateClient("ServerApi");
		}

		public async Task<IEnumerable<RecipeDto>> GetRecipesAsync()
		{
			try
			{
				var response = await _httpClient.GetAsync("Recipe/MyRecipes/0");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
					{
						return Enumerable.Empty<RecipeDto>();
					}

					return await response.Content.ReadFromJsonAsync<IEnumerable<RecipeDto>>();
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

		public async Task<RecipeDto> GetRecipeByIdAsync(int id)
		{
			try
			{
				var response = await _httpClient.GetAsync($"Recipe/MyRecipes/{id}");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
					{
						return default(RecipeDto);
					}

					var recipes = await response.Content.ReadFromJsonAsync<List<RecipeDto>>();
					return recipes.FirstOrDefault();
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
					if (response.StatusCode == HttpStatusCode.NoContent)
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

		public async Task<RecipeDto> DeleteRecipe(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"Recipe/DeleteRecipeAsync/{id}");

				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadFromJsonAsync<RecipeDto>();
				}
				else if (response.StatusCode == HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw new Exception($"Failed to delete recipe. Status code: {response.StatusCode}");
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Failed to communicate with the API.", ex);
			}
		}

		public async Task EditRecipe(RecipeToEditDto recipeToEditDto)
		{
			try
			{
				var response = await _httpClient.PutAsJsonAsync($"Recipe/UpdateAsync/", recipeToEditDto);

				if (!response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NotFound)
					{
						return; 
					}
					else
					{
						throw new Exception($"Failed to update recipe. Status code: {response.StatusCode}");
					}
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Failed to communicate with the API.", ex);
			}
		}

		public async Task<IEnumerable<RecipeDto>> GetRecipeBySearchAsync(string searchParam)
		{				
			try
			{
				var response = await _httpClient.GetAsync($"Recipe/RecipesBySearchParam/{searchParam}");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
					{
						return Enumerable.Empty<RecipeDto>();
					}

					return await response.Content.ReadFromJsonAsync<IEnumerable<RecipeDto>>();
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

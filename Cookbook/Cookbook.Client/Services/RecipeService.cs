using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using System.Net.Http.Json;
using System.Net;

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

		public async Task<RecipeDto> DeleteRecipe(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"Recipe/DeleteRecipeAsync{id}");

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

		public async Task<RecipeDto> EditRecipe(RecipeToEditDto recipeToEditDto)
		{
			try
			{
				var response = await _httpClient.PutAsJsonAsync($"Recipe/UpdateAsync/", recipeToEditDto);

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
					throw new Exception($"Failed to update recipe. Status code: {response.StatusCode}");
				}
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Failed to communicate with the API.", ex);
			}
		}
	}
}

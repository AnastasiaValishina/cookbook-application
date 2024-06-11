using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace Cookbook.Client.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IHttpClientFactory _factory;

        public CategoryService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
		{
			try
			{
				var response = await _factory.CreateClient("ServerApi").GetAsync("Category/GetCategories");

				if (response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.NoContent)
					{
						return Enumerable.Empty<Category>();
					}

					return await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
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

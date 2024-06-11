using Cookbook.Models.Dtos;

namespace Cookbook.Client.Services.Contracts
{
	public interface ICategoryService
	{
		Task<IEnumerable<Category>> GetCategoriesAsync();
	}
}

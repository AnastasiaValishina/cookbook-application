using Cookbook.Models.Dtos;

namespace Cookbook.Client.Services.Contracts
{
	public interface IRecipeService
	{
		Task<IEnumerable<RecipeDto>> GetRecipesAsync();
		Task<RecipeDto> GetRecipeByIdAsync(int id);
		Task<IEnumerable<RecipeDto>> GetRecipeBySearchAsync(string searchParam);
		Task<RecipeDto> AddRecipeAsync(RecipeToAddDto recipeToAddDto);
		Task<RecipeDto> DeleteRecipe(int id);
		Task<RecipeDto> EditRecipe(RecipeToEditDto recipeToEditDto);
	}
}

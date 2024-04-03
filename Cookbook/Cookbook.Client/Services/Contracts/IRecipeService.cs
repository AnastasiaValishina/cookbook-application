using Cookbook.Models.Dtos;

namespace Cookbook.Client.Services.Contracts
{
	public interface IRecipeService
	{
		Task<IEnumerable<RecipeDto>> GetRecipesAsync();
		Task<RecipeDto> GetRecipeByIdAsync(int id);
		Task<RecipeDto> AddRecipeAsync(RecipeToAddDto recipeToAddDto);
		Task<RecipeDto> DeleteRecipe(int id);
	}
}

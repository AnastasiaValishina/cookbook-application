using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class AddRecipeBase : ComponentBase
	{
		[Inject]
		public IRecipeService RecipeService { get; set; }
		
		[Inject]
		public NavigationManager NavigationManager { get; set; }

		protected RecipeToAddDto RecipeToAdd = new RecipeToAddDto();

		protected async Task AddRecipe_Click()
		{
			try
			{
				RecipeDto recipe = await RecipeService.AddRecipeAsync(RecipeToAdd);
				NavigationManager.NavigateTo($"/RecipeDetails/{recipe.RecipeId}");
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		protected void AddNewIngredient()
		{
            RecipeToAdd.Ingredients.Add(new IngredientToAddDto());
		}
	}
}

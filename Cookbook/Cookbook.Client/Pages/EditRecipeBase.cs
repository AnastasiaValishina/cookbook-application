using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class EditRecipeBase : ComponentBase
	{
		[Parameter]
		public int Id { get; set; }

		[Inject]
		public required IRecipeService RecipeService { get; set; }

		[Inject]
		public required ICategoryService CategoryService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		public RecipeDto? Recipe { get; set; } = new RecipeDto();
		public string? ErrorMessage { get; set; }
		protected IEnumerable<Category>? categories;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				Recipe = await RecipeService.GetRecipeByIdAsync(Id);
				categories = await CategoryService.GetCategoriesAsync();
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected async Task EditRecipe_Click()
		{
			try
			{
				await RecipeService.EditRecipe(new RecipeToEditDto
				{
					RecipeId = Recipe!.RecipeId,
					Title = Recipe.Title,
					Notes = Recipe.Notes,
					CategoryId = Recipe.CategoryId,
					Ingredients = Recipe.Ingredients,
					Source = Recipe.Source
				});
				NavigationManager.NavigateTo($"/RecipeDetails/{Id}");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected async Task DeleteRecipe_Click()
		{
			try
			{
				await RecipeService.DeleteRecipe(Id);
				NavigationManager.NavigateTo("/Recipes");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected void AddNewIngredient_Click()
		{
			Recipe?.Ingredients.Add(new IngredientDto());
		}

		protected async Task DeleteIngredient_Click(IngredientDto ingredientToDelete)
		{
			Recipe?.Ingredients.Remove(ingredientToDelete);
		}
	}
}

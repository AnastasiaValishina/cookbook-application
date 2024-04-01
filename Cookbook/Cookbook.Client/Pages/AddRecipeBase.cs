using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class AddRecipeBase : ComponentBase
	{
		[Inject]
		public IRecipeService RecipeService { get; set; }
		protected string Title { get; set; } = "";
		protected string Notes { get; set; } = "";
		protected int CategoryId { get; set; }
		protected List<IngredientToAddDto> Ingredients { get; set; } = new List<IngredientToAddDto>();
		protected string Source { get; set; } = "";

		protected async Task AddRecipe_Click(RecipeToAddDto recipeToAddDto)
		{
			try
			{
				await RecipeService.AddRecipeAsync(recipeToAddDto);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		protected void AddIngredient(string name, int qty, string unit)
		{
			Ingredients.Add(new IngredientToAddDto { Name = name, Qty = qty, Unit = unit });
		}
		protected void AddNewIngredient()
		{
			Ingredients.Add(new IngredientToAddDto());
		}
	}
}

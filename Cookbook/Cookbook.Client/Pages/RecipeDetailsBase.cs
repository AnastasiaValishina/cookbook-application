using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class RecipeDetailsBase : ComponentBase
	{
		[Parameter]
		public int Id { get; set; }
		
		[Inject]
		public IRecipeService RecipeService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }
		public RecipeDto? Recipe { get; set; }
		public string? ErrorMessage { get; set; }
		protected override async Task OnInitializedAsync()
		{
			try
			{
				Recipe = await RecipeService.GetRecipeByIdAsync(Id);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected string GetIngredientDetails(IngredientDto ingredient)
		{
			return $"{ingredient.Qty} {ingredient.Unit} {ingredient.Name}";
		}

		protected async Task DeleteRecipe_Click()
		{
			try
			{
				await RecipeService.DeleteRecipe(Id);
				NavigationManager.NavigateTo("/");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected void EditRecipe_Click()
		{
			NavigationManager.NavigateTo($"/EditRecipe/{Id}");
		}

	}
}

using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Cookbook.Client.Pages
{
	public class RecipeDetailsBase : ComponentBase
	{
		[Parameter]
		public int Id { get; set; }
		
		[Inject]
		public required IRecipeService RecipeService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }
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

		protected string GetIngredientQty(IngredientDto ingredient)
		{
			if (ingredient.Qty == 0) 
				return "";

			return ingredient.Qty.ToString();
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

		protected void EditRecipe_Click()
		{
			NavigationManager.NavigateTo($"/EditRecipe/{Id}");
		}

		protected bool IsValidUrl(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return false;

			string pattern = @"^(http|https):\/\/[^\s/$.?#].[^\s]*$";
			return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
		}
	}
}

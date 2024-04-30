using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class RecipesBase : ComponentBase
	{
		[Inject]
		public IRecipeService RecipeService { get; set; }
		
		[Inject]
		public NavigationManager NavigationManager { get; set; }

		public IEnumerable<RecipeDto>? Recipes { get; set; }
		protected string SearchText { get; set; } = string.Empty;
		public string? ErrorMessage { get; set; }

		protected override async Task OnInitializedAsync()
		{
			try
			{
				ErrorMessage = "Loading...";

				Recipes = await RecipeService.GetRecipesAsync();

				ErrorMessage = null;
			}
			catch 
			{
				ErrorMessage = "Failed to retrieve data.";
			}
		}

		protected IOrderedEnumerable<IGrouping<int, RecipeDto>> GetGroupedByCategory()
		{
			return from recipe in Recipes
				   group recipe by recipe.CategoryId into prodByCatGroup
				   orderby prodByCatGroup.Key
				   select prodByCatGroup;
		}

		protected string GetCategoryName(IGrouping<int, RecipeDto> groupedRecipeDtos)
		{
			return groupedRecipeDtos.FirstOrDefault(pg => pg.CategoryId == groupedRecipeDtos.Key).CategoryName;
		}

		protected async Task AddNewRecipe_Click()
		{
			NavigationManager.NavigateTo("/AddRecipe");
		}

		protected async Task Search_Click()
		{
			if (SearchText == string.Empty) return;

			Recipes = await RecipeService.GetRecipeBySearchAsync(SearchText);
		}
	}
}

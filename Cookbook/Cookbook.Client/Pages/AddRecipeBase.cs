﻿using Cookbook.Models.Dtos;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class AddRecipeBase : ComponentBase
	{
		[Inject]
		public required IRecipeService RecipeService { get; set; }

		[Inject]
		public required ICategoryService CategoryService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		protected RecipeToAddDto RecipeToAdd = new RecipeToAddDto { CategoryId = 1 };

		protected IEnumerable<Category>? categories;
		public string? ErrorMessage { get; set; }

		protected override async Task OnInitializedAsync()
		{
			AddNewIngredient();
			try
			{
				categories = await CategoryService.GetCategoriesAsync();
				ErrorMessage = null;
			}
			catch 
			{
				ErrorMessage = "Failed to retrieve available categories";
			}
		}

		protected async Task AddRecipe_Click()
		{
			try
			{
				RecipeDto? recipe = await RecipeService.AddRecipeAsync(RecipeToAdd);
				NavigationManager.NavigateTo($"/RecipeDetails/{recipe?.RecipeId}");
				ErrorMessage = null;
			}
			catch
			{
				ErrorMessage = "Failed to add recipe";
			}
		}

		protected void AddNewIngredient()
		{
            RecipeToAdd.Ingredients.Add(new IngredientToAddDto());
		}

		protected async Task DeleteIngredient_Click(IngredientToAddDto ingredientToAddDto)
		{
			RecipeToAdd.Ingredients.Remove(ingredientToAddDto);
		}
	}
}

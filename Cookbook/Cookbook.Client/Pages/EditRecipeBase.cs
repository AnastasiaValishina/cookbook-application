﻿using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class EditRecipeBase : ComponentBase
	{
		[Parameter]
		public int Id { get; set; }

		[Inject]
		public IRecipeService RecipeService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }
		public RecipeDto Recipe { get; set; }

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

		protected async Task EditRecipe_Click(RecipeToEditDto recipeToEditDto)
		{
			try
			{
				await RecipeService.EditRecipe(recipeToEditDto);
				NavigationManager.NavigateTo($"/RecipeDetails/{Id}");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}

		protected void AddNewIngredient_Click()
		{
			Recipe.Ingredients.Add(new IngredientDto());
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

		protected async Task DeleteIngredient_Click(IngredientDto ingredientToDelete)
		{
			Recipe.Ingredients.Remove(ingredientToDelete);
		}
	}
}

﻿using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RecipeController : ControllerBase
	{
		private readonly DataContextDapper _dapper;

		public RecipeController(IConfiguration config)
		{
			_dapper = new DataContextDapper(config);
		}
		/*
				[HttpGet("Connection")]
				public DateTime Connection()
				{
					return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
				}*/

		[HttpGet("RecipesBySearchParam/{searchParam}")]
		public async Task<IEnumerable<RecipeDto>> GetRecipesBySearchParam(string searchParam)
		{
			string sql = $"EXEC CookbookAppSchema.spRecipes_GetBySearch @SearchParam = {searchParam}";

			var recipes = await _dapper.LoadDataAsync<RecipeDto>(sql);

			foreach (var recipe in recipes)
			{
				var ingredients = await GetIngredients(recipe.RecipeId);
				recipe.Ingredients = ingredients.ToList();
			}

			return recipes;
		}

		[HttpGet("RecipesByUserAsync/{userId}")]
		public async Task<IEnumerable<RecipeDto>> GetRecipesByUserAsync(int userId)
		{
			string sql = "EXEC CookbookAppSchema.spRecipes_Get @UserId = " + userId.ToString();

			var recipes = await _dapper.LoadDataAsync<RecipeDto>(sql);

			foreach (var recipe in recipes)
			{
				var ingredients = await GetIngredients(recipe.RecipeId);
				recipe.Ingredients = ingredients.ToList();
			}

			return recipes;
		}

		[HttpGet("RecipeByIdAsync/{recipeId}")]
		public async Task<RecipeDto> GetRecipeByIdAsync(int recipeId)
		{
			string sql = "EXEC CookbookAppSchema.spRecipes_Get @RecipeId = " + recipeId.ToString();

			var recipe = await _dapper.LoadDataSingleAsync<RecipeDto>(sql);

			var ingredients = await GetIngredients(recipe.RecipeId);
			recipe.Ingredients = ingredients.ToList();

			return recipe;
		}

		[HttpPost("AddRecipeAsync")]
		public async Task<ActionResult<RecipeDto>> AddRecipeAsync(RecipeToAddDto recipe)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipe_Add 
				@UserId = " + 101 // this.User.FindFirst("userId")?.Value
				+ ", @Title = '" + recipe.Title
				+ "', @Notes = '" + recipe.Notes 
				+ "', @CategoryId = " + recipe.CategoryId
				+ ", @Source = '" + recipe.Source + "';";

			int recipeId = await _dapper.ExecuteSqlWithIdAsync(sql);

			if (recipeId != 0)
			{
				foreach (var ingredient in recipe.Ingredients)
				{
					string ingredientSql = @"EXEC CookbookAppSchema.spIngredient_Upsert 
						@RecipeId = " + recipeId
						+ ", @Name = '" + ingredient.Name
						+ "', @Qty = '" + ingredient.Qty
						+ "', @Unit = '" + ingredient.Unit
						+ "'";

					await _dapper.ExecuteSqlAsync(ingredientSql);
				}

				RecipeDto createdRecipe = await GetRecipeByIdAsync(recipeId);

				if (createdRecipe != null)
				{
					return Ok(createdRecipe);
				}
				else
				{
					return BadRequest("Failed to retrieve created recipe");
				}
			}
			else
			{
				return BadRequest("Failed to add recipe");
			}
		}

		[HttpPut("UpdateAsync")]
		public async Task<RecipeDto> UpdateAsync(RecipeToEditDto recipeToEdit)
		{
			// update recipe
			string updateSql = @"EXEC CookbookAppSchema.spRecipes_Update 
				@RecipeId = " + recipeToEdit.RecipeId.ToString() +
				", @Title = '" + recipeToEdit.Title + 
				"', @Notes = '" + recipeToEdit.Notes + 
				"', @CategoryId = " + recipeToEdit.CategoryId.ToString() + 
				", @Source = '" + recipeToEdit.Source + "'"; 

			await _dapper.ExecuteSqlAsync(updateSql);

			// delete ingredients
			var oldIngredients = await GetIngredients(recipeToEdit.RecipeId);

			List<int> newIds = new List<int>();
			foreach (var newIng in recipeToEdit.Ingredients)
			{
				newIds.Add(newIng.IngredientId);
			}
			foreach (var ingredient in oldIngredients)
			{
				if (!newIds.Contains(ingredient.IngredientId))
					await DeleteIngredientAsync(ingredient.IngredientId);
			}

			// update/add ingredients 
			foreach (var ingredient in recipeToEdit.Ingredients)
			{
				string ingSql = @"EXEC CookbookAppSchema.spIngredient_Upsert 
					@IngredientId = " + ingredient.IngredientId + 
					", @RecipeId = " + recipeToEdit.RecipeId + 
					", @Name = '" + ingredient.Name + 
					"', @Qty = " + ingredient.Qty + 
					", @Unit = '" + ingredient.Unit + "'";

				await _dapper.ExecuteSqlAsync(ingSql);
			}

			var updatedRecipe = await GetRecipeByIdAsync(recipeToEdit.RecipeId);

			return updatedRecipe;
		}

		[HttpDelete("DeleteRecipeAsync/{recipeId}")]
		public async Task<ActionResult<RecipeDto>> DeleteRecipeAsync(int recipeId)
		{
			try
			{
				var recipeToDelete = await GetRecipeByIdAsync(recipeId);

				if (recipeToDelete != null)
				{
					string sql = $"EXEC CookbookAppSchema.spRecipes_Delete @RecipeId = {recipeId}, @UserId = {101}";
					//this.User.FindFirst("userId")?.Value;

					if (await _dapper.ExecuteSqlAsync(sql))
					{
						return Ok(recipeToDelete);
					}
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to delete recipe: {ex.Message}");
			}
		}

		private async Task<IActionResult> DeleteIngredientAsync(int ingredientId) // перенести в хелпер
		{
			try
			{
				string sql = "EXEC CookbookAppSchema.spIngredient_Delete @IngredientId = " + ingredientId;

				if (await _dapper.ExecuteSqlAsync(sql))
				{
					return Ok();
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to delete ingredient: {ex.Message}");
			}
		}

		private async Task<IEnumerable<IngredientDto>> GetIngredients(int recipeId) // перенести в хелпер
		{
			string iSql = "EXEC CookbookAppSchema.spIngredients_Get @RecipeId = " + recipeId;

			return await _dapper.LoadDataAsync<IngredientDto>(iSql);
		}
	}
}
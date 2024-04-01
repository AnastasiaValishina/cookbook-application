using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

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

		[HttpGet("Connection")]
		public DateTime Connection()
		{
			return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
		}

		[HttpGet("RecipesAsync")]
		public async Task<IEnumerable<RecipeDto>> GetRecipesAsync()
		{
			string sql = @"SELECT 
					[RecipeId],
                    [UserId],
                    [Title],
                    [Notes],
                    [CategoryId],
                    [RecipeCreated],
                    [RecipeUpdated],
                    [Source], 
                    [CategoryName]
				FROM CookbookAppSchema.Recipes as Recipes
				INNER JOIN CookbookAppSchema.Categories as Categories
				ON Recipes.CategoryId = Categories.Id;";

			var recipes = await _dapper.LoadDataAsync<RecipeDto>(sql);

            foreach (var recipe in recipes)
			{
				var ingredients = await GetIngredients(recipe.RecipeId);
				recipe.Ingredients = ingredients.ToList();
			}

			return recipes;
		}

		private async Task<IEnumerable<IngredientDto>> GetIngredients(int recipeId)
		{
			string iSql = @"SELECT 
						[IngredientId],
						[Name],
						[Qty],
						[Unit] 
					FROM CookbookAppSchema.Ingredients as Ingredients
					WHERE RecipeId = " + recipeId;

			return await _dapper.LoadDataAsync<IngredientDto>(iSql);			
		}

		[HttpGet("RecipesByUserAsync/{userId}")]
		public async Task<IEnumerable<RecipeDto>> GetRecipesByUserAsync(int userId)
		{
			string sql = @"SELECT 
				[RecipeId],
                [UserId],
				[Title],
				[Notes],
				[CategoryId],
				[RecipeCreated],
				[RecipeUpdated],
				[Source],
				[CategoryName] 
			FROM CookbookAppSchema.Recipes as Recipes
			INNER JOIN CookbookAppSchema.Categories as Categories
			ON Recipes.CategoryId = Categories.Id
				WHERE UserId = " + userId.ToString();

			var recipes = await _dapper.LoadDataAsync<RecipeDto>(sql);

			foreach (var recipe in recipes)
			{
				var ingredients = await GetIngredients(recipe.RecipeId);
				recipe.Ingredients = ingredients.ToList();
			}

			return recipes;
		}

/*		[HttpGet("RecipeById/{recipeId}")] 
		public Recipe GetRecipeById(int recipeId)
		{
			string sql = @"SELECT 
				[Title],
				[Notes],
				[CategoryId],
				[RecipeCreated],
				[RecipeUpdated],
				[Source] 
					FROM CookbookAppSchema.Recipes
				WHERE RecipeId = " + recipeId.ToString();

			return _dapper.LoadDataSingle<Recipe>(sql);
		}*/

		[HttpGet("RecipeByIdAsync/{recipeId}")]
		public async Task<RecipeDto> GetRecipeByIdAsync(int recipeId)
		{
			string sql = @"SELECT
				[RecipeId],
                [UserId],
				[Title],
				[Notes],
				[CategoryId],
				[RecipeCreated],
				[RecipeUpdated],
				[Source],
				[CategoryName] 
			FROM CookbookAppSchema.Recipes as Recipes
			INNER JOIN CookbookAppSchema.Categories as Categories
			ON Recipes.CategoryId = Categories.Id
				WHERE RecipeId = " + recipeId.ToString();

			var recipe = await _dapper.LoadDataSingleAsync<RecipeDto>(sql);

			var ingredients = await GetIngredients(recipe.RecipeId);
			recipe.Ingredients = ingredients.ToList();

			return recipe;
		}

		[HttpPost("AddRecipe")]
		public IActionResult AddRecipe(RecipeToAddDto recipe)
		{
			string sql = @"
            INSERT INTO CookbookAppSchema.Recipes (
                [UserId],
                [Title],
                [Notes],
                [CategoryId],
                [RecipeCreated],
                [RecipeUpdated],
                [Source]
			) VALUES (" + 101 // this.User.FindFirst("userId")?.Value
				+ ",'" + recipe.Title
				+ "','" + recipe.Notes
				+ "', " + recipe.CategoryId
				+ ", GETDATE(), GETDATE() "
				+ ", '" + recipe.Source +
				"');" +
				"DECLARE @RecipeId INT; SET @RecipeId = SCOPE_IDENTITY();";

			foreach (var ingredient in recipe.Ingredients)
            {
				string ingridientSql = @"
					INSERT INTO CookbookAppSchema.Ingredients (
						[RecipeId],
						[Name],
						[Qty],
						[Unit]
					) VALUES (@RecipeId" 
						+ ",'" + ingredient.Name
						+ "'," + ingredient.Qty
						+ ", '" + ingredient.Unit
						+ "')";

				sql += ingridientSql;
			}

            //Console.WriteLine(sql);

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add Recipe");
		}

		[HttpDelete("Recipe/{recipeId}")]
		public IActionResult DeleteRecipe(int recipeId)
		{
			string sql = @"DELETE FROM CookbookAppSchema.Recipes 
                WHERE RecipeId = " + recipeId.ToString();
			//+ "AND UserId = " + this.User.FindFirst("userId")?.Value;


			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to delete post!");
		}
	}
}

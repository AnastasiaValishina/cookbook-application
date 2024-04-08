using Cookbook.Api.Data;
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

		[HttpGet("RecipesBySearchParam/{searchParam}")]
		public async Task<IEnumerable<RecipeDto>> RecipesBySearchParam(string searchParam)
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
                WHERE Title LIKE '%" + searchParam + "%'" +
					" OR Notes LIKE '%" + searchParam + "%'";

			var recipes = await _dapper.LoadDataAsync<RecipeDto>(sql);

			string iSql = @"SELECT 
						[IngredientId],
						[RecipeId],
						[Name],
						[Qty],
						[Unit] 
					FROM CookbookAppSchema.Ingredients as Ingredients
						WHERE Name LIKE '%" + searchParam + "%'";

			var searchedByIngredients = await _dapper.LoadDataAsync<Ingredient>(iSql);

			var recipesList = recipes.ToList();

			foreach (Ingredient i in searchedByIngredients)
			{
				var r = await GetRecipeByIdAsync(i.RecipeId);
				recipesList.Add(r);
			}

			foreach (var recipe in recipesList)
			{
				var ingredients = await GetIngredients(recipe.RecipeId);
				recipe.Ingredients = ingredients.ToList();
			}

			return recipesList;
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

		[HttpPost("AddRecipeAsync")]
		public async Task<ActionResult<RecipeDto>> AddRecipeAsync(RecipeToAddDto recipe)
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
			) 
				OUTPUT INSERTED.RecipeId
				VALUES (" + 101 // this.User.FindFirst("userId")?.Value
				+ ",'" + recipe.Title
				+ "','" + recipe.Notes
				+ "', " + recipe.CategoryId
				+ ", GETDATE(), GETDATE() "
				+ ", '" + recipe.Source + "');";

			int recipeId = await _dapper.ExecuteSqlWithIdAsync(sql);

			if (recipeId != 0)
			{
				foreach (var ingredient in recipe.Ingredients)
				{
					string ingredientSql = @"
					INSERT INTO CookbookAppSchema.Ingredients (
						[RecipeId],
						[Name],
						[Qty],
						[Unit]
					) VALUES (" + recipeId
							+ ",'" + ingredient.Name
							+ "'," + ingredient.Qty
							+ ", '" + ingredient.Unit
							+ "')";

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
			string updateSql = @"UPDATE CookbookAppSchema.Recipes
				SET Title = '" + recipeToEdit.Title +
				"', Notes = '" + recipeToEdit.Notes +
				"', CategoryId = " + recipeToEdit.CategoryId.ToString() +
				", Source = '" + recipeToEdit.Source +
				"', RecipeUpdated = GETDATE()" +
				"WHERE RecipeId = " + recipeToEdit.RecipeId.ToString();

			await _dapper.ExecuteSqlAsync(updateSql);

			foreach (var ingredient in recipeToEdit.Ingredients)
			{
				if (ingredient.IngredientId != 0)
				{
					string ingSql = "SELECT * FROM CookbookAppSchema.Ingredients WHERE IngredientId = " + ingredient.IngredientId.ToString();

					var oldIngedient = await _dapper.LoadDataSingleAsync<IngredientDto>(ingSql);

					if (oldIngedient != null)
					{
						string ingredientSql = @"
						UPDATE CookbookAppSchema.Ingredients 
							SET Name = '" + ingredient.Name +
								"', Qty = " + ingredient.Qty +
								", Unit = '" + ingredient.Unit +
								"' WHERE IngredientId = " + ingredient.IngredientId.ToString();

						await _dapper.ExecuteSqlAsync(ingredientSql);
					}
				}
				else if (ingredient.IngredientId == 0)
				{
					string ingredientSql = @"
					INSERT INTO CookbookAppSchema.Ingredients (
						[RecipeId],
						[Name],
						[Qty],
						[Unit]
					) VALUES (" + recipeToEdit.RecipeId
							+ ",'" + ingredient.Name
							+ "'," + ingredient.Qty
							+ ", '" + ingredient.Unit
							+ "')";

					await _dapper.ExecuteSqlAsync(ingredientSql);
				}

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
					string sql = @"DELETE FROM CookbookAppSchema.Recipes 
						WHERE RecipeId = " + recipeId.ToString();
					//+ "AND UserId = " + this.User.FindFirst("userId")?.Value;

					Console.WriteLine(sql);

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

		private async Task<IEnumerable<IngredientDto>> GetIngredients(int recipeId) // перенести в хелпер
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
	}
}

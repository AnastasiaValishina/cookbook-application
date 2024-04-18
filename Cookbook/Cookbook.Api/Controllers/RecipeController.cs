using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cookbook.Api.Controllers
{
	[Authorize]
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

		[HttpGet("MyRecipes/{recipeId}")]
		public async Task<ActionResult<IEnumerable<RecipeDto>>> GetMyRecipesAsync(int recipeId = 0)
		{
			string sql = "EXEC CookbookAppSchema.spRecipes_Get @UserId = @UserIdParameter"; 

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

			if (recipeId != 0)
			{
				sql += ", @RecipeId = @RecipeIdParameter";
				sqlParameters.Add("@RecipeIdParameter", recipeId, DbType.Int32);
			}

			var recipes = await _dapper.LoadDataWithParamsAsync<RecipeDto>(sql, sqlParameters);

			if (recipes != null)
			{
				foreach (var recipe in recipes)
				{
					var ingredients = await GetIngredients(recipe.RecipeId);
					recipe.Ingredients = ingredients.ToList();
				}

				return Ok(recipes);
			}
			return BadRequest("Failed to find recipe");
		}

		[HttpGet("RecipesBySearchParam/{searchParam}")]
		public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesBySearchParam(string searchParam)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipes_GetBySearch 
							@SearchParam = @SearchParameter, 
							@UserId = @UserIdParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@SearchParameter", searchParam, DbType.String);
			sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

			var recipes = await _dapper.LoadDataWithParamsAsync<RecipeDto>(sql, sqlParameters);

			if (recipes != null)
			{
				foreach (var recipe in recipes)
				{
					var ingredients = await GetIngredients(recipe.RecipeId);
					recipe.Ingredients = ingredients.ToList();
				}

				return Ok(recipes);

			}
			return BadRequest("Failed to find recipe");
		}

		[HttpPost("AddRecipeAsync")]
		public async Task<ActionResult<RecipeDto>> AddRecipeAsync(RecipeToAddDto recipe)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipe_Add 
				@UserId = @UserIdParameter, 
				@Title = @TitleParameter, 
				@Notes = @NotesParameter, 
				@CategoryId = @CategoryIdParameter, 
				@Source = @SourceParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
			sqlParameters.Add("@TitleParameter", recipe.Title, DbType.String);
			sqlParameters.Add("@NotesParameter", recipe.Notes, DbType.String);
			sqlParameters.Add("@CategoryIdParameter", recipe.CategoryId, DbType.Int32);
			sqlParameters.Add("@SourceParameter", recipe.Source, DbType.String);

			int recipeId = await _dapper.ExecuteScalarWithParamsAsync(sql, sqlParameters);

			if (recipeId != 0)
			{
				foreach (var ingredient in recipe.Ingredients)
				{
					string ingredientSql = @"EXEC CookbookAppSchema.spIngredient_Upsert 
						@RecipeId = @RecipeIdParameter, 
						@Name = @NameParameter, 
						@Qty = @QtyParameter, 
						@Unit = @UnitParameter";

					DynamicParameters ingredientSqlParams = new DynamicParameters();
					ingredientSqlParams.Add("@RecipeIdParameter", recipeId, DbType.Int32);
					ingredientSqlParams.Add("@NameParameter", ingredient.Name, DbType.String);
					ingredientSqlParams.Add("@QtyParameter", ingredient.Qty, DbType.Single);
					ingredientSqlParams.Add("@UnitParameter", ingredient.Unit, DbType.String);

					await _dapper.ExecuteSqlWithParametersAsync(ingredientSql, ingredientSqlParams);
				}

				RecipeDto createdRecipe = await GetRecipeByIdAsync(recipeId);

				if (createdRecipe != null)
				{
					return Ok(createdRecipe);
				}

				return BadRequest("Failed to retrieve created recipe");
			}
			return BadRequest("Failed to add recipe");
		}

		[HttpPut("UpdateAsync")]
		public async Task<ActionResult> UpdateAsync(RecipeToEditDto recipeToEdit)
		{
			// update recipe
			string updateSql = @"EXEC CookbookAppSchema.spRecipes_Update 
				@UserId = @UserIdParameter, 
				@RecipeId = @RecipeIdParameter, 
				@Title = @TitleParameter, 
				@Notes = @NotesParameter, 
				@CategoryId = @CategoryIdParameter, 
				@Source = @SourceParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
			sqlParameters.Add("@RecipeIdParameter", recipeToEdit.RecipeId, DbType.Int32);
			sqlParameters.Add("@TitleParameter", recipeToEdit.Title, DbType.String);
			sqlParameters.Add("@NotesParameter", recipeToEdit.Notes, DbType.String);
			sqlParameters.Add("@CategoryIdParameter", recipeToEdit.CategoryId, DbType.Int32);
			sqlParameters.Add("@SourceParameter", recipeToEdit.Source, DbType.String);

			if (await _dapper.ExecuteSqlWithParametersAsync(updateSql, sqlParameters))
			{
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
							@IngredientId = @IngredientIdParameter, 
							@RecipeId = @RecipeIdParameter, 
							@Name = @NameParameter, 
							@Qty = @QtyParameter, 
							@Unit = @UnitParameter";

					DynamicParameters ingredientSqlParams = new DynamicParameters();
					ingredientSqlParams.Add("@IngredientIdParameter", ingredient.IngredientId, DbType.Int32);
					ingredientSqlParams.Add("@RecipeIdParameter", recipeToEdit.RecipeId, DbType.Int32);
					ingredientSqlParams.Add("@NameParameter", ingredient.Name, DbType.String);
					ingredientSqlParams.Add("@QtyParameter", ingredient.Qty, DbType.Single);
					ingredientSqlParams.Add("@UnitParameter", ingredient.Unit, DbType.String);

					await _dapper.ExecuteSqlWithParametersAsync(ingSql, ingredientSqlParams);
				}
				return Ok();
			}
			return BadRequest("Failed to update the recipe!");
		}

		[HttpDelete("DeleteRecipeAsync/{recipeId}")]
		public async Task<ActionResult<RecipeDto>> DeleteRecipeAsync(int recipeId)
		{
			var recipeToDelete = await GetRecipeByIdAsync(recipeId);

			if (recipeToDelete != null)
			{
				string sql = @"EXEC CookbookAppSchema.spRecipes_Delete 
					@RecipeId = @RecipeIdParameter, 
					@UserId = @UserIdParameter";

				DynamicParameters sqlParameters = new DynamicParameters();
				sqlParameters.Add("@RecipeIdParameter", recipeId, DbType.Int32);
				sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

				if (await _dapper.ExecuteSqlWithParametersAsync(sql, sqlParameters))
				{
					return Ok(recipeToDelete);
				}
			}
			return BadRequest("Failed to delete the recipe!");
		}

		private async Task<RecipeDto> GetRecipeByIdAsync(int recipeId)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipes_Get 
					@UserId = @UserIdParameter,
					@RecipeId = @RecipeIdParameter"; 

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
			sqlParameters.Add("@RecipeIdParameter", recipeId, DbType.Int32);

			var recipe = await _dapper.LoadDataSingleWithParamsAsync<RecipeDto>(sql, sqlParameters);

			var ingredients = await GetIngredients(recipe.RecipeId);
			recipe.Ingredients = ingredients.ToList();

			return recipe;
		}

		private async Task<IActionResult> DeleteIngredientAsync(int ingredientId) // перенести в хелпер
		{
			string sql = "EXEC CookbookAppSchema.spIngredient_Delete @IngredientId = @IngredientIdParameter";

			DynamicParameters ingredientSqlParams = new DynamicParameters();
			ingredientSqlParams.Add("@IngredientIdParameter", ingredientId, DbType.Int32);

			if (await _dapper.ExecuteSqlWithParametersAsync(sql, ingredientSqlParams))
			{
				return Ok();
			}
			return BadRequest("Failed to delete the indredient!");
		}

		private async Task<IEnumerable<IngredientDto>> GetIngredients(int recipeId) // перенести в хелпер
		{
			string iSql = "EXEC CookbookAppSchema.spIngredients_Get @RecipeId = @RecipeIdParameter";

			DynamicParameters ingredientSqlParams = new DynamicParameters();
			ingredientSqlParams.Add("@RecipeIdParameter", recipeId, DbType.Int32);

			return await _dapper.LoadDataWithParamsAsync<IngredientDto>(iSql, ingredientSqlParams);
		}
	}
}
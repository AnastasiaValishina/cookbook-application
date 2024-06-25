using Cookbook.Api.Data;
using Cookbook.Api.Helpers;
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
		private readonly RecipeHelper _recipeHelper;

		public RecipeController(IConfiguration config)
		{
			_dapper = new DataContextDapper(config);
			_recipeHelper = new RecipeHelper(config);
		}

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
					var ingredients = await _recipeHelper.GetIngredients(recipe.RecipeId);
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
					var ingredients = await _recipeHelper.GetIngredients(recipe.RecipeId);
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

				RecipeDto createdRecipe = await _recipeHelper.GetRecipeByIdAsync(recipeId, this.User.FindFirst("userId")?.Value);

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
				var oldIngredients = await _recipeHelper.GetIngredients(recipeToEdit.RecipeId);

				List<int> newIds = new List<int>();
				foreach (var newIng in recipeToEdit.Ingredients)
				{
					newIds.Add(newIng.IngredientId);
				}
				foreach (var ingredient in oldIngredients)
				{
					if (!newIds.Contains(ingredient.IngredientId))
						await _recipeHelper.DeleteIngredientAsync(ingredient.IngredientId);
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
			var recipeToDelete = await _recipeHelper.GetRecipeByIdAsync(recipeId, this.User.FindFirst("userId")?.Value);

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
	}
}
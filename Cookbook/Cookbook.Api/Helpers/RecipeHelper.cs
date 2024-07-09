using Cookbook.Api.Data;
using Cookbook.Models.Dtos;
using Dapper;
using System.Data;

namespace Cookbook.Api.Helpers
{
	public class RecipeHelper
	{
		private readonly DataContextDapper _dapper;
		public RecipeHelper(IConfiguration config)
        {
			_dapper = new DataContextDapper(config);
		}

		public async Task<RecipeDto> GetRecipeByIdAsync(int recipeId, string? userId)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipes_Get 
					@UserId = @UserIdParameter,
					@RecipeId = @RecipeIdParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
			sqlParameters.Add("@RecipeIdParameter", recipeId, DbType.Int32);

			var recipe = await _dapper.LoadDataSingleWithParamsAsync<RecipeDto>(sql, sqlParameters);

			var ingredients = await GetIngredients(recipe.RecipeId);
			recipe.Ingredients = ingredients.ToList();

			return recipe;
		}

		public async Task DeleteIngredientAsync(int ingredientId) 
		{
			string sql = "EXEC CookbookAppSchema.spIngredient_Delete @IngredientId = @IngredientIdParameter";

			DynamicParameters ingredientSqlParams = new DynamicParameters();
			ingredientSqlParams.Add("@IngredientIdParameter", ingredientId, DbType.Int32);

			await _dapper.ExecuteSqlWithParametersAsync(sql, ingredientSqlParams);
		}

		public async Task<IEnumerable<IngredientDto>> GetIngredients(int recipeId) 
		{
			string iSql = "EXEC CookbookAppSchema.spIngredients_Get @RecipeId = @RecipeIdParameter";

			DynamicParameters ingredientSqlParams = new DynamicParameters();
			ingredientSqlParams.Add("@RecipeIdParameter", recipeId, DbType.Int32);

			return await _dapper.LoadDataWithParamsAsync<IngredientDto>(iSql, ingredientSqlParams);
		}

		public async Task AddRecipeAsync(RecipeToAddDto recipe, int userId)
		{
			string sql = @"EXEC CookbookAppSchema.spRecipe_Add 
				@UserId = @UserIdParameter, 
				@Title = @TitleParameter, 
				@Notes = @NotesParameter, 
				@CategoryId = @CategoryIdParameter, 
				@Source = @SourceParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
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
			}
		}
	}
}

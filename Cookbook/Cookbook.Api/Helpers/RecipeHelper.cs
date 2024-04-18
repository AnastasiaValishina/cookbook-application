using Cookbook.Api.Data;
using Cookbook.Models.Dtos;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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

		public async Task<RecipeDto> GetRecipeByIdAsync(int recipeId, string userId)
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
	}
}

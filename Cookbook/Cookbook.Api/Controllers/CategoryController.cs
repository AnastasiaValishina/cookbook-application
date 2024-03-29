using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook.Api.Controllers
{
	public class CategoryController : ControllerBase
	{
		private readonly DataContextDapper _dapper;

		public CategoryController(IConfiguration config)
		{
			_dapper = new DataContextDapper(config);
		}

		[HttpGet("GetCategories")]
		public async Task<IEnumerable<Category>> GetCategories()
		{
			string sql = @"SELECT 
				[Id],
				[CategoryName] FROM CookbookAppSchema.Categories";

			var categories = await _dapper.LoadDataAsync<Category>(sql);
			return categories;
		}
	}
}

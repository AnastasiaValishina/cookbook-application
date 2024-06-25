using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cookbook.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly DataContextDapper _dapper;

		public UserController(IConfiguration config)
		{
			_dapper = new DataContextDapper(config);
		}

		[HttpPut("UpsertUser")]
		public async Task<IActionResult> UpsertUser(User user)
		{
			string sql = @"EXEC CookbookAppSchema.spUser_Upsert 
					@UserName = @UserNameParam, 
					@Email = @EmailParam, 
					@UserId = @UserIdParam";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserNameParam", user.UserName, DbType.String);
			sqlParameters.Add("@EmailParam", user.Email, DbType.String);
			sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);

			if (await _dapper.ExecuteSqlWithParametersAsync(sql, sqlParameters))
			{
				return Ok();
			}

			throw new Exception("Failed to Update User");
		}

		[HttpDelete("DeleteUser/{userId}")]
		public async Task<IActionResult> DeleteUser(int userId)
		{
			string sql = @"EXEC CookbookAppSchema.spUser_Delete
            @UserId = @UserIdParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

			if (await _dapper.ExecuteSqlWithParametersAsync(sql, sqlParameters))
			{
				return Ok();
			}

			throw new Exception("Failed to Delete User");
		}

		[HttpGet("GetUser/{userId}")]
		public async Task<ActionResult<User>> GetUser(int userId)
		{
			string sql = @"EXEC CookbookAppSchema.spUser_Get @UserId = @UserIdParameter";

			DynamicParameters sqlParameters = new DynamicParameters();
			sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

			var user = await _dapper.LoadDataSingleWithParamsAsync<User>(sql, sqlParameters);

			return user;
		}
	}
}

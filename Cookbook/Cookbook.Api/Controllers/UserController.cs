using Cookbook.Api.Data;
using Cookbook.Api.Models;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult UpsertUser(User user)
		{
			string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @UserName = '" + user.UserName +
				"', @Email = '" + user.Email +
				"', @UserId = " + user.UserId;

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Update User");
		}
	}
}

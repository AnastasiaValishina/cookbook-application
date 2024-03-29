using Cookbook.Api.Data;
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

		[HttpPost("AddUser")]
		public IActionResult AddUser(UserToAddDto user)
		{
			string sql = @"
            INSERT INTO CookbookAppSchema.Users(
                [UserName],
                [Email]
            ) VALUES (" +
					"'" + user.UserName +
					"', '" + user.Email +
				"')";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add User");
		}
	}
}

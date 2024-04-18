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
	public class AuthController : ControllerBase
	{
		private readonly DataContextDapper _dapper;
		private readonly AuthHelper _authHelper;

		public AuthController(IConfiguration config)
		{
			_dapper = new DataContextDapper(config);
			_authHelper = new AuthHelper(config);
		}

		[AllowAnonymous]
		[HttpGet("Connection")]
		public DateTime Connection()
		{
			return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
		}

		[AllowAnonymous]
		[HttpPost("Register")]
		public async Task<IActionResult> Register(UserForRegistrationDto userForRegistration)
		{
			if (userForRegistration.Password == userForRegistration.PasswordConfirm)
			{
				string sqlCheckUserExists = "SELECT Email FROM CookbookAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";

				IEnumerable<string> existingUsers = await _dapper.LoadDataAsync<string>(sqlCheckUserExists);

				if (existingUsers.Count() == 0)
				{
					UserForLoginDto userForSetPassword = new UserForLoginDto()
					{
						Email = userForRegistration.Email,
						Password = userForRegistration.Password
					};

					if (_authHelper.SetPassword(userForSetPassword))
					{
						string sqlAddUser = @"EXEC CookbookAppSchema.spUser_Upsert
							@UserName = '" + userForRegistration.UserName +
							"', @Email = '" + userForRegistration.Email + "'";

/*						string sqlAddUser = @"INSERT INTO CookbookAppSchema.Users(
							[UserName],
							[Email]
								) VALUES (" +
							"'" + userForRegistration.UserName +
							"', '" + userForRegistration.Email +
							"')";*/

						if (_dapper.ExecuteSql(sqlAddUser))
							return Ok();

						throw new Exception("failed to add user!");
					}
					throw new Exception("failed to register user!");
				}
				throw new Exception("User with this email already exists!");
			}
			throw new Exception("Password do not match!");
		}

		[HttpPut("ResetPassword")]
		public IActionResult ResetPassword(UserForLoginDto userForSetPassword) // add new model with password and password confirm
		{
			if (_authHelper.SetPassword(userForSetPassword))
			{
				return Ok();
			}
			throw new Exception("Failed to update password!");
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login(UserForLoginDto userForLogin)
		{
			string sqlForHashAndSalt = @"EXEC CookbookAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

			DynamicParameters sqlParameters = new DynamicParameters();

			sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

/*			SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
			emailParameter.Value = userForLogin.Email;
			sqlParameters.Add(emailParameter);*/

			UserForLoginConfirmationDto userForConfirmation = await _dapper
				.LoadDataSingleWithParamsAsync<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

			byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

			for (int i = 0; i < passwordHash.Length; i++)
			{
				if (passwordHash[i] != userForConfirmation.PasswordHash[i])
					return StatusCode(401, "Incorrect password!");
			}

			string userIdSql = "SELECT UserId FROM CookbookAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";

			int userId = await _dapper.LoadDataSingleAsync<int>(userIdSql);

			return Ok(new Dictionary<string, string> {
				{"token", _authHelper.CreateToken(userId) }
			});
		}

		[HttpGet("RefreshToken")]
		public IActionResult RefreshToken()
		{
			string userId = User.FindFirst("userId")?.Value + "";

			string userIdSql = "SELECT UserId FROM CookbookAppSchema.Users WHERE UserId = " + userId.ToString();

			int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

			return Ok(new Dictionary<string, string> {
				{"token", _authHelper.CreateToken(userIdFromDb) }
			});
		}
	}
}

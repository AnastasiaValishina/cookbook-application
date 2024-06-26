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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
		{
			_dapper = new DataContextDapper(config);
			_authHelper = new AuthHelper(config);
			_logger = logger;
		}

		[AllowAnonymous]
		[HttpPost("Register")]
		public async Task<IActionResult> Register(UserForRegistrationDto userForRegistration)
		{
            _logger.LogInformation("Register called");

            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
			{
				string sqlCheckUserExists = "EXEC CookbookAppSchema.spEmailExists_Get @Email = @EmailParameter";

				DynamicParameters sqlUserExistsParameters = new DynamicParameters();
				sqlUserExistsParameters.Add("@EmailParameter", userForRegistration.Email, DbType.String);

				IEnumerable<string> existingUsers = await _dapper.LoadDataWithParamsAsync<string>(sqlCheckUserExists, sqlUserExistsParameters);

				if (!existingUsers.Any())
				{
					UserForLoginDto userForSetPassword = new UserForLoginDto()
					{
						Email = userForRegistration.Email,
						Password = userForRegistration.Password
					};

					if (await _authHelper.SetPassword(userForSetPassword))
					{
						string sqlAddUser = @"EXEC CookbookAppSchema.spUser_Upsert
							@UserName = @UserNameParameter, 
							@Email = @EmailParameter";

						DynamicParameters sqlAddUserParameters = new DynamicParameters();
						sqlAddUserParameters.Add("@UserNameParameter", userForRegistration.UserName, DbType.String);
						sqlAddUserParameters.Add("@EmailParameter", userForRegistration.Email, DbType.String);

						if (await _dapper.ExecuteSqlWithParametersAsync(sqlAddUser, sqlAddUserParameters))
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
		public async Task<IActionResult> ResetPassword(UserForLoginDto userForSetPassword) // add new model with password and password confirm
		{
			if (await _authHelper.SetPassword(userForSetPassword))
			{
				return Ok();
			}
			throw new Exception("Failed to update password!");
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login(UserForLoginDto userForLogin)
		{
            _logger.LogInformation("Login called");

            string sqlForHashAndSalt = @"EXEC CookbookAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

			DynamicParameters sqlParameters = new DynamicParameters();

			sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

			UserForLoginConfirmationDto userForConfirmation = await _dapper
				.LoadDataSingleWithParamsAsync<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

			byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

			for (int i = 0; i < passwordHash.Length; i++)
			{
				if (passwordHash[i] != userForConfirmation.PasswordHash[i])
					return StatusCode(401, "Incorrect password!");
			}

			string userIdSql = "EXEC CookbookAppSchema.spUserId_Get @Email = @EmailParameter";

			DynamicParameters sqlUserIdParameters = new DynamicParameters();
			sqlUserIdParameters.Add("@EmailParameter", userForLogin.Email, DbType.String);

			int userId = await _dapper.LoadDataSingleWithParamsAsync<int>(userIdSql, sqlUserIdParameters);

            _logger.LogInformation("Login succeeded");

            return Ok(_authHelper.CreateToken(userId));
		}

		[HttpGet("RefreshToken")]
		public async Task<IActionResult> RefreshToken()
		{
			string userIdSql = "EXEC CookbookAppSchema.spUserId_Get @UserId = @UserIdParameter";

			DynamicParameters sqlUserIdParameters = new DynamicParameters();
			sqlUserIdParameters.Add("@UserIdParameter", User.FindFirst("userId")?.Value, DbType.Int32);

			int userIdFromDb = await _dapper.LoadDataSingleWithParamsAsync<int>(userIdSql, sqlUserIdParameters);

			return Ok(_authHelper.CreateToken(userIdFromDb));
		}
	}
}

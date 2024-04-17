using Cookbook.Api.Data;
using Cookbook.Api.Helpers;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;

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

				Console.WriteLine(sqlCheckUserExists);

				IEnumerable<string> existingUsers = await _dapper.LoadDataAsync<string>(sqlCheckUserExists);

				if (existingUsers.Count() == 0)
				{
					byte[] passwordSalt = new byte[128 / 8];
					using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
					{
						rng.GetNonZeroBytes(passwordSalt);
					}
					
					byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

					string sqlAddAuth = @"INSERT INTO CookbookAppSchema.Auth (
						[Email],
						[PasswordHash],
						[PasswordSalt]) VALUES ('" + userForRegistration.Email +
						"', @PasswordHash, @PasswordSalt)";

					List<SqlParameter> sqlParameters = new List<SqlParameter>();

					SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
					passwordSaltParameter.Value = passwordSalt;
					SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
					passwordHashParameter.Value = passwordHash;

					sqlParameters.Add(passwordSaltParameter);
					sqlParameters.Add(passwordHashParameter);

					if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
					{
						string sqlAddUser = @"INSERT INTO CookbookAppSchema.Users(
							[UserName],
							[Email]
								) VALUES (" +
							"'" + userForRegistration.UserName +
							"', '" + userForRegistration.Email +
							"')";

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

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login(UserForLoginDto userForLogin)
		{
			string sqlForHashAndSalt = @"SELECT 
				[PasswordHash],
				[PasswordSalt] 
					FROM CookbookAppSchema.Auth WHERE Email = '" + userForLogin.Email + "'";

			UserForLoginConfirmationDto userForConfirmation = await _dapper.LoadDataSingleAsync<UserForLoginConfirmationDto>(sqlForHashAndSalt);

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

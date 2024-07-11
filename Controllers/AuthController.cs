using Dapper;
using LibrarySystem.DTOs;
using LibrarySystem.Helpers;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LibrarySystem.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly DataContextDapper dapper;
	private readonly AuthHelper authHelper;

	public AuthController(IConfiguration config)
	{
		dapper = new DataContextDapper(config);
		authHelper = new AuthHelper(config);
	}

	[HttpPut("Register")]
	public IActionResult RegisterUser(UserRegistrationDTO user)
	{
		if (user.ConfirmPassword != user.Password)
			return BadRequest(new { message = "Password and confirm password do not match" });

		// Check if the Email already exists
		string emailSql = "SELECT COUNT(Email) FROM Users WHERE Email = @EmailParam";

		DynamicParameters sqlParameters = new DynamicParameters();
		sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);

		if (dapper.LoadSingle<int>(emailSql, sqlParameters) > 0)
			return Conflict(new { message = "Email already exists" });

		// Get password Hash and Salt
		byte[] salt = authHelper.GetRandomSalt();
		byte[] hash = authHelper.GetPasswordHash(user.Password, salt);

		// Insert into UsersAuth table
		string registerSql = "EXEC spUserRegister @FirstName, @LastName, @EmailParam, @PassHash, @PassSalt";
		sqlParameters = new DynamicParameters();
		sqlParameters.Add("@FirstName", user.FirstName, System.Data.DbType.String);
		sqlParameters.Add("@LastName", user.LastName, System.Data.DbType.String);
		sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);
		sqlParameters.Add("@PassHash", hash, System.Data.DbType.Binary);
		sqlParameters.Add("@PassSalt", salt, System.Data.DbType.Binary);

		if (dapper.ExecuteQuery(registerSql, sqlParameters) == 0)
			return StatusCode(500, "Couldn't register user");

		// Get created userId
		string userIdSql = "SELECT UserId FROM Users WHERE Email = @EmailParam";
		sqlParameters = new DynamicParameters();
		sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);
		int userId = dapper.LoadSingle<int>(userIdSql, sqlParameters);

		// Create and return Authorization Token
		string token = authHelper.CreateToken(userId);

		return Ok(new Dictionary<string, string>() { { "token", token } });
	}

	[HttpPut("Login")]
	public IActionResult UserLogin(UserLoginDTO user)
	{
		// Compare password with database
		string sql = "EXEC spAuth_Get @Email";

		DynamicParameters sqlParameters = new DynamicParameters();
		sqlParameters.Add("@Email", user.Email, System.Data.DbType.String);

		UserLoginConfirmationDTO userAuth = dapper.LoadSingle<UserLoginConfirmationDTO>(sql, sqlParameters);

		byte[] hash = authHelper.GetPasswordHash(user.Password, userAuth.PasswordSalt);
		if (!hash.SequenceEqual(userAuth.PasswordHash))
			return BadRequest(new { message = "Incorrect email or password" });

		// Get User ID
		sql = "EXEC spUsers_Get @Email = @EmailParam";
		sqlParameters = new DynamicParameters();
		sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);
		int userId = dapper.LoadSingle<User>(sql, sqlParameters).UserId;

		// Create and return Authorization Token
		string token = authHelper.CreateToken(userId);
		return Ok(new Dictionary<string, string>() { { "token", token } });
	}

	[Authorize]
	[HttpGet("RefreshToken")]
	public IActionResult RefreshToken()
	{
		int userId = Int32.Parse(this.User.FindFirst("userId")?.Value ?? "-1");
		string token = authHelper.CreateToken(userId);
		return Ok(new Dictionary<string, string>() { { "token", token } });
	}

	[HttpPost("GeneratePasswords")]
	public IActionResult GeneratePasswords()
	{
		IEnumerable<User> dbUsers = dapper.LoadData<User>("SELECT * FROM Users");
		if (dbUsers.Count() == 0)
			return NotFound(new { message = "Couldn't find any Users" });

		dapper.ExecuteQuery("TRUNCATE TABLE UsersAuth");

		foreach (User user in dbUsers)
		{
			string password = user.FirstName;
			byte[] salt = authHelper.GetRandomSalt();
			byte[] hash = authHelper.GetPasswordHash(password, salt);

			string insertSql = @"INSERT INTO UsersAuth(UserId, PasswordHash, PasswordSalt)
								VALUES (@UserIdParam, @HashParam, @SaltParam)";
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("@UserIdParam", user.UserId, System.Data.DbType.Int32);
			parameters.Add("@HashParam", hash, System.Data.DbType.Binary);
			parameters.Add("@SaltParam", salt, System.Data.DbType.Binary);

			if (dapper.ExecuteQuery(insertSql, parameters) == 0)
				return StatusCode(500, $"Problem in inserting \nUser ID: {user.UserId} \t Password: {password}");

		}
		return Ok();
	}

	[HttpGet("Test")]
	public string Test()
	{
		return "Your API is working";
	}
}
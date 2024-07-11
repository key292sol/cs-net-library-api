using System.Runtime.CompilerServices;
using Dapper;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private DataContextDapper dapper;
	public UserController(IConfiguration config)
	{
		dapper = new DataContextDapper(config);
	}

	[HttpGet("GetUsers/{userId}/{email}")]
	public IEnumerable<User> GetUsers(int userId = 0, string email = "none")
	{
		string sql = "EXEC spUsers_Get @UserId, @Email";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@UserId", userId != 0 ? userId : null, System.Data.DbType.Int32);
		sqlParameters.Add("@Email", email != "none" ? email : null, System.Data.DbType.String);

		return dapper.LoadData<User>(sql, sqlParameters);
	}

	[HttpPut("UpsertUser")]
	public IActionResult UpsertUser(User user)
	{
		string sql = "EXEC spUsers_Upsert @FirstName, @LastName, @Email, @UserId";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@FirstName", user.FirstName, System.Data.DbType.String);
		sqlParameters.Add("@LastName", user.LastName, System.Data.DbType.String);
		sqlParameters.Add("@Email", user.Email, System.Data.DbType.String);
		sqlParameters.Add("@UserId", user.UserId != 0 ? user.UserId : null, System.Data.DbType.Int32);

		if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
			return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
		return Ok();
	}

	[HttpDelete("DeleteUser/{userId}")]
	public IActionResult DeleteUser(int userId)
	{
		string sql = "EXEC spUsers_Delete @UserId";
		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@UserId", userId, System.Data.DbType.Int32);

		if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
			return BadRequest(new { message = "Failed to delete user" });
		return Ok();
	}
}
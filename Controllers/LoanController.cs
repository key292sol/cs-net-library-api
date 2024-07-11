using Dapper;
using LibrarySystem.Models;
using LibrarySystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystem.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LoanController : ControllerBase
{
	DataContextDapper dapper;
	public LoanController(IConfiguration config)
	{
		dapper = new DataContextDapper(config);
	}

	[HttpGet("GetLoans/{userId}/{bookId}/{loanId}/{hasReturned}")]
	public IEnumerable<UserLoanComplete> GetLoans(int userId = 0, int bookId = 0, int loanId = 0, bool hasReturned = false, DateTime? start = null, DateTime? end = null)
	{
		string sql = @"
			EXEC spUsersLoans_Get
				@UserId = @UserIdParam,
				@BookId = @BookIdParam,
				@LoanId = @LoanIdParam,
				@LoanEndDate = @LoanEndDateParam,
				@LoanStartDate = @LoanStartDateParam,
				@HasReturned = @HasReturnedParam";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@UserIdParam", userId == 0 ? null : userId, System.Data.DbType.Int32);
		sqlParameters.Add("@BookIdParam", bookId == 0 ? null : bookId, System.Data.DbType.Int32);
		sqlParameters.Add("@LoanIdParam", loanId == 0 ? null : loanId, System.Data.DbType.Int32);
		sqlParameters.Add("@LoanEndDateParam", end, System.Data.DbType.DateTime);
		sqlParameters.Add("@LoanStartDateParam", start, System.Data.DbType.DateTime);
		sqlParameters.Add("@HasReturnedParam", hasReturned, System.Data.DbType.Boolean);

		return dapper.LoadData<UserLoanComplete>(sql, sqlParameters);
	}

	// [HttpPut("LendBook/{userId}/{bookId}")]
	// public IActionResult LendBook(int userId, int bookId)
	// {
	// 	string sql = @"
	// 		EXEC spLoan_Upsert
	// 			@UserId = @UserIdParam,
	// 			@BookId = @BookIdParam";

	// 	DynamicParameters sqlParameters = new();
	// 	sqlParameters.Add("@UserIdParam", userId, System.Data.DbType.Int32);
	// 	sqlParameters.Add("@BookIdParam", bookId, System.Data.DbType.Int32);

	// 	if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
	// 		return BadRequest(new { message = "Couldn't Update the data" });
	// 	return Ok();
	// }

	[HttpPut("LoanUpsert/{isReturning}")]
	public IActionResult LoanUpsert(LoanUpsertDTO loanObj, bool isReturning = false)
	{
		string sql = @"
			EXEC spLoan_Upsert
				@UserId = @UserIdParam,
				@BookId = @BookIdParam,
				@LoanId = @LoanIdParam,
				@IsReturning = @IsReturningParam";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@UserIdParam", loanObj.UserId, System.Data.DbType.Int32);
		sqlParameters.Add("@BookIdParam", loanObj.BookId, System.Data.DbType.Int32);
		sqlParameters.Add("@LoanIdParam", loanObj.LoanId == 0 ? null : loanObj.LoanId, System.Data.DbType.Int32);
		sqlParameters.Add("@IsReturningParam", isReturning, System.Data.DbType.Boolean);

		int res = dapper.ExecuteQuery(sql, sqlParameters);
		if (res == 0)
			return BadRequest(new { message = "Couldn't Update the data" });
		return Ok();
	}

	[HttpDelete("DeleteLoan/{loanId}")]
	public IActionResult DeleteLoan(int loanId)
	{
		string sql = "EXEC spLoan_Delete @LoanId";
		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@LoanId", loanId, System.Data.DbType.Int32);

		if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
			return BadRequest(new { message = "Failed to delete loan" });
		return Ok();
	}
}
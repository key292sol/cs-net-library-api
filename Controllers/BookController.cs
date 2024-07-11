namespace LibrarySystem.Controllers;

using Dapper;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
	private readonly DataContextDapper dapper;

	public BookController(IConfiguration config)
	{
		dapper = new DataContextDapper(config);
	}

	[HttpGet("GetBooks/{title}/{author}/{isbn}/{bookId}")]
	public IEnumerable<Book> GetBooks(string title = "~~", string author = "~~", string isbn = "~~", int bookId = 0)
	{
		string sql = @"
			EXEC spBooks_Get 
				@Title = @TitleParam,
				@Author = @AuthorParam,
				@ISBN = @ISBNParam,
				@BookId = @BookIdParam";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@TitleParam", title == "~~" ? null : title, System.Data.DbType.String);
		sqlParameters.Add("@AuthorParam", author == "~~" ? null : author, System.Data.DbType.String);
		sqlParameters.Add("@ISBNParam", isbn == "~~" ? null : isbn, System.Data.DbType.String);
		sqlParameters.Add("@BookIdParam", bookId == 0 ? null : bookId, System.Data.DbType.Int32);

		return dapper.LoadData<Book>(sql, sqlParameters);
	}

	[HttpPut("BookUpsert")]
	public IActionResult BookUpsert(Book book)
	{
		string sql = @"
			EXEC spBooks_Upsert 
				@Title = @TitleParam,
				@Author = @AuthorParam,
				@ISBN = @ISBNParam,
				@Copies = @CopiesParam,
				@BookId = @BookIdParam";

		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@TitleParam", book.Title, System.Data.DbType.String);
		sqlParameters.Add("@AuthorParam", book.Author, System.Data.DbType.String);
		sqlParameters.Add("@ISBNParam", book.ISBN, System.Data.DbType.String);
		sqlParameters.Add("@CopiesParam", book.CopiesAvailable, System.Data.DbType.Int32);
		sqlParameters.Add("@BookIdParam", book.BookId == 0 ? null : book.BookId, System.Data.DbType.Int32);

		if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
			return BadRequest(new { message = "Couldn't Update the book" });
		return Ok();
	}

	[HttpDelete("DeleteBook/{bookId}")]
	public IActionResult DeleteUser(int bookId)
	{
		string sql = "EXEC spBooks_Delete @BookId";
		DynamicParameters sqlParameters = new();
		sqlParameters.Add("@BookId", bookId, System.Data.DbType.Int32);

		if (dapper.ExecuteQuery(sql, sqlParameters) == 0)
			return BadRequest(new { message = "Failed to delete book" });
		return Ok();
	}
}
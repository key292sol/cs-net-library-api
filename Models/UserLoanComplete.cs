namespace LibrarySystem.Models;

public class UserLoanComplete
{
	public int UserId { get; set; }
	public string FirstName { get; set; } = "";
	public string LastName { get; set; } = "";
	public string Email { get; set; } = "";
	public int BookId { get; set; }
	public string Title { get; set; } = "";
	public string Author { get; set; } = "";
	public DateTime LoanDate { get; set; }
	public DateTime ReturnDate { get; set; }
}
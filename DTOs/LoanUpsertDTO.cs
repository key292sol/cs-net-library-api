namespace LibrarySystem.DTOs;

public class LoanUpsertDTO
{
	public int LoanId { get; set; }
	public int UserId { get; set; }
	public int BookId { get; set; }
}
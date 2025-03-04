namespace LibrarySystem.Models;

public class Book
{
	public int BookId { get; set; }
	public string Title { get; set; } = "";
	public string Author { get; set; } = "";
	public string ISBN { get; set; } = "";
	public int CopiesAvailable { get; set; }
}
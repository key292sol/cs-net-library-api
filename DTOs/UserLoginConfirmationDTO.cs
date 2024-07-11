namespace LibrarySystem.DTOs;

public class UserLoginConfirmationDTO
{
	public byte[] PasswordHash { get; set; } = new byte[0];
	public byte[] PasswordSalt { get; set; } = new byte[0];
}
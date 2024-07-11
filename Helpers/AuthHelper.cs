using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace LibrarySystem.Helpers;

public class AuthHelper
{
	private readonly DataContextDapper dapper;
	private readonly IConfiguration config;

	public AuthHelper(IConfiguration _config)
	{
		dapper = new DataContextDapper(_config);
		config = _config;
	}

	public byte[] GetRandomSalt()
	{
		byte[] passSalt = new byte[128 / 8];
		using (RandomNumberGenerator randGen = RandomNumberGenerator.Create())
		{
			randGen.GetNonZeroBytes(passSalt);
		}
		return passSalt;
	}

	public byte[] GetPasswordHash(string password, byte[] salt)
	{
		string saltPlusString = config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(salt);
		byte[] passHash = KeyDerivation.Pbkdf2(
			password: password,
			salt: Encoding.ASCII.GetBytes(saltPlusString),
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 2,
			numBytesRequested: 256 / 8
		);
		return passHash;
	}

	public string CreateToken(int userId)
	{
		Claim[] claims = [
				new Claim("userId", userId.ToString())
			];

		string tokenKeyString = config.GetSection("AppSettings:TokenKey").Value ?? "";
		SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));
		SigningCredentials creds = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature);

		SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
		{
			Subject = new ClaimsIdentity(claims),
			SigningCredentials = creds,
			Expires = DateTime.Now.AddDays(1)
		};

		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
		SecurityToken token = tokenHandler.CreateToken(descriptor);
		return tokenHandler.WriteToken(token);
	}
}
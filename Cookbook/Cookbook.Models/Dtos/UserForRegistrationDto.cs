namespace Cookbook.Models.Dtos
{
	public class UserForRegistrationDto
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PasswordConfirm { get; set; } = string.Empty;
	}
}

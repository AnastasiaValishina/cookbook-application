using System.ComponentModel.DataAnnotations;

namespace Cookbook.Models.Dtos
{
	public class UserForRegistrationDto
	{
		[Required, MaxLength(50, 
			ErrorMessage = "Name cannot be longer than 50 characters.")]
		public string? UserName { get; set; }

		[Required, EmailAddress]
		public string? Email { get; set; }

		[Required]
		[StringLength(24, MinimumLength = 6, 
			ErrorMessage = "The password must be between 6 and 24 characters.")]
		public string? Password { get; set; }

		[Required]
		[Compare(nameof(Password))]
		[StringLength(24, MinimumLength = 6, 
			ErrorMessage = "The password must be between 6 and 24 characters.")]
		public string? PasswordConfirm { get; set; }
	}
}

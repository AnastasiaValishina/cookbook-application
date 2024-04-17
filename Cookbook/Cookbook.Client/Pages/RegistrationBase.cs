using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class RegistrationBase : ComponentBase
	{
		[Inject]
		public IAuthService AuthService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		protected string UserName { get; set; } = string.Empty;
		protected string Email { get; set; } = string.Empty;
		protected string Password { get; set; } = string.Empty;
		protected string PasswordConfirm { get; set; } = string.Empty;
		public string? ErrorMessage { get; set; }

		protected async Task RegisterUser_Click(UserForRegistrationDto userForRegistration)
		{
			try
			{
				await AuthService.RegisterUser(userForRegistration);
				NavigationManager.NavigateTo($"/Login");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}

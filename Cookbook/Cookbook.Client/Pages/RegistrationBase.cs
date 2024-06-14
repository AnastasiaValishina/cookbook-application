using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class RegistrationBase : ComponentBase
	{
		[Inject]
		public required IAuthService AuthService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		protected UserForRegistrationDto UserForRegistration { get; set; } = new UserForRegistrationDto();

		public string? ErrorMessage { get; set; }

		protected async Task RegisterUser_Click()
		{
			try
			{
				await AuthService.RegisterUser(UserForRegistration);
				NavigationManager.NavigateTo($"/Login");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}

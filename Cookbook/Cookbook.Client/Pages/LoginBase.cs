using Cookbook.Client.Services.Contracts;
using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class LoginBase : ComponentBase
	{
		[Inject]
		public IAuthService AuthService { get; set; }

		[Inject]
		public NavigationManager NavigationManager { get; set; }

		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string? ErrorMessage { get; set; }

		protected async Task LoginUser_Click(UserForLoginDto userForLogin)
		{
			try
			{
				await AuthService.LoginAsync(userForLogin);
				NavigationManager.NavigateTo($"/");
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}

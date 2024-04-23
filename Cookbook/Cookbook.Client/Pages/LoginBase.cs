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

		protected UserForLoginDto UserForLogin { get; set; } = new UserForLoginDto();

		public DateTime? Expiration {  get; set; }
		public string? ErrorMessage { get; set; }

		protected async Task LoginUser_Click()
		{
			try
			{
				Expiration = await AuthService.LoginAsync(UserForLogin);

				ErrorMessage = null;
				NavigationManager.NavigateTo($"/");
			}
			catch (Exception ex)
			{
				Expiration = null;
				ErrorMessage = ex.Message;
			}
		}
	}
}

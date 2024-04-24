using Cookbook.Client.Services.Contracts;
using System.Net.Http.Headers;

namespace Cookbook.Client.Handlers
{
	public class AuthHandler : DelegatingHandler
	{
		private readonly IAuthService _authService;
		private readonly IConfiguration _configuration;

		public AuthHandler(IAuthService authService, IConfiguration configuration)
		{
			_authService = authService;
			_configuration = configuration;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var jwt = await _authService.GetJwtAsync();
			var isToServer = request.RequestUri?.AbsoluteUri.StartsWith(_configuration["ServerUrl"] ?? "") ?? false;
			 
			if (isToServer && !string.IsNullOrEmpty(jwt)) 
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}

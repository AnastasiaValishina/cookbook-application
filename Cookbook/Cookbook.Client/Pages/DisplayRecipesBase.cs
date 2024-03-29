using Cookbook.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace Cookbook.Client.Pages
{
	public class DisplayRecipesBase : ComponentBase
	{
		[Parameter]
		public IEnumerable<RecipeDto> Recipes { get; set; }
	}
}

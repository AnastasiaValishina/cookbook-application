using System.ComponentModel.DataAnnotations;

namespace Cookbook.Models.Dtos
{
	public class RecipeToAddDto
	{
		[Required]
		public string? Title { get; set; }
		public string? Notes { get; set; }

		[Required]
		public int CategoryId { get; set; }
		public List<IngredientToAddDto> Ingredients { get; set; } = [];
		public string? Source { get; set; }
	}
}

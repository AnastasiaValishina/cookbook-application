using System.ComponentModel.DataAnnotations;

namespace Cookbook.Models.Dtos
{
	public class RecipeDto
	{
		public int RecipeId { get; set; }
		public int UserId { get; set; }

		[Required]
		public string? Title { get; set; }
		public string? Notes { get; set; }
		public int CategoryId { get; set; }
		public List<IngredientDto> Ingredients { get; set; } = [];
		public DateTime RecipeCreated { get; set; }
		public DateTime RecipeUpdated { get; set; }
		public string? Source { get; set; }
		public string? CategoryName { get; set; }
	}
}

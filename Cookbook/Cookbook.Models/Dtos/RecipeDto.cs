namespace Cookbook.Models.Dtos
{
	public class RecipeDto
	{
		public int RecipeId { get; set; }
		public int UserId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public int CategoryId { get; set; }
		public List<IngredientDto> Ingredients { get; set; } = [];
		public DateTime RecipeCreated { get; set; }
		public DateTime RecipeUpdated { get; set; }
		public string Source { get; set; } = string.Empty;
		public string CategoryName { get; set; } = string.Empty;
	}
}

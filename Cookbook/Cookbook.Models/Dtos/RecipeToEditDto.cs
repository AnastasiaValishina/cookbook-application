namespace Cookbook.Models.Dtos
{
	public class RecipeToEditDto
	{
		public int RecipeId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public int CategoryId { get; set; }
		public List<IngredientDto> Ingredients { get; set; } = [];
		public string Source { get; set; } = string.Empty;
	}
}

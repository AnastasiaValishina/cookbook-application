namespace Cookbook.Models.Dtos
{
	public class RecipeToEditDto
	{
		public int RecipeId { get; set; }
		public string Title { get; set; } = "";
		public string Notes { get; set; } = "";
		public int CategoryId { get; set; }
		public List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
		public string Source { get; set; } = "";
	}
}

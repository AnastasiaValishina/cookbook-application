namespace Cookbook.Models.Dtos
{
	public class RecipeToAddDto
	{
		public string Title { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public int CategoryId { get; set; }
		public List<IngredientToAddDto> Ingredients { get; set; } = [];
		public string Source { get; set; } = string.Empty;
	}
}

namespace Cookbook.Api.Models
{
	public class Recipe
	{
        public int RecipeId { get; set; }
		public int UserId { get; set; }
		public string Title { get; set; } = "";
		public string Notes { get; set; } = "";
		public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
		public int CategoryId { get; set; }
		public DateTime RecipeCreated { get; set; }
		public DateTime RecipeUpdated { get; set; }
		public string Source { get; set; } = "";
	}
}

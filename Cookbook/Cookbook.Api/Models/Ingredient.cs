namespace Cookbook.Api.Models
{
	public class Ingredient
	{
		public int IngredientId { get; set; }
		public string Name { get; set; } = "";
		public float Qty { get; set; }
		public string Unit { get; set; } = "";

		public int RecipeId { get; set; } 
	}
}

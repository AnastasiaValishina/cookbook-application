namespace Cookbook.Models.Dtos
{
	public class IngredientDto
	{
		public int IngredientId { get; set; }
		public string Name { get; set; } = string.Empty;
		public float Qty { get; set; }
		public string Unit { get; set; } = string.Empty;
	}
}

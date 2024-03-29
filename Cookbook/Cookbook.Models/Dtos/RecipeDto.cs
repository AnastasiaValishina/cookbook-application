using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cookbook.Models.Dtos
{
	public class RecipeDto
	{
		public int RecipeId { get; set; }
		public int UserId { get; set; }
		public string Title { get; set; } = "";
		public string Notes { get; set; } = "";
		public int CategoryId { get; set; }
		public List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
		public DateTime RecipeCreated { get; set; }
		public DateTime RecipeUpdated { get; set; }
		public string Source { get; set; } = "";
		public string CategoryName { get; set; } = "";
	}
}

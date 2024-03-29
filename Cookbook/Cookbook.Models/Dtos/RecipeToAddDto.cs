using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cookbook.Models.Dtos
{
	public class RecipeToAddDto
	{
		public string Title { get; set; } = "";
		public string Notes { get; set; } = "";
		public int CategoryId { get; set; }
		public List<IngredientToAddDto> Ingredients { get; set; } = new List<IngredientToAddDto>();
		public string Source { get; set; } = "";
	}
}

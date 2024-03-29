using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cookbook.Models.Dtos
{
	public class IngredientDto
	{
		public int IngredientId { get; set; }
		public string Name { get; set; } = "";
		public float Qty { get; set; }
		public string Unit { get; set; } = "";
	}
}

using Cookbook.Models.Dtos;

namespace Cookbook.Api.Helpers
{
	public class StarterHelper
	{
		private readonly RecipeHelper _recipeHelper;

		public StarterHelper(IConfiguration config)
		{
			_recipeHelper = new RecipeHelper(config);
		}

		public async Task AddStarterRecipes(int userId)
		{
			var recipeToAdd = new RecipeToAddDto()
			{
				Title = "Example: 5 Minute Chocolate Chip Cookies!",
				Notes = "Combine all the ingredients. Roll the dough in cling wrap, shaping it into a log. Pop it in the fridge for 30 minutes to chill. Cut into cookie-width slices. Bake at 160°C for 15 minutes or until they’re a beautiful light brown.",
				CategoryId = 2,
				Ingredients = new List<IngredientToAddDto>
				{
					new() { Name = "plain flour", Qty = 2.5f, Unit = "cups" },
					new() { Name = "brown sugar", Qty = 1.0f, Unit = "cup" },
					new() { Name = "white sugar", Qty = 0.5f, Unit = "cup" },
					new() { Name = "baking powder", Qty = 1.0f, Unit = "tsp" },
					new() { Name = "eggs", Qty = 2.0f, Unit = "pcs" },
					new() { Name = "melted butter", Qty = 200.0f, Unit = "g" },
					new() { Name = "vanilla", Qty = 1.0f, Unit = "splash" },
					new() { Name = "chocolate chips", Qty = 250.0f, Unit = "g" }
				},
				Source = "Cookbook application"
			};
			await _recipeHelper.AddRecipeAsync(recipeToAdd, userId);

			var recipeToAdd2 = new RecipeToAddDto
			{
				Title = "Example: Greek Salad Recipe",
				Notes = "Wash and chop the tomatoes and the cucumber into bite-sized pieces. Thinly slice the red onion. Slice the green and red bell peppers into rings or strips. In a large salad bowl, combine the chopped vegetables. Add the olives. Gently toss the salad ingredients together, then add the feta cheese on top. You can either crumble the feta or cut it into cubes.\n\nMake the Dressing:\nWhisk together the extra virgin olive oil, red wine vinegar, dried oregano, salt, and pepper. Pour the dressing over the salad and gently toss to combine, ensuring all the ingredients are evenly coated.",
				CategoryId = 3, 
				Ingredients = new List<IngredientToAddDto>
			{
				new IngredientToAddDto { Name = "tomatoes", Qty = 4.0f, Unit = "large" },
				new IngredientToAddDto { Name = "cucumber", Qty = 1.0f, Unit = "pcs" },
				new IngredientToAddDto { Name = "red onion", Qty = 1.0f, Unit = "pcs" },
				new IngredientToAddDto { Name = "green bell pepper", Qty = 1.0f, Unit = "pcs" },
				new IngredientToAddDto { Name = "red bell pepper", Qty = 1.0f, Unit = "pcs" },
				new IngredientToAddDto { Name = "olives", Qty = 1.0f, Unit = "cup" },
				new IngredientToAddDto { Name = "feta cheese", Qty = 200.0f, Unit = "g" },
				new IngredientToAddDto { Name = "extra virgin olive oil", Qty = 0.25f, Unit = "cup" },
				new IngredientToAddDto { Name = "red wine vinegar", Qty = 1.0f, Unit = "tablespoon" }, 
                new IngredientToAddDto { Name = "dried oregano", Qty = 1.0f, Unit = "tsp" }, 
                new IngredientToAddDto { Name = "salt", Qty = 1.0f, Unit = "to taste" },
				new IngredientToAddDto { Name = "pepper", Qty = 1.0f, Unit = "to taste" }
			},
				Source = "Cookbook application"
			};
			await _recipeHelper.AddRecipeAsync(recipeToAdd2, userId);
		}
	}
}

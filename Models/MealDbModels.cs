namespace Recipes_app.Models
{
    /// <summary>
    /// Maps TheMealDB API JSON response to C# objects.
    /// </summary>
    public class MealDbResponse
    {
        public List<MealDbMeal>? Meals { get; set; }
    }

    public class MealDbCategoriesResponse
    {
        public List<MealDbCategory>? Categories { get; set; }
    }

    public class MealDbCategory
    {
        public string IdCategory { get; set; } = string.Empty;
        public string StrCategory { get; set; } = string.Empty;
        public string StrCategoryThumb { get; set; } = string.Empty;
        public string StrCategoryDescription { get; set; } = string.Empty;
    }

    public class MealDbFilterResponse
    {
        public List<MealDbFilterItem>? Meals { get; set; }
    }

    public class MealDbFilterItem
    {
        public string IdMeal { get; set; } = string.Empty;
        public string StrMeal { get; set; } = string.Empty;
        public string StrMealThumb { get; set; } = string.Empty;
    }

    public class MealDbMeal
    {
        public string IdMeal { get; set; } = string.Empty;
        public string StrMeal { get; set; } = string.Empty;
        public string StrCategory { get; set; } = string.Empty;
        public string StrArea { get; set; } = string.Empty;
        public string StrInstructions { get; set; } = string.Empty;
        public string StrMealThumb { get; set; } = string.Empty;
        public string StrYoutube { get; set; } = string.Empty;

        // Ingredients (TheMealDB has 20 ingredient slots)
        public string? StrIngredient1 { get; set; }
        public string? StrIngredient2 { get; set; }
        public string? StrIngredient3 { get; set; }
        public string? StrIngredient4 { get; set; }
        public string? StrIngredient5 { get; set; }
        public string? StrIngredient6 { get; set; }
        public string? StrIngredient7 { get; set; }
        public string? StrIngredient8 { get; set; }
        public string? StrIngredient9 { get; set; }
        public string? StrIngredient10 { get; set; }
        public string? StrIngredient11 { get; set; }
        public string? StrIngredient12 { get; set; }
        public string? StrIngredient13 { get; set; }
        public string? StrIngredient14 { get; set; }
        public string? StrIngredient15 { get; set; }
        public string? StrIngredient16 { get; set; }
        public string? StrIngredient17 { get; set; }
        public string? StrIngredient18 { get; set; }
        public string? StrIngredient19 { get; set; }
        public string? StrIngredient20 { get; set; }

        // Measures
        public string? StrMeasure1 { get; set; }
        public string? StrMeasure2 { get; set; }
        public string? StrMeasure3 { get; set; }
        public string? StrMeasure4 { get; set; }
        public string? StrMeasure5 { get; set; }
        public string? StrMeasure6 { get; set; }
        public string? StrMeasure7 { get; set; }
        public string? StrMeasure8 { get; set; }
        public string? StrMeasure9 { get; set; }
        public string? StrMeasure10 { get; set; }
        public string? StrMeasure11 { get; set; }
        public string? StrMeasure12 { get; set; }
        public string? StrMeasure13 { get; set; }
        public string? StrMeasure14 { get; set; }
        public string? StrMeasure15 { get; set; }
        public string? StrMeasure16 { get; set; }
        public string? StrMeasure17 { get; set; }
        public string? StrMeasure18 { get; set; }
        public string? StrMeasure19 { get; set; }
        public string? StrMeasure20 { get; set; }

        /// <summary>
        /// Converts MealDB meal to our Recipe model.
        /// </summary>
        public Recipe ToRecipe()
        {
            var ingredients = new List<Ingredient>();
            var ingredientProperties = GetType().GetProperties()
                .Where(p => p.Name.StartsWith("StrIngredient"))
                .OrderBy(p => p.Name);

            var measureProperties = GetType().GetProperties()
                .Where(p => p.Name.StartsWith("StrMeasure"))
                .OrderBy(p => p.Name)
                .ToList();

            int index = 0;
            foreach (var prop in ingredientProperties)
            {
                var ingredientName = prop.GetValue(this)?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(ingredientName))
                {
                    var measure = index < measureProperties.Count
                        ? measureProperties[index].GetValue(this)?.ToString()?.Trim() ?? ""
                        : "";

                    ingredients.Add(new Ingredient
                    {
                        Name = ingredientName,
                        Quantity = measure,
                        Unit = string.Empty
                    });
                }
                index++;
            }

            // Split instructions into steps
            var instructions = (StrInstructions ?? "")
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return new Recipe
            {
                Id = IdMeal,
                Title = StrMeal,
                Description = $"{StrCategory} recipe from {StrArea}",
                ImageUrl = StrMealThumb,
                Category = StrCategory,
                Ingredients = ingredients,
                Instructions = instructions,
                IngredientsJson = Newtonsoft.Json.JsonConvert.SerializeObject(ingredients),
                InstructionsJson = Newtonsoft.Json.JsonConvert.SerializeObject(instructions),
                CookTimeMinutes = 30,   // Default, MealDB doesn't provide this
                PrepTimeMinutes = 15,
                Servings = 4,
                Rating = 4.0,
                IsFromApi = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

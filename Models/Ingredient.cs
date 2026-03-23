namespace Recipes_app.Models
{
    public class Ingredient
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Quantity) && !string.IsNullOrEmpty(Unit))
                return $"{Quantity} {Unit} {Name}";
            if (!string.IsNullOrEmpty(Quantity))
                return $"{Quantity} {Name}";
            return Name;
        }
    }
}

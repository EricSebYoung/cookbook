using SQLite;

namespace CookBookApp
{
    public class Categories
    {
        [PrimaryKey, AutoIncrement]
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryImage { get; set; }
    }

    public class Recipes
    {
        [PrimaryKey, AutoIncrement]
        public int recipeId { get; set; }
        public string recipeName { get; set; }
        public int categoryId { get; set; }
        public string recipeDirections { get; set; }
        public string recipeImage { get; set; }
    }

    public class Ingredients
    {
        [PrimaryKey, AutoIncrement]
        public int ingredientId { get; set; }
        public string ingredientName { get; set; }
        public int ingredientAmou { get; set; }
        public string ingredientMeas { get; set; }
        public int recipeId { get; set; }
    }
}

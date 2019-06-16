using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CookBookApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddRecipe : ContentPage
	{
        int CategoryID = 0;
        public AddRecipe ()
        {
            InitializeComponent();
        }

		public AddRecipe (int categoryID)
		{
            InitializeComponent();
            CategoryID = categoryID;
            Ingredients.Children.Add(new IngredientTemplate() { BindingContext = new Ingredients() });
        }

        void OnItemAdded(object sender, EventArgs e)
        {
            Ingredients.Children.Add(new IngredientTemplate() { BindingContext = new Ingredients() });
            removeIngredient.IsVisible = true;
        }

        void OnItemRemoved(object sender, EventArgs e)
        {
            Ingredients.Children.RemoveAt(Ingredients.Children.Count-1);
            if (Ingredients.Children.Count == 0)
            {
                removeIngredient.IsVisible = false;
            }
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            var recipe = (Recipes)BindingContext;
            recipe.categoryId = CategoryID;

            var ingredients = (Ingredients)(Ingredients.Children.ElementAt(0) as IngredientTemplate).BindingContext;
            await App.Database.SaveRecipeAsync(recipe);
            int RecipeId = App.Database.GetRecentRecipeID();
            foreach (IngredientTemplate ingredient in Ingredients.Children)
            {
                ingredients = (Ingredients)ingredient.BindingContext;
                ingredients.recipeId = RecipeId;
                await App.Database.SaveIngredientAsync(ingredients);
            }
            await Navigation.PopAsync();
        }
    }
}
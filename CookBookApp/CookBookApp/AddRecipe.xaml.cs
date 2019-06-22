using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var recipe = (Recipes)BindingContext;
            if (recipe.recipeImage != null)
            {
                var image = new Image
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(recipe.recipeImage)),
                };
                ShowImage.Children.Add(image);
            }

            bool firstIng = true;
            foreach (Ingredients ingredient in await App.Database.GetIngredients(recipe.recipeId))
            {
                if (firstIng) {
                    Ingredients.Children.Clear();
                    firstIng = false;
                }

                Ingredients.Children.Add(new IngredientTemplate() { BindingContext = ingredient as Ingredients });
            }
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

        async void OnImageClicked(object sender, EventArgs e)
        {
            var recipe = (Recipes)BindingContext;
            string[] allowedTypes = new string[] { ".jpg", ".png", ".gif", ".jpeg", "UTType.Image", "image/*" };
            var file = await CrossFilePicker.Current.PickFile(allowedTypes);

            if (file != null)
            {
                ImageFile.Text = file.FilePath;
                Stream imageStream = file.GetStream();
                MemoryStream ms = new MemoryStream();
                imageStream.CopyTo(ms);
                imageStream.Seek(0, SeekOrigin.Begin);
                recipe.recipeImage = ms.ToArray();
                var image = new Image { Source = ImageSource.FromStream(() => imageStream) };
                if (ShowImage.Children.Count() != 0)
                {
                    ShowImage.Children.RemoveAt(0);
                }
                ShowImage.Children.Add(image);
            }

            file.Dispose();
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
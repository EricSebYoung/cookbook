using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;

namespace CookBookApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowRecipe : ContentPage
	{
        int RecipeID = 0;

		public ShowRecipe (int RecipeId, string RecipeName)
		{
			InitializeComponent ();
            Title = RecipeName;
            RecipeID = RecipeId;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            int ingredientCount = Ingredients.Children.Count();
            if (ingredientCount == 0)
            {
                foreach (Ingredients ingredient in await App.Database.GetIngredients(RecipeID))
                {
                    StackLayout IngRow = new StackLayout { Spacing = 4, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };
                    Ingredients.Children.Add(IngRow);
                    IngRow.Children.Add(new Label { Text = ingredient.ingredientAmou.ToString(), VerticalTextAlignment = TextAlignment.Center });
                    IngRow.Children.Add(new Label { Text = ingredient.ingredientMeas, FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center });
                    IngRow.Children.Add(new Label { Text = ingredient.ingredientName, TextDecorations = TextDecorations.Underline, VerticalTextAlignment = TextAlignment.Center });
                    Button addToList = new Button
                    {
                        Text = "+",
                        Command = new Command(() =>
                        {
                            // click handler
                            AddToList(ingredient.ingredientName, ingredient.ingredientAmou, ingredient.ingredientMeas);
                        }),
                    };
                    IngRow.Children.Add(addToList);
                }
            }
            var recipe = (Recipes)BindingContext;
            RecipeImage.Source = ImageSource.FromStream(() => new MemoryStream(recipe.recipeImage));
        }

        async void AddToList(string ItemName, int ItemAmou, string ItemMeas)
        {
            ShoppingListItems item = new ShoppingListItems();
            item.itemName = ItemName;
            item.itemAmou = ItemAmou;
            item.itemMeas = ItemMeas;
            item.itemChecked = false;
            await App.Database.SaveShoppingListAsync(item);

            await DisplayAlert("Item Added", "You have added " + ItemName + " to the shopping list.", "OK");
        }

        async void OnShoppingList(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShoppingList()
            {
            });
        }

        async void OnShareClicked(object sender, EventArgs e)
        {
            var recipe = (Recipes)BindingContext;
            string text = "";
            text += "Ingredients: " + "\n";
            foreach (Ingredients ingredient in await App.Database.GetIngredients(RecipeID))
            {
                text += ingredient.ingredientAmou + " " + ingredient.ingredientMeas + " " + ingredient.ingredientName + "\n";
            }

            text += "Directions: " + "\n" + recipe.recipeDirections;

            await Share.RequestAsync(new ShareTextRequest
            {
                Title = recipe.recipeName,
                Text = text
            });
        }
    }
}
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
            foreach(Ingredients ingredient in await App.Database.GetIngredients(RecipeID))
            {
                StackLayout IngRow = new StackLayout { Spacing = 4, Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };
                Ingredients.Children.Add(IngRow);
                IngRow.Children.Add(new Label { Text = ingredient.ingredientAmou.ToString(), VerticalTextAlignment = TextAlignment.Center});
                IngRow.Children.Add(new Label { Text = ingredient.ingredientMeas, FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center });
                IngRow.Children.Add(new Label { Text = ingredient.ingredientName, TextDecorations = TextDecorations.Underline, VerticalTextAlignment = TextAlignment.Center });
                Button addToList = new Button { Text = "+",
                    Command = new Command(() => {
                        // click handler
                    })
                };
                IngRow.Children.Add(addToList);
            }
        }

        async void OnShareClicked(object sender, SelectedItemChangedEventArgs e)
        {
         
        }
    }
}
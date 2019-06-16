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
	public partial class ViewRecipes : ContentPage
	{
        int CategoryID = 0;

		public ViewRecipes (int categoryID, string categoryNAME)
		{
			InitializeComponent();
            CategoryID = categoryID;
            Title = categoryNAME;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            listView.ItemsSource = await App.Database.GetRecipes(CategoryID);
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRecipe(CategoryID)
            {
                BindingContext = new Recipes()
            });
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((App)App.Current).ResumeAtId = (e.SelectedItem as Recipes).recipeId;
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new ShowRecipe((e.SelectedItem as Recipes).recipeId, (e.SelectedItem as Recipes).recipeName)
                {
                    BindingContext = e.SelectedItem as Recipes
                });
            }
        }

        async void OnShoppingList(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShoppingList()
            {
            });
        }
    }
}
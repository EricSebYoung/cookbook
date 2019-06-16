using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CookBookApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            listView.ItemsSource = await App.Database.GetCategories();
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCategory()
            {
                BindingContext = new Categories()
            });
        }

        async void OnShoppingList(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShoppingList()
            {
            });
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((App)App.Current).ResumeAtId = (e.SelectedItem as Categories).categoryId;
            //Debug.WriteLine("setting ResumeAtTodoId = " + (e.SelectedItem as TodoItem).ID);
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new ViewRecipes((e.SelectedItem as Categories).categoryId, (e.SelectedItem as Categories).categoryName)
                {
                });
            }
        }
    }
}

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
	public partial class ShoppingList : ContentPage
	{
		public ShoppingList ()
		{
			InitializeComponent ();
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            listView.ItemsSource = await App.Database.GetList();
        }

        async void OnClear(object sender, EventArgs e)
        {
            await App.Database.DeleteShoppingListAsync();
            listView.ItemsSource = null;
            listView.ItemsSource = await App.Database.GetList();
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((App)App.Current).ResumeAtId = (e.SelectedItem as ShoppingListItems).itemId;
            //Debug.WriteLine("setting ResumeAtTodoId = " + (e.SelectedItem as TodoItem).ID);
            if (e.SelectedItem != null)
            {
                ShoppingListItems item = e.SelectedItem as ShoppingListItems;
                if (item.itemChecked == true)
                {
                    item.itemChecked = false;
                } else
                {
                    item.itemChecked = true;
                }
                await App.Database.SaveShoppingListAsync(item);
                listView.ItemsSource = null;
                listView.ItemsSource = await App.Database.GetList();
            }
        }
    }
}
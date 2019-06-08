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
	public partial class AddCategory : ContentPage
	{
		public AddCategory ()
		{
            InitializeComponent();
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            var category = (Categories)BindingContext;
            await App.Database.SaveCategoryAsync(category);
            await Navigation.PopAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FilePicker;
using System.IO;

namespace CookBookApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCategory : ContentPage
	{

		public AddCategory ()
		{
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var category = (Categories)BindingContext;
            if (category.categoryImage != null)
            {
                var image = new Image
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(category.categoryImage)),
                };
                ShowImage.Children.Add(image);
            }
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            var category = (Categories)BindingContext;

            await App.Database.SaveCategoryAsync(category);
            await Navigation.PopAsync();
        }

        async void OnImageClicked(object sender, EventArgs e)
        {
            var category = (Categories)BindingContext;
            string[] allowedTypes = new string[] { ".jpg", ".png", ".gif", ".jpeg", "UTType.Image", "image/*" };
            var file = await CrossFilePicker.Current.PickFile(allowedTypes);

            if (file != null)
            {
                ImageFile.Text = file.FilePath;
                Stream imageStream = file.GetStream();
                MemoryStream ms = new MemoryStream();
                imageStream.CopyTo(ms);
                imageStream.Seek(0, SeekOrigin.Begin);
                category.categoryImage = ms.ToArray();
                var image = new Image { Source = ImageSource.FromStream(() => imageStream) };
                if (ShowImage.Children.Count() != 0)
                {
                    ShowImage.Children.RemoveAt(0);
                }
                ShowImage.Children.Add(image);
            }

            file.Dispose();
        }
    }
}
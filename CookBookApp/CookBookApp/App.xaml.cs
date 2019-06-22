using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CookBookApp
{
    public partial class App : Application
    {
        static CookbookDatabase database;

        public App()
        {
            Resources = new ResourceDictionary();
            Resources.Add("primaryGreen", Color.FromHex("91CA47"));
            Resources.Add("primaryDarkGreen", Color.FromHex("6FA22E"));

            var nav = new NavigationPage(new MainPage());
            nav.BarBackgroundColor = (Color)App.Current.Resources["primaryGreen"];
            nav.BarTextColor = Color.White;

            MainPage = nav;

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static CookbookDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CookbookDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CookbookSQLite.db3"));
                }
                if (!database.CategoryExists())
                {
                    string unsortedName = "Unsorted";
                    var unsortedCat = new Categories();
                    unsortedCat.categoryName = unsortedName;
                    unsortedCat.categoryImage = null;

                    database.SaveCategoryAsync(unsortedCat);
                }
                return database;
            }
        }

        public int ResumeAtId { get; set; }
    }
}


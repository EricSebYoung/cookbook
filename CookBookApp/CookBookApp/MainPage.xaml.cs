using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CookBookApp
{
    public partial class MainPage : ContentPage
    {
        bool DeleteMode = false;
        bool EditMode = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            if (EditMode)
            {
                ToggleEditMode();
            }
            if (DeleteMode)
            {
                ToggleEditMode();
            }
            PopulateMenu();
        }

        async void PopulateMenu()
        {
            if (CategoriesView.Children.Count() > 0)
            {
                CategoriesView.Children.Clear();
            }

            foreach (Categories category in await App.Database.GetCategories())
            {

                Grid grid = new Grid
                {
                    VerticalOptions = LayoutOptions.Start,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = 20 },
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                    }
                };

                CategoriesView.Children.Add(grid);

                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.Command = new Command(() =>
                {
                    // click handler
                    OnItemSelected(category);
                });

                var nameLabel = new Label { Text = category.categoryName.ToString(), VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                nameLabel.GestureRecognizers.Add(tapRecognizer);

                if (category.categoryImage != null)
                {
                    var imageButton = new Image
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(category.categoryImage)), HeightRequest = 160, WidthRequest = 160
                    };
                    imageButton.Aspect = Aspect.AspectFill;
                    imageButton.GestureRecognizers.Add(tapRecognizer);
                    grid.Children.Add(imageButton, 0, 0);
                }
                else
                {
                    var boxView = new BoxView
                    {
                        Color = Color.CornflowerBlue,
                        CornerRadius = 10,
                        WidthRequest = 160,
                        HeightRequest = 160,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    boxView.GestureRecognizers.Add(tapRecognizer);
                    grid.Children.Add(boxView, 0, 0);
                }
                grid.Children.Add(nameLabel, 0, 1);
            }

            Grid endGrid = new Grid
            {
                VerticalOptions = LayoutOptions.Start,
                RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = 20 },
                    },
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                    }
            };
            CategoriesView.Children.Add(endGrid);

            Button addItemButton = new Button
            {
                Text = "Add Category",
                HeightRequest = 160,
                WidthRequest = 160
            };

            addItemButton.Clicked += OnItemAdded;
            endGrid.Children.Add(addItemButton,0,0);
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

        void OnItemEdit(object sender, EventArgs e)
        {
            ToggleEditMode();
        }

        void ToggleEditMode()
        {
            if (!EditMode)
            {
                if (DeleteMode)
                {
                    ToggleDeleteMode();
                }
                EditMode = true;
                EditLabel.IsVisible = true;
                Title = "Select Category to Edit";
                EditTool.Text = "Stop Editing..";
            }
            else
            {
                EditMode = false;
                EditLabel.IsVisible = false;
                Title = "Your Cookbook";
                EditTool.Text = "Edit Category";
            }
        }

        void OnItemDelete(object sender, EventArgs e)
        {
            ToggleDeleteMode();
        }
        
        void ToggleDeleteMode() {
            if (!DeleteMode)
            {
                if (EditMode)
                {
                    ToggleDeleteMode();
                }
                DeleteMode = true;
                DeleteLabel.IsVisible = true;
                Title = "Deleting Categories";
                DeleteTool.Text = "Stop Deleting..";
            }
            else
            {
                DeleteMode = false;
                DeleteLabel.IsVisible = false;
                Title = "Your Cookbook";
                DeleteTool.Text = "Delete Category";
            }
        }

        async void OnItemSelected(Categories selectedCategory)
        {
            ((App)App.Current).ResumeAtId = selectedCategory.categoryId;

            if (DeleteMode)
            {
                if (selectedCategory != null && selectedCategory.categoryId != 1)
                {
                    var result = await DisplayAlert("Deleting " + selectedCategory.categoryName,
                                        "Are you sure you wish to delete " + selectedCategory.categoryName + "?",
                                        "Confirm", "Cancel");
                    if (result) {
                        await App.Database.DeleteCategoryAsync(selectedCategory);
                        PopulateMenu();
                    }
                }
                if (selectedCategory.categoryId == 1)
                {
                    await DisplayAlert("Cannot Delete", "You cannot delete the Unsorted category", "OK");
                }

            } else if (EditMode)
            {
                if (selectedCategory != null)
                {
                    await Navigation.PushAsync(new AddCategory()
                    {
                        BindingContext = selectedCategory as Categories
                    });
                }
            } else {
                if (selectedCategory != null)
                {
                    await Navigation.PushAsync(new ViewRecipes((selectedCategory as Categories).categoryId, selectedCategory.categoryName)
                    {
                    });
                }
            }
        }
    }
}

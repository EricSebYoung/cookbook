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
	public partial class ViewRecipes : ContentPage
	{
        int CategoryID = 0;
        bool DeleteMode = false;
        bool EditMode = false;
        string catName = "";

		public ViewRecipes (int categoryID, string categoryNAME)
		{
			InitializeComponent();
            CategoryID = categoryID;
            catName = categoryNAME;
            Title = categoryNAME;
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtId = -1;
            if (EditMode) { ToggleEditMode(); }
            if (DeleteMode) { ToggleDeleteMode(); }
            PopulateMenu();
        }

        async void PopulateMenu()
        {
            if (RecipesView.Children.Count() > 0)
            {
                RecipesView.Children.Clear();
            }

            foreach (Recipes recipe in await App.Database.GetRecipes(CategoryID))
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

                RecipesView.Children.Add(grid);

                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.Command = new Command(() =>
                {
                    // click handler
                    OnItemSelected(recipe);
                });

                var nameLabel = new Label { Text = recipe.recipeName.ToString(), VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                nameLabel.GestureRecognizers.Add(tapRecognizer);

                if (recipe.recipeImage != null)
                {
                    var imageButton = new Image
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(recipe.recipeImage)),
                        HeightRequest = 160,
                        WidthRequest = 160
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
            RecipesView.Children.Add(endGrid);

            Button addItemButton = new Button
            {
                Text = "Add Recipe",
                HeightRequest = 160,
                WidthRequest = 160
            };
            addItemButton.Clicked += OnItemAdded;
            endGrid.Children.Add(addItemButton);
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
                Title = "Select Recipe to Edit";
                EditTool.Text = "Stop Editing..";
            }
            else
            {
                EditMode = false;
                EditLabel.IsVisible = false;
                Title = "Your Cookbook";
                EditTool.Text = "Edit Recipe";
            }
        }

        void OnItemDelete(object sender, EventArgs e)
        {
            ToggleDeleteMode();
        }

        void ToggleDeleteMode()
        {
            if (!DeleteMode)
            {
                if (EditMode)
                {
                    ToggleDeleteMode();
                }
                DeleteMode = true;
                DeleteLabel.IsVisible = true;
                Title = "Deleting Recipes";
                DeleteTool.Text = "Stop Deleting..";
            }
            else
            {
                DeleteMode = false;
                DeleteLabel.IsVisible = false;
                Title = "Your Cookbook";
                DeleteTool.Text = "Delete Recipe";
            }
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRecipe(CategoryID)
            {
                BindingContext = new Recipes()
            });
        }

        async void OnItemSelected(Recipes recipe)
        {
            ((App)App.Current).ResumeAtId = recipe.recipeId;

            if (DeleteMode)
            {
                if (recipe != null)
                {
                    var result = await DisplayAlert("Deleting " + recipe.recipeName,
                                        "Are you sure you wish to delete " + recipe.recipeName + "?",
                                        "Confirm", "Cancel");
                    if (result)
                    {
                        await App.Database.DeleteRecipeAsync(recipe);
                        PopulateMenu();
                    }
                }

            } else if (EditMode) {
                if (recipe != null)
                {
                    await Navigation.PushAsync(new AddRecipe(CategoryID)
                    {
                        BindingContext = recipe as Recipes
                    });
                }
            } else {
                if (recipe != null)
                {
                    await Navigation.PushAsync(new ShowRecipe((recipe as Recipes).recipeId, recipe.recipeName)
                    {
                        BindingContext = recipe as Recipes
                    });
                }
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
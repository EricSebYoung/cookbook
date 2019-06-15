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
	public partial class IngredientTemplate : ContentView
	{
		public IngredientTemplate ()
		{
			InitializeComponent ();
		}

    }
}
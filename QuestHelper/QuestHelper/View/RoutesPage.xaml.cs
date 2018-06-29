using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutesPage : ContentPage
	{
		public RoutesPage ()
		{
			InitializeComponent ();
            BindingContext = new RoutesViewModel() { Navigation = this.Navigation };
        }
    }
}
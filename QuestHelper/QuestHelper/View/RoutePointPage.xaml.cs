using QuestHelper.Model.DB;
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
	public partial class RoutePointPage : ContentPage
	{
		public RoutePointPage (Route route, RoutePoint routePoint)
		{
			InitializeComponent ();
            BindingContext = new RoutePointViewModel(route, routePoint) { Navigation = this.Navigation };
        }
    }
}
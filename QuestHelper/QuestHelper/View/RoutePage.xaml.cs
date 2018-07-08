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
	public partial class RoutePage : ContentPage
	{
		public RoutePage()
		{

            InitializeComponent ();
            BindingContext = new RouteViewModel(new Route()) { Navigation = this.Navigation };
        }
        public RoutePage(Route routeItem)
        {

            InitializeComponent();
            BindingContext = new RouteViewModel(routeItem) { Navigation = this.Navigation };
        }
    }
}
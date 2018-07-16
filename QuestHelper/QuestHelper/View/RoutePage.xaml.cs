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
        RouteViewModel vm;
        public RoutePage()
		{
            InitializeComponent ();
            vm = new RouteViewModel(new Route()) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public RoutePage(Route routeItem)
        {

            InitializeComponent();
            vm = new RouteViewModel(routeItem) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
        }
    }
}
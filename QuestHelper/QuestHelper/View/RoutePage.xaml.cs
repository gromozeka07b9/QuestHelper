using QuestHelper.LocalDB.Model;
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
            vm = new RouteViewModel(new Route(), false) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public RoutePage(Route routeItem, bool isFirstRoute)
        {

            InitializeComponent();
            Title = routeItem.Name;
            vm = new RouteViewModel(routeItem, isFirstRoute) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}
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
    public partial class NewRoutePage : ContentPage
	{
        NewRouteViewModel vm;
        public NewRoutePage()
		{
            InitializeComponent ();
            vm = new NewRouteViewModel(new Route(), false) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public NewRoutePage(Route routeItem, bool isFirstRoute)
        {

            InitializeComponent();
            vm = new NewRouteViewModel(routeItem, isFirstRoute) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}
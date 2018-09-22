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
    public partial class RouteCreatedPage : ContentPage
	{
        RouteCreatedViewModel vm;
        public RouteCreatedPage()
		{
            InitializeComponent ();
        }
        public RouteCreatedPage(Route routeItem)
        {

            InitializeComponent();
            vm = new RouteCreatedViewModel(routeItem) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}
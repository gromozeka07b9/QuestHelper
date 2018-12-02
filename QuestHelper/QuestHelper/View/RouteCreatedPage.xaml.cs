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
    public partial class RouteCreatedPage : ContentPage
	{
        RouteCreatedViewModel vm;
        public RouteCreatedPage()
		{
            InitializeComponent ();
        }
        public RouteCreatedPage(string routeId)
        {

            InitializeComponent();
            vm = new RouteCreatedViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}
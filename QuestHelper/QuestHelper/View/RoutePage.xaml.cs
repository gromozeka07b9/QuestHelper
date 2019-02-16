using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using QuestHelper.Model.Messages;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePage : ContentPage
	{
        private RouteViewModel _vm;
	    private Route _route;
        public RoutePage()
		{
            InitializeComponent ();
		    _vm = new RouteViewModel(string.Empty, false) { Navigation = this.Navigation };
            BindingContext = _vm;
        }
        public RoutePage(string routeId, bool isFirstRoute)
        {

            InitializeComponent();
            RouteManager manager = new RouteManager();
            if (!string.IsNullOrEmpty(routeId))
            {
                _route = manager.GetRouteById(routeId);
            }
            Title = _route.Name;
            _vm = new RouteViewModel(_route.RouteId, isFirstRoute) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.startDialog();
        }

        /*internal void AddSharePointMessage(ShareFromGoogleMapsMessage sharePointMessage)
        {
            _vm.AddNewPointFromShare(sharePointMessage);
        }*/

	    private void RoutePage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.closeDialog();
	    }
	}
}
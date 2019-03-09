using Microsoft.AppCenter.Crashes;
using QuestHelper.Managers;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutePointCarouselPage : CarouselPage
	{
	    private string _routePointId;
	    private RoutePointCarouselViewModel _vm;

	    public RoutePointCarouselPage()
	    {
	        InitializeComponent();
	    }

        public RoutePointCarouselPage (string routeId, string routePointId)
		{
			InitializeComponent ();
		    _routePointId = routePointId;
		    _vm = new RoutePointCarouselViewModel(routeId) { Navigation = this.Navigation };
		    BindingContext = _vm;
		}

	    private void RoutePointCarouselPage_OnAppearing(object sender, EventArgs e)
	    {
	        _vm.StartDialog();
	        try
	        {
	            foreach (var page in _vm.CarouselPages)
	            {
	                Children.Add(page);
	            }
	        }
            catch (Exception exception)
	        {
	            Crashes.TrackError(exception);
	        }
        }

        private void RoutePointCarouselPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.CloseDialog();
	    }

	    private void RoutePointCarouselPage_OnCurrentPageChanged(object sender, EventArgs e)
	    {
	        var currentPage = CurrentPage;
            currentPage?.SendAppearing();
	    }
	}
}
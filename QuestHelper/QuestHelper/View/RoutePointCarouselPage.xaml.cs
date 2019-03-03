using QuestHelper.Managers;
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
	public partial class RoutePointCarouselPage : CarouselPage
	{
	    //private RoutePoint _routePoint;
	    private string _routePointId;
	    private RoutePointViewModel _vm;

	    public RoutePointCarouselPage()
	    {
	        InitializeComponent();
	    }

        public RoutePointCarouselPage (string routeId, string routePointId)
		{
			InitializeComponent ();
		    _routePointId = routePointId;
            /*RoutePointManager manager = new RoutePointManager();
		    if (!string.IsNullOrEmpty(routePointId))
		        _routePoint = manager.GetPointById(routePointId);*/
		    _vm = new RoutePointViewModel(routeId, routePointId) { Navigation = this.Navigation };
		    BindingContext = _vm;
		}

	    private void RoutePointCarouselPage_OnAppearing(object sender, EventArgs e)
	    {
	        _vm.StartDialog();
	        foreach (var media in _vm.Images)
	        {
	            Children.Add(new PointCarouselItemPage(_routePointId, media.MediaId));
	        }
        }

        private void RoutePointCarouselPage_OnDisappearing(object sender, EventArgs e)
	    {
	        _vm.CloseDialog();
	    }
    }
}
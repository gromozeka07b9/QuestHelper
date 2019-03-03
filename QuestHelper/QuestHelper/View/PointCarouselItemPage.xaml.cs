using QuestHelper.Model;
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
	public partial class PointCarouselItemPage : ContentPage
	{
        private ViewRoutePoint _routePoint;
        private PointCarouselItemViewModel _vm;
        public PointCarouselItemPage()
		{
			InitializeComponent ();
		}
	    public PointCarouselItemPage(string routePointId, string routePointMediaId)
	    {
	        InitializeComponent();
	        _vm = new PointCarouselItemViewModel(routePointId, routePointMediaId) { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }
    }
}
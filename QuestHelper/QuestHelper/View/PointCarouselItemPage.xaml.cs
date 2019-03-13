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
        public ViewRoutePoint _routePoint;
        private PointCarouselItemViewModel _vm;
	    private bool isShowPreview = true;

        public PointCarouselItemPage()
		{
			InitializeComponent ();
		}
	    public PointCarouselItemPage(string routeId, string routePointId, string routePointMediaId)
	    {
	        InitializeComponent();
	        _vm = new PointCarouselItemViewModel(routeId, routePointId, routePointMediaId) { Navigation = this.Navigation };
	        BindingContext = _vm;
	    }


        private void PointCarouselItemPage_OnAppearing(object sender, EventArgs e)
	    {
	        if (isShowPreview)
	        {
	            ImageItem.Source = _vm.OneImagePreview;
	            Device.StartTimer(TimeSpan.FromMilliseconds(500), OnTimerForUpdate);
	        }
        }

        private bool OnTimerForUpdate()
        {
            ImageItem.Source = _vm.OneImage;
            isShowPreview = false;
            return false;
        }

        private void PointCarouselItemPage_OnDisappearing(object sender, EventArgs e)
	    {
	    }
	}
}
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Managers;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

        private async void RoutePointCarouselPage_OnAppearing(object sender, EventArgs e)
	    {
	        _vm.StartDialog();
	        await FillPagesCollectionAsync();
	        /*try
            {
                //FillPagesCollectionAsync();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }*/
        }

        private async Task FillPagesCollectionAsync()
	    {
	        UserDialogs.Instance.ShowLoading("Подготовка альбома...");
	        //await Thread.Sleep(2000);
	        DateTime startTime = DateTime.Now;
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

	        string username = await DependencyService.Get<IUsernameService>().GetUsername();
            LoggerLevels levels = new LoggerLevels();
	        var eventStructure = new Dictionary<string, string>
	            {{"DelayLevel", levels.GetTimeLevels(startTime, DateTime.Now, 2, 5, 8).ToString()}};
	        eventStructure.Add("Username", username);
            Analytics.TrackEvent("Carousel: create time", eventStructure);
	        UserDialogs.Instance.HideLoading();
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
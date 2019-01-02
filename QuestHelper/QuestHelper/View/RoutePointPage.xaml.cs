using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutePointPage : ContentPage
	{
        private RoutePoint _routePoint;
        private RoutePointViewModel _vm;

        public ICommand Test { get; }

        public RoutePointPage()
        {
            InitializeComponent();
        }
        public RoutePointPage (string routeId, string routePointId)
		{
			InitializeComponent ();
            RoutePointManager manager = new RoutePointManager();
            if(!string.IsNullOrEmpty(routePointId))
                _routePoint = manager.GetPointById(routePointId);
            _vm = new RoutePointViewModel(routeId, routePointId) { Navigation = this.Navigation };
            _vm.PropertyChanged += Vm_PropertyChanged;
            _vm.DeleteCommand = new Command(deleteRoutePoint);
            BindingContext = _vm;

        }

        private async void deleteRoutePoint(object obj)
        {
            var result = await DisplayAlert("Внимание!", "Удалить точку?", "Да", "Нет");
            if(result)
            {
                _vm.DeletePoint();
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Latitude":
                case "Longitude":
                {
                    if ((_vm.Latitude > 0) && (_vm.Longitude > 0))
                    {
                        CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
                        _vm.ApplyChanges();
                    }
                };break;
                case "Images":
                {
                    UpdateImages();
                };break;
            }
        }

	    private void UpdateImages()
	    {
	        var control = this.FindByName<StackLayout>("ImagesStackPanel");
	        control.Children.Clear();
	        if (_vm.Images.Count == 0)
	        {
	            Image img = new Image()
	            {
	                Source = "camera1.png",
	                Aspect = Aspect.AspectFill,
	                HorizontalOptions = LayoutOptions.CenterAndExpand,
	                VerticalOptions = LayoutOptions.CenterAndExpand
	            };
	            img.GestureRecognizers.Add(new TapGestureRecognizer() { Command = _vm.ViewPhotoCommand, CommandParameter = new FileImageSource() });
                control.Children.Add(img);
	        }
            foreach (var image in _vm.Images)
	        {
	            Image img = new Image()
	            {
	                Source = image.Source,
	                Aspect = Aspect.AspectFill,
                    WidthRequest = _vm.Images.Count == 1?0:200,
	                HorizontalOptions = LayoutOptions.FillAndExpand,
	                VerticalOptions = LayoutOptions.FillAndExpand
                };
                img.GestureRecognizers.Add(new TapGestureRecognizer() { Command = _vm.ViewPhotoCommand, CommandParameter = img.Source});
                control.Children.Add(img);
	        }
	    }

        private void PointPin_Clicked(object sender, EventArgs e)
        {
        }

        private async void ContentPage_AppearingAsync(object sender, EventArgs e)
        {
            if((_routePoint!=null)&&(_routePoint.Latitude > 0) && (_routePoint.Longitude > 0) && (!string.IsNullOrEmpty(_routePoint.Name)))
            {
                await CenterMap(_routePoint.Latitude, _routePoint.Longitude, _routePoint.Name, _routePoint.Address);
            }
            _vm.StartDialog();
            UpdateImages();
        }

        private async Task CenterMap(double latitude, double longitude, string name, string address)
        {
            CustomMapView customMap = new CustomMapView((CustomMap)this.PointMapOverview, 15);
            if (customMap.CenterMapToPosition(latitude, longitude))
            {
                customMap.AddPin(latitude, longitude, name, address, PointPin_Clicked);
            }
            else
            {
                await DisplayAlert("Ошибка", customMap.LastError, "Ок");
            }
        }

	    private void Editor_OnCompleted(object sender, EventArgs e)
	    {
	        _vm.ApplyChanges();
	    }
    }
}
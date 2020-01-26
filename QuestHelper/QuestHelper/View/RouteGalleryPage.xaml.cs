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
    public partial class RouteGalleryPage : ContentPage
    {
        private RouteGalleryViewModel _vm;

        public RouteGalleryPage(string routeId)
        {
            InitializeComponent();
            _vm = new RouteGalleryViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {

        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {

        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}
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
    public partial class RoutePointV2Page : ContentPage
    {
        IToolbarService _toolbarService = DependencyService.Get<IToolbarService>();
        private RoutePointV2ViewModel _vm;
        
        public RoutePointV2Page()
        {
            InitializeComponent();
        }
        public RoutePointV2Page(string routeId, string routePointId)
        {
            InitializeComponent();
            RoutePointManager manager = new RoutePointManager();
            _vm = new RoutePointV2ViewModel(routeId, routePointId) 
            { 
                Navigation = this.Navigation 
            };
            BindingContext = _vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _toolbarService.SetVisibilityToolbar(false);
            _vm.StartDialog();
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}
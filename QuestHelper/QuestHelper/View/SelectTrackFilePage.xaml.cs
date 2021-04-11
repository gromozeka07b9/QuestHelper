using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectTrackFilePage : ContentPage
    {
        private SelectTrackFileViewModel _vm;
        public SelectTrackFilePage(string routeId)
        {
            InitializeComponent();
            _vm = new SelectTrackFileViewModel(routeId);
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
        }

        private void SelectTrackFilePage_OnAppearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
        }

        private void SelectTrackFilePage_OnDisappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}
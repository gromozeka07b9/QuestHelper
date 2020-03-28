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
    public partial class EditPoiPage : ContentPage
    {
        EditPoiViewModel _vm;
        public EditPoiPage(string poiId, string routePointId)
        {
            InitializeComponent();
            _vm = new EditPoiViewModel(poiId, routePointId);
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
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
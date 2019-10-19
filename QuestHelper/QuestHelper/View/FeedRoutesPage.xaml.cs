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
    public partial class FeedRoutesPage : ContentPage
    {
        private FeedRoutesViewModel _vm;
        public FeedRoutesPage()
        {
            InitializeComponent();
            _vm = new FeedRoutesViewModel() { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void FeedRoutesPage_OnAppearing(object sender, EventArgs e)
        {
            _vm.startDialog();
        }

        private void FeedRoutesPage_OnDisappearing(object sender, EventArgs e)
        {
            _vm.closeDialog();
        }
    }
}
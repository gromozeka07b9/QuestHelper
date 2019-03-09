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
    public partial class ShareRoutePage : ContentPage
    {
        ShareRouteViewModel _vm;
        public ShareRoutePage(string routeId)
        {
            InitializeComponent();
            _vm = new ShareRouteViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTxt = e.NewTextValue.ToString().Trim();
            if (!string.IsNullOrEmpty(searchTxt))
            {
                _vm.FilterUsersByTextAsync(searchTxt);
            }
        }
    }
}
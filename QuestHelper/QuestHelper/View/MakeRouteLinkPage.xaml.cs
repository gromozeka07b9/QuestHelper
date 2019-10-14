using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model.Messages;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MakeRouteLinkPage : ContentPage
    {
        private MakeRouteLinkPageViewModel _vm;
        public MakeRouteLinkPage(string routeId)
		{
			InitializeComponent ();
            _vm = new MakeRouteLinkPageViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
		}

        private void MakeRouteLinkPage_OnAppearing(object sender, EventArgs e)
        {
            _vm.StartDialogAsync();
        }

        private void MakeRouteLinkPage_OnDisappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}
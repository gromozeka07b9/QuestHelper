using QuestHelper.LocalDB.Model;
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
    public partial class NewRoutePage : ContentPage
	{
        NewRouteViewModel vm;
        public NewRoutePage()
		{
            InitializeComponent ();
            vm = new NewRouteViewModel(false) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public NewRoutePage(bool isFirstRoute)
        {

            InitializeComponent();
            vm = new NewRouteViewModel(isFirstRoute) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
        }
    }
}
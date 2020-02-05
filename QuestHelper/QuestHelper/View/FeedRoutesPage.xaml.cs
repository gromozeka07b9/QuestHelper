using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers;
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
            if (IsNeedShowOnboarding())
            {
                ParameterManager par = new ParameterManager();
                par.Set("NeedShowOnboarding", "0");
                Navigation.PushModalAsync(new NavigationPage(new SplashWizardPage()));
            }
            else
            {
                _vm.startDialogAsync();
            }
        }
        private bool IsNeedShowOnboarding()
        {
            ParameterManager par = new ParameterManager();
            string showOnboarding = string.Empty;
            if (par.Get("NeedShowOnboarding", out showOnboarding))
            {
                return showOnboarding.Equals("1");
            }
            return false;
        }

        private void FeedRoutesPage_OnDisappearing(object sender, EventArgs e)
        {
            _vm.closeDialog();
        }
    }
}
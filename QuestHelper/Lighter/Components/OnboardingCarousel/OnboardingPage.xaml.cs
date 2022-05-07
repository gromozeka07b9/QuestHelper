using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lighter.Components.OnboardingCarousel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public sealed partial class OnboardingPage : ContentPage
    {
        private readonly OnboardingViewModel ViewModel;
        
        public OnboardingPage(Models.OnboardingCarousel onboardingCarouselModel)
        {
            InitializeComponent();
            ViewModel = new OnboardingViewModel() { Navigation = this.Navigation};
            ViewModel.UpdateCarouselCollection(onboardingCarouselModel);
            BindingContext = ViewModel;
        }


        private void OnboardingPage_OnAppearing(object sender, EventArgs e)
        {
            ViewModel.StartDialog();
        }
    }
}
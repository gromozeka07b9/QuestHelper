using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Lighter.Events;
using Xamarin.Forms;

namespace Lighter.Components.OnboardingCarousel
{
    public sealed class OnboardingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OnBoardingModel> OnBoardingModelCollection { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand SkipCommand { get; private set; }
        public string ApplicationName { get; private set; }
        public string ApplicationIconName { get; private set; }
        public bool CanSkipEnabled { get; private set; }
        public string SkipText { get; private set; }

        internal OnboardingViewModel()
        {
            SkipCommand = new Command(SkipOnboardingAsync);
        }

        private async void SkipOnboardingAsync(object obj)
        {
            MessagingCenter.Send(new StartApplicationEvent(), String.Empty);
        }

        internal Task UpdateCarouselCollection(Models.OnboardingCarousel onboardingCarouselModel)
        {
            ApplicationName = onboardingCarouselModel.Application.Name;
            ApplicationIconName = onboardingCarouselModel.Application.IconFilename;
            CanSkipEnabled = onboardingCarouselModel.CanSkipEnabled;
            SkipText = onboardingCarouselModel.SkipText.Content;
            var models = onboardingCarouselModel.OnboardingScreens.Select(i => new OnBoardingModel()
            {
                ImgSource = i.Image.Filename,
                HeadlineText = i.Headline.Content,
                SubheadText = i.Subhead.Content,
                HeadlineTextFontSize = Convert.ToDouble(i.Headline.FontSize),
                SubheadTextFontSize = Convert.ToDouble(i.Subhead.FontSize),
                HeadlineTextColor = Color.FromHex(i.Headline.Color),
                SubheadTextColor = Color.FromHex(i.Subhead.Color),
                ScreenTopColor = Color.FromHex(i.TopColor),
                ScreenBottomColor = Color.FromHex(i.BottomColor)
            });
            OnBoardingModelCollection = new ObservableCollection<OnBoardingModel>(models);
            return Task.CompletedTask;
        }

        internal void StartDialog()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lighter.Components.OnboardingCarousel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingCarouselPage : CarouselPage
    {
        private Models.OnboardingCarousel onboardingCarouselModel;
        public OnboardingCarouselPage(Models.OnboardingCarousel onboardingCarouselModel)
        {
            this.onboardingCarouselModel = onboardingCarouselModel;
            InitializeComponent();
            BuildContent();
            
        }

        private void OnboardingCarouselPage_OnAppearing(object sender, EventArgs e)
        {
            foreach (var screen in onboardingCarouselModel.OnboardingScreens)
            {
                /*var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(8, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(5, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(3, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(2, GridUnitType.Star)});*/
                //var label = new Label();
                //label.Text = screen.Headline.Content;
                /*var contentPage = new ContentPage();
                var onboardingView = new OnboardingView();
                onboardingView.Text = screen.Headline.Content;
                onboardingView.ImgSource = screen.Image.Filename;
                //grid.Children.Add(onboardingView, 1, 1);
                contentPage.Content = onboardingView;
                this.Children.Add(contentPage);*/
            }
        }

        void BuildContent()
        {
            foreach (var screen in onboardingCarouselModel.OnboardingScreens)
            {
                var contentPage = new ContentPage();
                /*var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(8, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(5, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(3, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(2, GridUnitType.Star)});*/
                //var label = new Label();
                //label.Text = screen.Headline.Content;
                var onboardingView = new OnboardingView();
                onboardingView.Text = screen.Headline.Content;
                onboardingView.ImgSource = screen.Image.Filename;
                //grid.Children.Add(onboardingView, 1, 1);
                contentPage.Content = onboardingView;
                this.Children.Add(contentPage);
            }            
        }
    }
}
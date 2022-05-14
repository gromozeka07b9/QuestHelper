using Xamarin.Forms;

namespace Lighter.Components.OnboardingCarousel
{
    public class OnBoardingModel
    {
        public string ImgSource { get; set; }
        public string HeadlineText { get; set; }
        public string SubheadText { get; set; }
        public double HeadlineTextFontSize { get; set; }
        public double SubheadTextFontSize { get; set; }
        public Color ScreenTopColor { get; set; }
        public Color ScreenBottomColor { get; set; }
        public Color HeadlineTextColor { get; set; }
        public Color SubheadTextColor { get; set; }
    }
}
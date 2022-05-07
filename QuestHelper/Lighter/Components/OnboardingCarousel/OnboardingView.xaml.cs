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
    public partial class OnboardingView : Grid
    {
        public static readonly BindableProperty HeadlineTextFontSizeProperty = BindableProperty.Create(nameof(HeadlineTextFontSize), typeof(double), typeof(OnboardingView));
        public static readonly BindableProperty SubheadTextFontSizeProperty = BindableProperty.Create(nameof(SubheadTextFontSize), typeof(double), typeof(OnboardingView));
        public static readonly BindableProperty HeadlineTextProperty = BindableProperty.Create(nameof(HeadlineText), typeof(string), typeof(OnboardingView));
        public static readonly BindableProperty SubheadTextProperty = BindableProperty.Create(nameof(SubheadText), typeof(string), typeof(OnboardingView));
        public static readonly BindableProperty ImgSourceProperty = BindableProperty.Create(nameof(ImgSource), typeof(string), typeof(OnboardingView));
        public static readonly BindableProperty ScreenTopColorProperty = BindableProperty.Create(nameof(ScreenTopColor), typeof(Color), typeof(OnboardingView));
        public static readonly BindableProperty ScreenBottomColorProperty = BindableProperty.Create(nameof(ScreenBottomColor), typeof(Color), typeof(OnboardingView));

        public OnboardingView()
        {
            InitializeComponent();
        }

        public double HeadlineTextFontSize
        {
            get => (double) GetValue(HeadlineTextFontSizeProperty);
            set => SetValue(HeadlineTextFontSizeProperty, value);
        }
        public double SubheadTextFontSize
        {
            get => (double) GetValue(SubheadTextFontSizeProperty);
            set => SetValue(SubheadTextFontSizeProperty, value);
        }

        public string HeadlineText
        {
            get => (string) GetValue(HeadlineTextProperty);
            set => SetValue(HeadlineTextProperty, value);
        }
        public string SubheadText
        {
            get => (string) GetValue(SubheadTextProperty);
            set => SetValue(SubheadTextProperty, value);
        }
        public string ImgSource
        {
            get => (string)GetValue(ImgSourceProperty);
            set => SetValue(ImgSourceProperty, value);
        }
        public Color ScreenTopColor
        {
            get => (Color)GetValue(ScreenTopColorProperty);
            set => SetValue(ScreenTopColorProperty, value);
        }
        public Color ScreenBottomColor
        {
            get => (Color)GetValue(ScreenBottomColorProperty);
            set => SetValue(ScreenBottomColorProperty, value);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingView : Grid
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(OnboardingView));
        public static readonly BindableProperty ImgSourceProperty = BindableProperty.Create(nameof(ImgSource), typeof(string), typeof(OnboardingView));

        public OnboardingView()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public string ImgSource
        {
            get => (string)GetValue(ImgSourceProperty);
            set => SetValue(ImgSourceProperty, value);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using QuestHelper.ViewModel;
using Xamarin.Forms;
using static QuestHelper.Model.AutoGeneratedRouted;

namespace QuestHelper.View
{
    public partial class PreviewRoutePointImagesPage : ContentPage
    {
        PreviewRoutePointImagesViewModel _vm;

        public PreviewRoutePointImagesPage()
        {
            InitializeComponent();
        }

        public PreviewRoutePointImagesPage(AutoGeneratedPoint selectedPoint)
        {
            InitializeComponent();
            _vm = new PreviewRoutePointImagesViewModel(selectedPoint);
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
        }

        void ContentPage_Appearing(System.Object sender, System.EventArgs e)
        {
            _vm.StartDialog();
        }

        void ContentPage_Disappearing(System.Object sender, System.EventArgs e)
        {
            _vm.CloseDialog();
        }

        void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            sfListImages.ResetSwipe(true);
            var eventArgs = ((TappedEventArgs)e);
            _vm.ToggleDeleteImage(eventArgs.Parameter as AutoGeneratedImage);
        }
    }
}

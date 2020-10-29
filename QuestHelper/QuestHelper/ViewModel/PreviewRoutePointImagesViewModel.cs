﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using QuestHelper.Model;
using Xamarin.Forms;
using static QuestHelper.Model.AutoGeneratedRouted;

namespace QuestHelper.ViewModel
{
    public class PreviewRoutePointImagesViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ToggleDeleteImageCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand OpenImageInViewerCommand { get; private set; }
        

        private AutoGeneratedPoint _selectedPoint;

        public PreviewRoutePointImagesViewModel(AutoGeneratedRouted.AutoGeneratedPoint selectedPoint)
        {
            ToggleDeleteImageCommand = new Command(toggleDeleteImageCommand);
            BackNavigationCommand = new Command(backNavigationCommand);
            OpenImageInViewerCommand = new Command(openImageInViewerCommand);
            _selectedPoint = selectedPoint;
        }

        private void openImageInViewerCommand(object obj)
        {
            var currentImage = (AutoGeneratedImage)obj;
            string path = currentImage.ImageOriginalFullPath;
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            if (!string.IsNullOrEmpty(path))
            {
                defaultViewerService.Show(path.Replace("_preview", ""));
            }
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        private void toggleDeleteImageCommand(object obj)
        {
            var currentImage = (AutoGeneratedImage)obj;
            currentImage.IsDeleted = !currentImage.IsDeleted;
            if (!_selectedPoint.Images.Where(i=>!i.IsDeleted).Any()) _selectedPoint.IsDeleted = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
        }

        public void ToggleDeleteImage(AutoGeneratedImage image)
        {
            toggleDeleteImageCommand(image);
        }

        public void CloseDialog()
        {
            
        }

        public void StartDialog()
        {

        }

        public ObservableCollection<AutoGeneratedImage> Images
        {
            get
            {
                return _selectedPoint.Images;
            }
        }
    }
}
﻿using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Resources;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Syncfusion.SfChart.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Syncfusion.ListView.XForms;
using static QuestHelper.Model.AutoGeneratedRouted;
using QuestHelper.Model.Messages;

namespace QuestHelper.ViewModel
{
    public class MakeNewRouteAutoViewModel : INotifyPropertyChanged, IDialogEvents
    {
        TokenStoreService _tokenService = new TokenStoreService();
        private string _currentUserId;
        //private ObservableCollection<ViewLocalFile> _newRouteImgCollection = new ObservableCollection<ViewLocalFile>();
        private AutoGeneratedRouted _autoGeneratedRoute;
        private DateTime _periodRouteBegin = DateTime.Now.AddDays(-7);
        private DateTime _periodRouteEnd = DateTime.Now;
        private LocalFileCacheManager _localFileCacheManager = new LocalFileCacheManager();
        private DateTime _minRangeDate = DateTime.Now;
        private int _selectedImagesCount = 0;
        private bool _isGalleryIndexed = false;
        private bool _isVisiblePeriodChart = false;
        //private AutoGeneratedPoint _selectedPreviewRoutePoint;
        //private List<ViewLocalFile> _selectedPreviewPointImages;
        private int _countImagesForToday;
        private int _countImagesFor1Day;
        private int _countImagesFor7Day;
        private int _countImagesForAllDays;
        private bool _isRouteMaking = false;
        private int _maxCountForWarning = 100;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GenerateNewRouteCommand { get; private set; }
        public ICommand StartIndexGalleryCommand { get; private set; }
        public ICommand ShowPeriodChartCommand { get; private set; }
        public ICommand ShowMinimalPeriodCommand { get; private set; }
        public ICommand ShowImagesPreviewPointCommand { get; private set; }
        public ICommand SaveRouteCommand { get; private set; }

        public bool IsBusy { get; set; }

        public ObservableCollection<ChartDataPoint> ImagesRangeData { get; set; }

        public MakeNewRouteAutoViewModel()
        {
            GenerateNewRouteCommand = new Command(generateNewRouteCommand);
            StartIndexGalleryCommand = new Command(startIndexGalleryCommand);
            ShowPeriodChartCommand = new Command(showPeriodChartCommand);
            ShowMinimalPeriodCommand = new Command(showMinimalPeriodCommand);
            ShowImagesPreviewPointCommand = new Command(showImagesPreviewPointCommand);
            SaveRouteCommand = new Command(saveRouteCommand);
            TokenStoreService tokenService = new TokenStoreService();
        }

        private void saveRouteCommand(object obj)
        {
            AutoRouteMakerManager maker = new AutoRouteMakerManager(new ImageManager());
            bool makeResult = maker.Make(_autoGeneratedRoute, _currentUserId);
            if (makeResult)
            {
                UserDialogs.Instance.Toast("Маршрут создан");
                MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() {  PageToOpen = MainPages.Private}, string.Empty);
            }
        }

        private void showImagesPreviewPointCommand(object obj)
        {
            AutoGeneratedPoint selectedPoint = new AutoGeneratedPoint();
            if(obj is AutoGeneratedImage)
            {
                AutoGeneratedImage selectedImage = (AutoGeneratedImage)obj;
                selectedPoint = PreviewRoutePoints.Where(p => p.Images.Contains(selectedImage)).SingleOrDefault();
            }
            else
            {
                selectedPoint = (AutoGeneratedPoint)obj;
            }
            OpenImagesPreviewPoint(selectedPoint);
        }

        public void OpenImagesPreviewPoint(AutoGeneratedPoint selectedPoint)
        {
            Navigation.PushModalAsync(new PreviewRoutePointImagesPage(selectedPoint));
        }

        private void showMinimalPeriodCommand(object obj)
        {
            //bool result = await askToSaveRoute();
            //IsVisiblePeriodChart = result ? false : true;
            IsVisiblePeriodChart = false;
        }

        private void showPeriodChartCommand(object obj)
        {
            //bool result = await askToSaveRoute();
            IsVisiblePeriodChart = true;
        }

        private static async Task<bool> askToSaveRoute()
        {
            return await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Title = "Фотоальбом еще не сохранен", Message = "Продолжить без сохранения?", OkText = "Да", CancelText = "Отменить" });
        }

        private static async Task<bool> askToMaxImageCountWarning()
        {
            return await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Title = "Выбрано большое количество фотографий", Message = "Создание альбома может занять продолжительное время. Продолжить?", OkText = "Да", CancelText = "Отменить" });
        }

        private async void startIndexGalleryCommand(object obj)
        {
            //UserDialogs.Instance.Toast("Индексирование галереи...");
            IsGalleryIndexed = false;
            ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), PeriodRouteBegin, PeriodRouteEnd);
            imagesCache.UpdateFilenames();
            MinRangeDate = _localFileCacheManager.GetMinDate();
            IsGalleryIndexed = true;
            await Task.Factory.StartNew(() => {
                updateRangeContent();
                _countImagesForToday = imagesCache.GetCountImagesForDaysAgo(0);
                _countImagesFor1Day = imagesCache.GetCountImagesForDaysAgo(1);
                _countImagesFor7Day = imagesCache.GetCountImagesForDaysAgo(7);
                _countImagesForAllDays = imagesCache.GetCountImagesForDaysAgo(1000);
                Device.BeginInvokeOnMainThread(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesForToday"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesFor1Day"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesFor7Day"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesForAllDays"));
                });
            });
        }

        private async void generateNewRouteCommand(object obj)
        {
            _autoGeneratedRoute = null;

            if(obj != null)
            {
                int daysDepth = 0;
                try
                {
                    daysDepth = Convert.ToInt32(obj);
                }
                catch (Exception)
                {
                }

                setPeriodByDepth(daysDepth);

            }

            bool maxCountOk = true;
            if(_selectedImagesCount > _maxCountForWarning)
            {
                maxCountOk = await askToMaxImageCountWarning();
            }
            if (maxCountOk)
            {
                IsRouteMaking = true;
                await Task.Factory.StartNew(() =>
                 {
                     ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), PeriodRouteBegin, PeriodRouteEnd);
                     imagesCache    .UpdateMetadata();
                     AutoRoutePreviewMakerManager routeMaker = new AutoRoutePreviewMakerManager(new LocalFileCacheManager());
                     _autoGeneratedRoute = routeMaker.Make(PeriodRouteBegin, PeriodRouteEnd);
                     Device.BeginInvokeOnMainThread(() =>
                     {
                         IsRouteMaking = false;
                         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewRoutePoints"));
                         UpdateRouteInfo();
                     });
                 });
            }

        }

        private void setPeriodByDepth(int daysDepth)
        {
            var currentDate = DateTime.Now;
            var dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            PeriodRouteBegin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day - daysDepth, 0, 0, 0);
            PeriodRouteEnd = dateEnd;
        }

        public void UpdateRouteInfo()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPreviewRouteMade"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointCount"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageCount"));
        }

        public void UpdateSelectedCountDays(DateTime minDate, DateTime maxDate)
        {
            var countByDays = _localFileCacheManager.GetCountImagesByDay(minDate, maxDate).OrderBy(c=>c.Item1);
            _selectedImagesCount = countByDays.Sum(x=>x.Item2);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPeriodText"));
        }

        public void CloseDialog()
        {
         
        }

        public async void StartDialog()
        {
            //if (string.IsNullOrEmpty(_currentUserId))
            {
                _currentUserId = await _tokenService.GetUserIdAsync();
            }

            if(!IsGalleryIndexed)
            {
                setPeriodByDepth(14);
                startIndexGalleryCommand(new object());
                await Task.Factory.StartNew(() => updateRangeContent());
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewRoutePoints"));
        }

        private void updateRangeContent()
        {
            var countByDays = _localFileCacheManager.GetCountImagesByDay(MinRangeDate, PeriodRouteEnd).OrderBy(c=>c.Item1);
            ImagesRangeData = new ObservableCollection<ChartDataPoint>(countByDays.Select(c => new ChartDataPoint(c.Item1, c.Item2)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesRangeData"));
            UpdateSelectedCountDays(PeriodRouteBegin, PeriodRouteEnd);
        }

        
        public bool IsPreviewRouteMade
        {
            get
            {
                return PreviewRoutePoints.Where(p=>!p.IsDeleted).Count() > 0;
            }
        }

        public bool IsGalleryIndexed
        {
            set
            {
                if (value != _isGalleryIndexed)
                {
                    _isGalleryIndexed = value;
                }
            }
            get
            {
                return _isGalleryIndexed;
            }
        }

        public bool IsRouteMaking
        {
            set
            {
                if(value != _isRouteMaking)
                {
                    _isRouteMaking = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRouteMaking"));
                }
            }
            get
            {
                return _isRouteMaking;
            }
        }

        public string SelectedPeriodText
        {
            get
            {
                return $"С {PeriodRouteBegin.ToString("d MMM")} по {PeriodRouteEnd.ToString("d MMM")}, выбрано изображений: {_selectedImagesCount.ToString()}";
            }    
        }

        public ObservableCollection<AutoGeneratedRouted.AutoGeneratedPoint> PreviewRoutePoints
        {
            get
            {
                if(_autoGeneratedRoute!=null)
                {
                    return _autoGeneratedRoute.Points;
                }
                return new ObservableCollection<AutoGeneratedPoint>();
            }
        }

        public bool IsVisiblePeriodChart
        {
            set
            {
                if (value != _isVisiblePeriodChart)
                {
                    _isVisiblePeriodChart = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisiblePeriodChart"));
                }
            }
            get
            {
                return _isVisiblePeriodChart;
            }
        }

        public DateTime MinRangeDate
        {
            set
            {
                if (value != _minRangeDate)
                {
                    _minRangeDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinRangeDate"));
                }
            }
            get
            {
                return _minRangeDate;
            }
        }
        
        public DateTime MaxRangeDate
        {
            get => DateTime.Now;
        }

        public DateTime PeriodRouteBegin
        {
            set
            {
                if (value != _periodRouteBegin)
                {
                    _periodRouteBegin = value;
                    UpdateSelectedCountDays(_periodRouteBegin, PeriodRouteEnd);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PeriodRouteBegin"));
                }
            }
            get
            {
                return _periodRouteBegin;
            }
        }
        public DateTime PeriodRouteEnd
        {
            set
            {
                if (value != _periodRouteEnd)
                {
                    _periodRouteEnd = value;
                    UpdateSelectedCountDays(PeriodRouteBegin, _periodRouteEnd);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PeriodRouteEnd"));
                }
            }
            get
            {
                return _periodRouteEnd;
            }
        }

        public int ImageCount
        {
            get
            {
                var images = PreviewRoutePoints.Where(p => !p.IsDeleted).Select(p => new { count = p.Images.Where(i=>!i.IsDeleted).Select(i=>i).Count()});
                var total = images.Sum(p=>p.count);
                return total;
            }
        }

        public int PointCount
        {
            get
            {
                var total = PreviewRoutePoints.Where(p => !p.IsDeleted).Count();
                return total;
            }
        }

        public string CountImagesForToday
        {
            get
            {
                return $"фото: {_countImagesForToday}";
            }
        }

        public string CountImagesFor1Day
        {
            get
            {
                return $"фото: {_countImagesFor1Day}";
            }
        }

        public string CountImagesFor7Day
        {
            get
            {
                return $"фото: {_countImagesFor7Day}";
            }
        }

        public string CountImagesForAllDays
        {
            get
            {
                return $"фото: {_countImagesForAllDays}";
            }
        }


    }

}

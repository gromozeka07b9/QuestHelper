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

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GenerateNewRouteCommand { get; private set; }
        public ICommand StartIndexGalleryCommand { get; private set; }
        public ICommand ShowPeriodChartCommand { get; private set; }
        public ICommand ShowMinimalPeriodCommand { get; private set; }
        //public ICommand DeletePreviewPointCommand { get; private set; }

        public bool IsBusy { get; set; }

        public ObservableCollection<ChartDataPoint> ImagesRangeData { get; set; }

        public MakeNewRouteAutoViewModel()
        {
            GenerateNewRouteCommand = new Command(generateNewRouteCommand);
            StartIndexGalleryCommand = new Command(startIndexGalleryCommand);
            ShowPeriodChartCommand = new Command(showPeriodChartCommand);
            ShowMinimalPeriodCommand = new Command(showMinimalPeriodCommand);
            //DeletePreviewPointCommand = new Command(deletePreviewPointCommand);
            TokenStoreService tokenService = new TokenStoreService();
        }

        private void showMinimalPeriodCommand(object obj)
        {
            IsVisiblePeriodChart = false;
        }

        private void showPeriodChartCommand(object obj)
        {
            IsVisiblePeriodChart = true;
        }

        private async void startIndexGalleryCommand(object obj)
        {
            UserDialogs.Instance.Toast("Индексирование галереи...");
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

        private void generateNewRouteCommand(object obj)
        {
            _autoGeneratedRoute = null;

            int daysDepth = 0;
            try
            {
                daysDepth = Convert.ToInt32(obj);
            }
            catch (Exception)
            {
            }

            setPeriodByDepth(daysDepth);

            UserDialogs.Instance.Toast("Построение альбома...");
            ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), PeriodRouteBegin, PeriodRouteEnd);
            imagesCache.UpdateMetadata();

            AutoRoutePreviewMakerManager routeMaker = new AutoRoutePreviewMakerManager(new LocalFileCacheManager());
            _autoGeneratedRoute = routeMaker.Make(PeriodRouteBegin, PeriodRouteEnd);
            //NewRouteImgCollection = new ObservableCollection<ViewLocalFile>(_autoGeneratedRoute.Points.Select(p => p.Images[0]));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewRoutePoints"));
        }

        private void setPeriodByDepth(int daysDepth)
        {
            var currentDate = DateTime.Now;
            var dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            PeriodRouteBegin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day - daysDepth, 0, 0, 0);
            PeriodRouteEnd = dateEnd;
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
            _currentUserId = await _tokenService.GetUserIdAsync();
            if(!IsGalleryIndexed)
            {
                setPeriodByDepth(14);
                startIndexGalleryCommand(new object());
            }

            //generateNewRouteCommand(0);
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewRoutePoints"));
            /*ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), PeriodRouteBegin, PeriodRouteEnd);
            imagesCache.UpdateFilenames();
            MinRangeDate = _localFileCacheManager.GetMinDate();
            await Task.Factory.StartNew(() => updateRangeContentByTimer());*/
        }

        private void updateRangeContent()
        {
            var countByDays = _localFileCacheManager.GetCountImagesByDay(MinRangeDate, PeriodRouteEnd).OrderBy(c=>c.Item1);
            ImagesRangeData = new ObservableCollection<ChartDataPoint>(countByDays.Select(c => new ChartDataPoint(c.Item1, c.Item2)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesRangeData"));
            UpdateSelectedCountDays(PeriodRouteBegin, PeriodRouteEnd);
        }
        
        public bool IsGalleryIndexed
        {
            set
            {
                if(value != _isGalleryIndexed)
                {
                    _isGalleryIndexed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsGalleryIndexed"));
                }
            }
            get
            {
                return _isGalleryIndexed;
            }
        }

        /*public bool IsLoadingNewRouteData
        {
            get
            {
                return NewRouteImgCollection.Count == 0;
            }
        }*/

        public string SelectedPeriodText
        {
            get
            {
                return $"Количество фотографий в галерее по дням. Выбран период с {PeriodRouteBegin} по {PeriodRouteEnd}, изображений: {_selectedImagesCount.ToString()}";
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

        /*public ObservableCollection<ViewLocalFile> NewRouteImgCollection
        {
            set
            {
                if(value != _newRouteImgCollection)
                {
                    _newRouteImgCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewRouteImgCollection"));
                }
            }
            get
            {
                return _newRouteImgCollection;
            }

        }*/

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

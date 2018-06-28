﻿using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Model.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CreateCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        RoutePoint _point = new RoutePoint();
        string _currentPositionString = string.Empty;

        public RoutePointViewModel()
        {
            CreateCommand = new Command(createRoutePoint);
            UpdateCommand = new Command(updateRoutePoint);
            DeleteCommand = new Command(deleteRoutePoint);
            fillCurrentPositionAsync();
            //currentPosition.
        }

        private async void fillCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            _point.Latitude = position.Latitude;
            _point.Longitude = position.Longitude;
            Coordinates = _point.Latitude + "," + _point.Longitude;
            //Coordinates = _currentPositionString;
        }

        /*private async Task<Position> GetCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            return position;
        }*/

        async void createRoutePoint()
        {
        }

        async void updateRoutePoint()
        {
        }
        async void deleteRoutePoint()
        {

        }

        public string Coordinates
        {
            set
            {
                if (_currentPositionString != value)
                {
                    _currentPositionString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                }
            }
            get
            {
                return _currentPositionString;
            }
        }
        public string Name
        {
            set
            {
                if (_point.Name != value)
                {
                    _point.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _point.Name;
            }
        }
        public string Description
        {
            set
            {
                if (_point.Description != value)
                {
                    _point.Description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
            get
            {
                return _point.Description;
            }
        }
    }
}

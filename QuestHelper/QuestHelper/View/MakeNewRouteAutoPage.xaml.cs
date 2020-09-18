﻿using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.RangeNavigator.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.ListView.XForms;
using static QuestHelper.Model.AutoGeneratedRouted;
using QuestHelper.Model;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MakeNewRouteAutoPage : ContentPage
    {
        MakeNewRouteAutoViewModel _vm;
        SfListView _listView;
        private CollectionView _collectionImagesFirstView;
        private AutoGeneratedPoint _currentPoint = new AutoGeneratedPoint();

        public MakeNewRouteAutoPage()
        {
            InitializeComponent();
            _vm = new MakeNewRouteAutoViewModel();
            _vm.Navigation = this.Navigation;
            _listView = FindByName("sfListRoutePoints") as SfListView;
            _collectionImagesFirstView = FindByName("CollectionViewImagesFirst") as CollectionView;
            BindingContext = _vm;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.StartDialog();

            if(_listView.DataSource.Items.Count() > 0)
            {
                //_listView.DataSource.Refresh();
                //_listView.RefreshListViewItem();
                /*Device.BeginInvokeOnMainThread(() => {
                    _listView.RefreshListViewItem(0,0,canReload: true);
                });*/
            }
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }

        private void SfDateTimeRangeNavigator_OnRangeChanged(object sender, RangeChangedEventArgs e)
        {
            _vm.PeriodRouteBegin = e.ViewRangeStartDate;
            _vm.PeriodRouteEnd = e.ViewRangeEndDate.AddDays(1).Date.AddSeconds(-1);
            _vm.UpdateSelectedCountDays(_vm.PeriodRouteBegin, _vm.PeriodRouteEnd);
        }

        /*void ListItemTapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            var selectedItem = ((Xamarin.Forms.TappedEventArgs)e).Parameter;
            AutoGeneratedPoint selectedPoint = (AutoGeneratedPoint)selectedItem;
            var indexItem = _listView.DataSource.DisplayItems.IndexOf(selectedPoint);
            selectedPoint.IsSelected = !selectedPoint.IsSelected;
            
            //_vm.SelectedPreviewRoutePoint = selectedPoint;
            Device.BeginInvokeOnMainThread(()=> { _listView.RefreshListViewItem(); });
            _vm.OpenImagesPreviewPoint(selectedPoint);

        }*/

        void TapGestureRecognizerDelete_Tapped(System.Object sender, System.EventArgs e)
        {
            sfListRoutePoints.ResetSwipe(true);
            var selectedItem = ((Xamarin.Forms.TappedEventArgs)e).Parameter;
            AutoGeneratedPoint selectedPoint = (AutoGeneratedPoint)selectedItem;
            var indexItem = _listView.DataSource.DisplayItems.IndexOf(selectedPoint);
            selectedPoint.IsDeleted = !selectedPoint.IsDeleted;
            Device.BeginInvokeOnMainThread(() => {
                _listView.RefreshListViewItem(indexItem, indexItem, true);
                _vm.UpdateRouteInfo();
            });
        }

    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using QuestHelper.Managers;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using Syncfusion.DataSource.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private bool _isUsageMainMemory;
        private ObservableCollection<FileSystemElement> _sourcePath = new ObservableCollection<FileSystemElement>();
        private string _pathToImagesDir = string.Empty;
        private bool _navigationToRootIsVisible = false;
        private string _pathToCustomDCIM = String.Empty;
        private string _pathToDefaultDCIM = String.Empty;
        private string _initDCIMDirectory = String.Empty;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand MainMemoryCheckBoxCommand { get; private set; }
        //public ICommand ExtMemoryCheckBoxCommand { get; private set; }
        public ICommand NavigateDirUpCommand { get; private set; }
        public ICommand ChooseDirCommand { get; private set; }
        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            MainMemoryCheckBoxCommand = new Command(mainMemoryCheckBoxCommand);
            //ExtMemoryCheckBoxCommand = new Command(extMemoryCheckBoxCommand);
            NavigateDirUpCommand = new Command(navigateDirUpCommand);
            ChooseDirCommand = new Command(chooseDirCommand);
        }

        private void OnCollectionViewRemainingItemsThresholdReached(object sender, EventArgs e)
        {
            
        }
        private void chooseDirCommand(object obj)
        {
            var newDir = (FileSystemElement)obj;
            if (newDir != null)
            {
                updateDirContent(new DirectoryInfo(Path.Combine(_pathToImagesDir, newDir.Path)));   
            }
        }

        private void navigateDirUpCommand(object obj)
        {
            var currentDirectory = Directory.GetParent(_pathToImagesDir);
            if ((currentDirectory != null) && (currentDirectory.Exists))
            {
                updateDirContent(currentDirectory);
            }
        }

        private void updateDirContent(DirectoryInfo currentDirectory)
        {
            (var files, var dirs, bool userHaveAccess) = getDirContent(currentDirectory);

            if (userHaveAccess)
            {
                SourcePaths =
                    new ObservableCollection<FileSystemElement>(dirs
                        .Select(i => new FileSystemElement() {Path = i, IsFile = false}).ToList());
                addFilesToSourcePaths(files, 10);

                _pathToImagesDir = currentDirectory.FullName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourcePaths"));
                CountOfPhotoCustomDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoCustomDCIM"));

                PathToCustomDCIM = _pathToImagesDir;
                var topDirectory = Directory.GetParent(currentDirectory.FullName);
                NavigationToRootIsVisible = (topDirectory != null) && (topDirectory.Exists);
            }
            else NavigationToRootIsVisible = false;
        }

        private void addFilesToSourcePaths(ObservableCollection<string> items, int count)
        {
            int index = 0;
            while (index < count)
            {
                if (items.ElementAtOrDefault(index) != null)
                {
                    SourcePaths.Add(new FileSystemElement()
                    {
                        Path = items.ElementAt(index),
                        IsFile = true
                    });
                }
                else
                {
                    break;
                }

                index++;
            }
        }

        private (ObservableCollection<string>, ObservableCollection<string>, bool userHaveAccess) getDirContent(DirectoryInfo currentDirectory)
        {
            ObservableCollection<string> files = new ObservableCollection<string>();
            ObservableCollection<string> dirs = new ObservableCollection<string>();
            bool userHaveAccess = false; 
            try
            {
                files = currentDirectory.GetFiles("*.jpg").OrderBy(f => f.CreationTime)
                    .Select(f => f.FullName).ToArray().ToObservableCollection();
                dirs = currentDirectory.GetDirectories().OrderBy(d => d.Name).Select(d => d.Name)
                    .ToObservableCollection();
                userHaveAccess = true;
            }
            catch (IOException ioException)
            {
            }
            catch (UnauthorizedAccessException e)
            {
            }
            return (files, dirs, userHaveAccess);
        }

        private void mainMemoryCheckBoxCommand(object obj)
        {

        }

        private void backNavigationCommand(object obj)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                int totalCount = 0;
                if (IsUsageMainMemory)
                {
                    totalCount = CountOfPhotoDefaultDCIM;
                }
                else
                {
                    totalCount = CountOfPhotoCustomDCIM;                    
                }

                if (totalCount == 0)
                {
                    var answerYes = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig(){Message = "Вы выбрали источник без фотографий. Отменить изменения?", Title = "Внимание! Нет фотографий", OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_Cancel});
                    if(answerYes)    
                        Navigation.PopModalAsync();
                }
                else
                {
                    string newPathToImages = string.Empty;
                    if ((IsUsageMainMemory) && (_initDCIMDirectory.Equals(PathToDefaultDCIM)))
                    {
                        newPathToImages = PathToDefaultDCIM;
                    }
                    if ((!IsUsageMainMemory) && (!_initDCIMDirectory.Equals(PathToCustomDCIM)))
                    {
                        newPathToImages = PathToCustomDCIM;
                    }

                    if (!newPathToImages.Equals(_initDCIMDirectory))
                    {
                        var answerYes = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig(){Message = "Сохранить изменения?", Title = "Выбран источник фотографий", OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No});
                        if (answerYes)
                        {
                            ParameterManager parameterManager = new ParameterManager();
                            parameterManager.Set("CameraDirectoryFullPath", newPathToImages);
                            Xamarin.Forms.MessagingCenter.Send<ImagesPathLocationChangedMessage>(new ImagesPathLocationChangedMessage() { FullPath = newPathToImages}, string.Empty);

                        }                        
                    }
                    Navigation.PopModalAsync();
                }
            });
        }

        public async void StartDialog()
        {
            PermissionManager permissions = new PermissionManager();
            var taskPermissionRead = await permissions.CheckAndRequestStorageReadPermission();
            if (taskPermissionRead.HasFlag(Xamarin.Essentials.PermissionStatus.Granted))
            {
                ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), DateTime.Now.AddDays(90), DateTime.Now);
                ParameterManager parameterManager = new ParameterManager();
                _initDCIMDirectory = string.Empty;
                if (!parameterManager.Get("CameraDirectoryFullPath", out _initDCIMDirectory))
                {
                    //параметр заполняется при старте индексации, уже должен быть заполнен
                    _initDCIMDirectory = imagesCache.GetPublicDirectoryDcim();
                }

                if (_initDCIMDirectory.Equals(imagesCache.GetPublicDirectoryDcim()))
                {
                    IsUsageMainMemory = true;
                    PathToDefaultDCIM = _initDCIMDirectory;
                }
                else
                {
                    IsUsageMainMemory = false;
                    PathToCustomDCIM = _initDCIMDirectory;                    
                }


                
                _pathToImagesDir = _initDCIMDirectory;
                var files = imagesCache.GetListFiles(_initDCIMDirectory);
                CountOfPhotoDefaultDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoDefaultDCIM"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoCustomDCIM"));
                updateDirContent(new DirectoryInfo(_pathToImagesDir));
            }
        }

        public void CloseDialog()
        {

        }
        
        public string PathToDefaultDCIM
        {
            get => _pathToDefaultDCIM;
            set
            {
                if(!value.Equals(_pathToDefaultDCIM))
                {
                    _pathToDefaultDCIM = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToDefaultDCIM"));
                }
            }
        }
        public string PathToCustomDCIM
        {
            get => _pathToCustomDCIM;
            set
            {
                if(!value.Equals(_pathToCustomDCIM))
                {
                    _pathToCustomDCIM = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToCustomDCIM"));
                }
            }
        }

        public int CountOfPhotoDefaultDCIM { get; set; } = 0;
        public int CountOfPhotoCustomDCIM { get; set; } = 0;

        public bool NavigationToRootIsVisible
        {
            get
            {
                return _navigationToRootIsVisible;
            }
            set
            {
                if(value != _navigationToRootIsVisible)
                {
                    _navigationToRootIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NavigationToRootIsVisible"));
                }
            }
        }

        public bool IsUsageMainMemory
        {
            get
            {
                return _isUsageMainMemory;
            }
            set
            {
                if(value != _isUsageMainMemory)
                {
                    _isUsageMainMemory = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsUsageMainMemory"));
                }
            }
        }

        public ObservableCollection<FileSystemElement> SourcePaths
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                if (value != _sourcePath)
                {
                    _sourcePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourcePaths"));
                }
            }
        }
    }

    public class FileSystemElement
    {
        public string Path { get; set; }
        public bool IsFile { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using QuestHelper.Managers;
using Syncfusion.DataSource.Extensions;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private bool _isUsageMainMemory;
        private bool _isUsageExtMemory;
        private ObservableCollection<string> _sourcePath = new ObservableCollection<string>();
        private string _pathToImagesDir = string.Empty;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand MainMemoryCheckBoxCommand { get; private set; }
        public ICommand ExtMemoryCheckBoxCommand { get; private set; }
        public ICommand NavigateDirUpCommand { get; private set; }
        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            MainMemoryCheckBoxCommand = new Command(mainMemoryCheckBoxCommand);
            ExtMemoryCheckBoxCommand = new Command(extMemoryCheckBoxCommand);
            NavigateDirUpCommand = new Command(navigateDirUpCommand);
        }

        private void navigateDirUpCommand(object obj)
        {
            var currentDirectory = Directory.GetParent(_pathToImagesDir);
            if (currentDirectory.Exists)
            {
                ObservableCollection<string> files = new ObservableCollection<string>();
                ObservableCollection<string> dirs = new ObservableCollection<string>();
                try
                {
                    files = currentDirectory.GetFiles("*.jpg;*.jpeg;*.png").OrderBy(f => f.CreationTime)
                        .Select(f => f.Name).ToArray().Take(10).ToObservableCollection();
                    dirs = currentDirectory.GetDirectories().OrderBy(d => d.Name).Select(d => d.Name)
                        .ToObservableCollection();
                    _pathToImagesDir = currentDirectory.FullName;
                    SourcePaths = new ObservableCollection<string>(dirs);
                    CountOfPhotoInternalDCIM = files.Count();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoInternalDCIM"));
                    int index = 0;
                    while (index < 10)
                    {
                        if (files.ElementAtOrDefault(index) != null)
                        {
                            SourcePaths.Add(files.ElementAt(index));
                        }
                        else
                        {
                            break;
                        }

                        index++;
                    }
                }
                catch (IOException ioException)
                {
                    
                }
                catch (Exception e)
                {
                    
                }
                
            }
        }

        private void extMemoryCheckBoxCommand(object obj)
        {

        }

        private void mainMemoryCheckBoxCommand(object obj)
        {

        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        public async void StartDialog()
        {
            PermissionManager permissions = new PermissionManager();
            var taskPermissionRead = await permissions.CheckAndRequestStorageReadPermission();
            if (taskPermissionRead.HasFlag(Xamarin.Essentials.PermissionStatus.Granted))
            {
                ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), DateTime.Now.AddDays(90), DateTime.Now);
                ParameterManager parameterManager = new ParameterManager();
                string pathToDCIMDirectory = string.Empty;
                if (!parameterManager.Get("CameraDirectoryFullPath", out pathToDCIMDirectory))
                {
                    //параметр заполняется при старте индексации, уже должен быть заполнен
                    pathToDCIMDirectory = imagesCache.GetPublicDirectoryDcim();
                }

                if (pathToDCIMDirectory.Equals(imagesCache.GetPublicDirectoryDcim()))
                {
                    IsUsageMainMemory = true;
                    IsUsageExtMemory = false;
                }

                PathToInternalDCIM = pathToDCIMDirectory;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToInternalDCIM"));
                _pathToImagesDir = pathToDCIMDirectory;
                var files = imagesCache.GetListFiles(pathToDCIMDirectory);
                CountOfPhotoInternalDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoInternalDCIM"));
                SourcePaths = new ObservableCollection<string>(files.ToList());
            }
        }

        public void CloseDialog()
        {

        }

        public string PathToInternalDCIM { get; set;} = string.Empty;
        public string PathToExternalDCIM { get; set; } = string.Empty;

        public int CountOfPhotoInternalDCIM { get; set; } = 0;
        public int CountOfPhotoExternalDCIM { get; set; } = 0;

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

        public bool IsUsageExtMemory
        {
            get
            {
                return _isUsageExtMemory;
            }
            set
            {
                if (value != _isUsageExtMemory)
                {
                    _isUsageExtMemory = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsUsageExtMemory"));
                }
            }
        }

        public ObservableCollection<string> SourcePaths
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
}

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
        private ObservableCollection<FileSystemElement> _sourcePath = new ObservableCollection<FileSystemElement>();
        private string _pathToImagesDir = string.Empty;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand MainMemoryCheckBoxCommand { get; private set; }
        public ICommand ExtMemoryCheckBoxCommand { get; private set; }
        public ICommand NavigateDirUpCommand { get; private set; }
        public ICommand ChooseDirCommand { get; private set; }
        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            MainMemoryCheckBoxCommand = new Command(mainMemoryCheckBoxCommand);
            ExtMemoryCheckBoxCommand = new Command(extMemoryCheckBoxCommand);
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
            if (currentDirectory.Exists)
            {
                updateDirContent(currentDirectory);
            }
        }

        private void updateDirContent(DirectoryInfo currentDirectory)
        {
            (var files, var dirs) = getDirContent(currentDirectory);
            if (files.Any() || dirs.Any())
            {
                SourcePaths =
                    new ObservableCollection<FileSystemElement>(dirs
                        .Select(i => new FileSystemElement() {Path = i, IsFile = false}).ToList());
                addFilesToSourcePaths(files, 30);

                _pathToImagesDir = currentDirectory.FullName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourcePaths"));
                CountOfPhotoInternalDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoInternalDCIM"));
            }
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

        private (ObservableCollection<string>, ObservableCollection<string>) getDirContent(DirectoryInfo currentDirectory)
        {
            ObservableCollection<string> files = new ObservableCollection<string>();
            ObservableCollection<string> dirs = new ObservableCollection<string>();
            try
            {
                files = currentDirectory.GetFiles("*.jpg").OrderBy(f => f.CreationTime)
                    .Select(f => f.FullName).ToArray().ToObservableCollection();
                dirs = currentDirectory.GetDirectories().OrderBy(d => d.Name).Select(d => d.Name)
                    .ToObservableCollection();
            }
            catch (IOException ioException)
            {
                    
            }
            catch (Exception e)
            {
                    
            }
            return (files, dirs);
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
                //SourcePaths = new ObservableCollection<string>(files.ToList());
                //navigateDirUpCommand(new object());
                updateDirContent(new DirectoryInfo(_pathToImagesDir));
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

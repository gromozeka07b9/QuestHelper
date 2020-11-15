using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using QuestHelper.Managers;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private bool _isUsageMainMemory;
        private bool _isUsageExtMemory;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand MainMemoryCheckBoxCommand { get; private set; }
        public ICommand ExtMemoryCheckBoxCommand { get; private set; }

        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            MainMemoryCheckBoxCommand = new Command(mainMemoryCheckBoxCommand);
            ExtMemoryCheckBoxCommand = new Command(extMemoryCheckBoxCommand);
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
                var files = imagesCache.GetListFiles(pathToDCIMDirectory);
                CountOfPhotoInternalDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoInternalDCIM"));
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
    }
}

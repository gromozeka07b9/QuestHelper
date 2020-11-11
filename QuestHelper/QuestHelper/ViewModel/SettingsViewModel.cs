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
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }

        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
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
                ParameterManager parameterManager = new ParameterManager();
                string pathToDCIMDirectory = string.Empty;
                if (!parameterManager.Get("CameraDirectoryFullPath", out pathToDCIMDirectory))
                {
                    //как так?
                }
                PathToDCIM = pathToDCIMDirectory;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PathToDCIM"));
                ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(new ImageManager(), DateTime.Now.AddDays(90), DateTime.Now);
                var files = imagesCache.GetListFiles(pathToDCIMDirectory);
                CountOfPhotoInDCIM = files.Count();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotoInDCIM"));
            }
        }

        public void CloseDialog()
        {

        }

        public string PathToDCIM {get; set;} = string.Empty;

        public int CountOfPhotoInDCIM { get; set; } = 0;
    }
}

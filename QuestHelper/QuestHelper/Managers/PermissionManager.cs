using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers
{
    public class PermissionManager
    {
        public async System.Threading.Tasks.Task<bool> PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission permission, string warningText)
        {
            PermissionStatus status = PermissionStatus.Unknown;
            try
            {
                status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                    {
                        UserDialogs.Instance.Alert(message: warningText, okText: "Ок");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                    if (results.ContainsKey(permission))
                    {
                        status = results[permission];
                    }
                }
            }
            catch (Exception)
            {
            }

            return status == PermissionStatus.Granted;
        }

        public async System.Threading.Tasks.Task<bool> PermissionGetCoordsGrantedAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
            return (status != PermissionStatus.Granted);
        }
    }
}

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
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        UserDialogs.Instance.Alert(message: warningText, okText: "Ок");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    if (results.ContainsKey(Permission.Location))
                    {
                        status = results[Permission.Location];
                    }
                }
            }
            catch (Exception)
            {
            }

            return status == PermissionStatus.Granted;
        }
    }
}

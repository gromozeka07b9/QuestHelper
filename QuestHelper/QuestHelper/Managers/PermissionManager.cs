using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers
{
    public class PermissionManager
    {
        public async System.Threading.Tasks.Task<bool> PermissionLocationGrantedAsync(string warningText)
        {
            PermissionStatus status = PermissionStatus.Unknown;
            try
            {
                status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
                if (status != PermissionStatus.Granted)
                {
                    var statusRequest = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                    if(statusRequest != PermissionStatus.Granted)
                    {
                        UserDialogs.Instance.Alert("Включить разрешение определения позиции можно через меню настроек системы", CommonResource.CommonMsg_Warning, CommonResource.CommonMsg_Ok);
                    }
                }
            }
            catch (Exception)
            {
            }

            return status == PermissionStatus.Granted;
        }

        /// <summary>
        /// Устаревший метод, надо уходить от него
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="warningText"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission permission, string warningText)
        {
            PermissionStatus status = PermissionStatus.Unknown;
            try
            {
                status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    var resultCheckRequest = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                    foreach (var permissionItem in resultCheckRequest)
                    {
                        if(permissionItem.Value != PermissionStatus.Granted)
                        {
                            UserDialogs.Instance.Alert("Включить разрешение определения позиции можно через меню настроек системы", CommonResource.CommonMsg_Warning, CommonResource.CommonMsg_Ok);
                            /*MainThread.BeginInvokeOnMainThread(() =>
                            {
                            });*/
                            return false;
                        }
                    }
                    bool resultRequest = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission);
                    if (!resultRequest)
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                        if (results.ContainsKey(permission))
                        {
                            status = results[permission];
                        }
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
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
            return (status == PermissionStatus.Granted);
        }
    }
}

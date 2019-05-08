using Acr.UserDialogs;
using QuestHelper.Model.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.Managers.Sync
{
    public class SyncPossibility
    {
        public async Task<bool> CheckAsync(bool showErrorMessageIfExist)
        {
            bool possibility = false;
            bool workInRoaming = false;
            bool isRoaming = DependencyService.Get<INetworkConnectionsService>().IsRoaming();
            if (isRoaming)
            {
                object storedWorkInRoaming;
                if (Application.Current.Properties.TryGetValue("WorkInRoaming", out storedWorkInRoaming))
                {
                    workInRoaming = (bool)storedWorkInRoaming;
                    possibility = workInRoaming;
                }
                else
                {
                    //ToDo:в фоне не работает!
                    workInRoaming = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = "Использовать синхронизацию в роуминге?", Title = "Используется роуминг", OkText = "Да", CancelText = "Нет" });
                    Application.Current.Properties.Add("WorkInRoaming", workInRoaming);
                }
            }
            if (!isRoaming || ((isRoaming) && (workInRoaming)))
            {
                var networkState = Connectivity.NetworkAccess;
                possibility = networkState == NetworkAccess.Internet;
                if ((!possibility) && (showErrorMessageIfExist))
                {
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = "Ошибка синхронизации", Message = "Проверьте ваше подключение к сети" }, string.Empty);
                }
            }

            return possibility;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers;
using Xamarin.Essentials;

namespace QuestHelper
{
    public class TokenStoreService : IAuthService
    {
        private string _tokenNameKey = "AuthToken";
        private string _userIdKey = "UserId";
        private string _tokenUsernameKey = "Username";
        private string _tokenEmailKey = "UserEmail";

        public async Task<string> GetAuthTokenAsync()
        {
            return await getDataByKey(_tokenNameKey);
        }

        public async Task<string> GetUserIdAsync()
        {
            return await getDataByKey(_userIdKey);
        }

        public async Task<string> GetUsernameAsync()
        {
            return await getDataByKey(_tokenUsernameKey);
        }
        public async Task<string> GetEmailAsync()
        {
            return await getDataByKey(_tokenEmailKey);
        }

        private async Task<string> getDataByKey(string keyName)
        {
            string datavalue = string.Empty;

            try
            {
                datavalue = await SecureStorage.GetAsync(keyName);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "GetAuthTokenAsync", e, false);
            }

            //если значение не было сохранено в безопасное хранилище, ищем его в небезопасном - Android 4.4.2
            if (string.IsNullOrEmpty(datavalue))
            {
                ParameterManager par = new ParameterManager();
                par.Get(keyName, out datavalue);
            }

            return datavalue;
        }

        public async Task<bool> SetAuthDataAsync(string authToken, string userId, string username, string email)
        {
            try
            {
                SecureStorage.Remove(_tokenNameKey);
                await SecureStorage.SetAsync(_tokenNameKey, authToken);
                SecureStorage.Remove(_userIdKey);
                await SecureStorage.SetAsync(_userIdKey, userId);
                SecureStorage.Remove(_tokenUsernameKey);
                await SecureStorage.SetAsync(_tokenUsernameKey, username);
                SecureStorage.Remove(_tokenEmailKey);
                await SecureStorage.SetAsync(_tokenEmailKey, email);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "SetAuthDataAsync", e, false);
                ParameterManager par = new ParameterManager();
                par.Set(_tokenNameKey, authToken);
                par.Set(_userIdKey, userId);
                par.Set(_tokenUsernameKey, username);
                par.Set(_tokenEmailKey, email);
            }

            return true;
        }

        public bool ResetAuthToken()
        {
            try
            {
                SecureStorage.Remove(_tokenNameKey);
                SecureStorage.Remove(_userIdKey);
                SecureStorage.Remove(_tokenUsernameKey);
                SecureStorage.Remove(_tokenEmailKey);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "ResetAuthTokenAsync", e, false);
                ParameterManager par = new ParameterManager();
                par.Delete(_tokenNameKey);
                par.Delete(_userIdKey);
                par.Delete(_tokenUsernameKey);
                par.Delete(_tokenEmailKey);
            }

            return true;
        }
    }
}

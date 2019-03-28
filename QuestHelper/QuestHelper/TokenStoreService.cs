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

        public async Task<string> GetAuthTokenAsync()
        {
            string token = string.Empty;

            try
            {
                token = await SecureStorage.GetAsync(_tokenNameKey);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "GetAuthTokenAsync", e, false);
            }

            //если токен не был сохранен в безопасное хранилище, ищем его в небезопасном - Android 4.4.2
            if (string.IsNullOrEmpty(token))
            {
                ParameterManager par = new ParameterManager();
                par.Get(_tokenNameKey, out token);
            }
            return token;
        }
        public async Task<string> GetUserIdAsync()
        {
            string userId = string.Empty;

            try
            {
                userId = await SecureStorage.GetAsync(_userIdKey);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "GetUserIdAsync", e, false);
            }

            //если токен не был сохранен в безопасное хранилище, ищем его в небезопасном - Android 4.4.2
            if (string.IsNullOrEmpty(userId))
            {
                ParameterManager par = new ParameterManager();
                par.Get(_userIdKey, out userId);
            }
            return userId;
        }

        public async Task SetAuthDataAsync(string authToken, string userId)
        {
            try
            {
                SecureStorage.Remove(_tokenNameKey);
                await SecureStorage.SetAsync(_tokenNameKey, authToken);
                SecureStorage.Remove(_userIdKey);
                await SecureStorage.SetAsync(_userIdKey, userId);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "SetAuthDataAsync", e, false);
                ParameterManager par = new ParameterManager();
                par.Set(_tokenNameKey, authToken);
                par.Set(_userIdKey, userId);
            }
        }
    }
}

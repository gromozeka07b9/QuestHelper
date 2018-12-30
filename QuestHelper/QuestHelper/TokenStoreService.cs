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
        private string _tokenName = "AuthToken";

        public async Task<string> GetAuthTokenAsync()
        {
            string token = string.Empty;

            try
            {
                token = await SecureStorage.GetAsync(_tokenName);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "SetAuthToken", e, false);
            }

            //если токен не был сохранен в безопасное хранилище, ищем его в небезопасном - Android 4.4.2
            if (string.IsNullOrEmpty(token))
            {
                ParameterManager par = new ParameterManager();
                par.Get(_tokenName, out token);
            }
            return token;
        }

        public async Task SetAuthTokenAsync(string authToken)
        {
            try
            {
                SecureStorage.Remove(_tokenName);
                await SecureStorage.SetAsync(_tokenName, authToken);
            }
            catch (Exception e)
            {
                HandleError.Process("TokenStoreService", "SetAuthToken", e, false);
                ParameterManager par = new ParameterManager();
                par.Set(_tokenName, authToken);
            }
        }
    }
}

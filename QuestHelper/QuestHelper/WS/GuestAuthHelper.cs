using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static QuestHelper.WS.AccountApiRequest;

namespace QuestHelper.WS
{
    public class GuestAuthHelper
    {
        private string _apiUrl = "http://igosh.pro/api";
        private string _username = "demo";
        private string _password = "demo";

        public GuestAuthHelper()
        {

        }

        public async Task<string> TryGetGuestTokenAsync()
        {
            string token = string.Empty;

            AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
            TokenResponse authData = await apiRequest.GetTokenAsync(_username, _password, true);
            if (!string.IsNullOrEmpty(authData?.Access_Token))
            {
                Analytics.TrackEvent("GetToken Demo", new Dictionary<string, string> { { "Username", _username } });
                TokenStoreService tokenService = new TokenStoreService();
                bool setResultIsOk = await tokenService.SetAuthDataAsync(authData.Access_Token, authData.UserId);
                if (setResultIsOk)
                {
                    token = authData.Access_Token;
                }
            }

            return token;
        }
    }
}

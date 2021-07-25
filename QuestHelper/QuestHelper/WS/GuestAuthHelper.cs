using Microsoft.AppCenter.Analytics;
using QuestHelper.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static QuestHelper.WS.AccountApiRequest;

namespace QuestHelper.WS
{
    public class GuestAuthHelper
    {
        private string _apiUrl = "https://igosh.pro/api";
        private string _username = "demo";
        private string _password = "demo";

        public GuestAuthHelper()
        {

        }

        public async Task<string> TryGetGuestTokenAsync()
        {
            string token = string.Empty;

            ParameterManager par = new ParameterManager();
            string demoUsername = generateDemoUsername();
            par.Set("DemoUsername", demoUsername);

            AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
            _username = demoUsername;
            _password = demoUsername;
            TokenResponse authData = await apiRequest.GetTokenAsync(_username, _password, true);
            if (!string.IsNullOrEmpty(authData?.Access_Token))
            {
                Analytics.TrackEvent("GetToken Demo", new Dictionary<string, string> { { "Username", _username } });
                TokenStoreService tokenService = new TokenStoreService();
                bool setResultIsOk = await tokenService.SetAuthDataAsync(authData.Access_Token, authData.UserId, string.Empty, string.Empty);
                if (setResultIsOk)
                {
                    token = authData.Access_Token;
                }
            }

            return token;
        }
        public bool ResetCurrentGuestUsername()
        {

            ParameterManager par = new ParameterManager();
            par.Delete("DemoUsername");
            /*string demoUsername = string.Empty;
            if (!par.Get("DemoUsername", out demoUsername))
            {
                par.Set("DemoUsername", string.Empty);
            }*/

            return true;
        }

        private string generateDemoUsername()
        {
            var dt = DateTime.Now;
            return $"demo-{dt.Year}-{dt.Month}-{dt.Day}-{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond}";
        }
    }
}

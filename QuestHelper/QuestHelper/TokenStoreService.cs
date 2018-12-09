using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestHelper
{
    public class TokenStoreService : IAuthService
    {
        private string _tokenName = "AuthToken";

        public string GetAuthToken()
        {
            string token = string.Empty;
            var key = App.Current.Properties.FirstOrDefault(x => x.Key == _tokenName);
            if (key.Value != null)
            {
                token = key.Value.ToString();
            }
            return token;
        }

        public void SetAuthToken(string authToken)
        {
            if (App.Current.Properties.Where(x => x.Key == _tokenName).Any())
                App.Current.Properties.Remove(_tokenName);
            App.Current.Properties.Add(_tokenName, authToken);
        }
    }
}

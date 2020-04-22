using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Consts
{
    public class Auth0Settings
    {
        //Константы для того, чтобы можно было использовать в настройках Intent
        public const string Domain = "gosh.eu.auth0.com";
        public const string DataSchemeDebug = "com.sd.goshdebug";
        public const string DataSchemeRelease = "com.sd.gosh";
        public const string DataPathPrefixDebug = "/android/com.sd.goshdebug/callback";
        public const string DataPathPrefixRelease = "/android/com.sd.gosh/callback";

        public static string ClientId
        {
            get
            {
#if DEBUG
                return "QUseZnFk1LQu8F2XI9bmwEBBVZ90duuZ";
#else
                return "tmSSaMSEpL2TjCv63UI3s54DmcP7qpd0";
#endif
            }
        }
    }
}

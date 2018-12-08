using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QuestHelper.Server.Managers
{
    public class BearerParser
    {
        public static string GetTokenFromHeader(IHeaderDictionary dict)
        {
            var bearerToken = dict.FirstOrDefault(x => x.Key == "Authorization").Value.ToString();
            return bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
        }
    }
}

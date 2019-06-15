using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuestHelper.Server.Auth;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Account
{
    public class JwtResponseMaker
    {
        public JwtResponseMaker()
        {
        }

        public string GetJwtResponse(User user)
        {
            string serializedResponse = string.Empty;
            IdentityManager identityManager = new IdentityManager();
            var identity = identityManager.GetIdentity(user);
            if (identity != null)
            {
                JwtManager jwt = new JwtManager();
                var encodedJwt = jwt.GetEncodedJwt(identity, user.TokenKey);

                var response = new
                {
                    access_token = encodedJwt,
                    username = user.Name,
                    email = user.Email,
                    userid = user.UserId
                };

                // сериализация ответа
                serializedResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
                //Response.ContentType = "application/json";
                //await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            }
            /*else
            {
                Console.WriteLine($"Generate JWT token:status 500, {user.Name}");
                Response.StatusCode = 500;
                await Response.WriteAsync("Error while generate token.");
            }*/
            return serializedResponse;
        }
    }
}

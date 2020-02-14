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
                    userid = user.UserId,
                    role = user.Role
                };

                // сериализация ответа
                serializedResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            return serializedResponse;
        }
    }
}

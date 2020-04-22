using Auth0.OidcClient;
using QuestHelper.Droid;
using QuestHelper.Model;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(OAuthService))]
namespace QuestHelper.Droid
{
    public class OAuthService : IOAuthService
    {
        public async System.Threading.Tasks.Task<OAuthUser> LoginAsync()
        {
            OAuthUser user = new OAuthUser();

            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = Consts.Auth0Settings.Domain,
                ClientId = Consts.Auth0Settings.ClientId
            });

            var loginResult = await client.LoginAsync();

            if(!loginResult.IsError)
            {
                string name = loginResult.User.FindFirst(n => n.Type == "name")?.Value;
                if(string.IsNullOrEmpty(name))
                    name = loginResult.User.FindFirst(n => n.Type == "nickname")?.Value;

                string email = loginResult.User.FindFirst(n => n.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    email = name;

                string imgurl = loginResult.User.FindFirst(n => n.Type == "picture")?.Value;
                user = new OAuthUser() { Id = Guid.NewGuid().ToString(), Name = name, Email = email, ImgUrl = new Uri(imgurl) };
            }

            return user;
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

    }
}
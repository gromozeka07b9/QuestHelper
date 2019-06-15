using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Auth
{
    public class OAuthUserCreator
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        public OAuthUserCreator()
        {
        }

        internal OauthUser CreateGoogleUser(OAuthTokenRequest request, User user)
        {
            OauthUser oAuthUser = new OauthUser();
            using (var _db = new ServerDbContext(_dbOptions))
            {
                oAuthUser = _db.OauthUser.FirstOrDefault(x => x.Email == request.Email);
                if(oAuthUser == null)
                {
                    oAuthUser = new OauthUser();
                    oAuthUser.OauthUserId = Guid.NewGuid().ToString();
                    oAuthUser.AuthenticatorId = request.AuthenticatorUserId;
                    oAuthUser.Name = request.Username;
                    oAuthUser.Email = request.Email;
                    oAuthUser.Locale = request.Locale;
                    oAuthUser.ImgUrl = request.ImgUrl;
                    oAuthUser.Version = 1;
                    oAuthUser.CreateDate = DateTime.Now;
                    oAuthUser.UserId = user.UserId;
                    oAuthUser.User = _db.User.Find(user.UserId);
                    _db.OauthUser.Add(oAuthUser);
                    _db.SaveChanges();
                }
                Console.WriteLine($"Created OauthUser: {oAuthUser.Name}");
            }
            return oAuthUser;
        }
    }
}

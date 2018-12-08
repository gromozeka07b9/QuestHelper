using QuestHelper.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuestHelper.Server.Managers;
using Xunit;
using Moq;
using QuestHelper.Server;
using QuestHelper.Server.Models;
using Xamarin.Forms.Xaml;

namespace QuestHelperServer.Tests
{
    /*public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<User> Users { get; set; }
    }*/
    public class TestAuth
    {
        [Fact]
        public void TestMust_GenerateJwt()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "name"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "role")
            };
            ClaimsIdentity testIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

            var options = ServerDbContext.GetOptionsContextDbServer(true);
            JwtManager jwt = new JwtManager(options);
            string encodedStringJwt = jwt.GetEncodedJwt(testIdentity);

            Assert.False(string.IsNullOrEmpty(encodedStringJwt));
        }

        [Fact]
        public void TestMust_GetIdentityOk()
        {
            var options = ServerDbContext.GetOptionsContextDbServer(true);
            var users = prepareUsers(options);

            using (var context = new ServerDbContext(options))
            {
                IdentityManager identity = new IdentityManager();
                var user = context.User.Find(users[0].UserId);
                var result = identity.GetIdentity(user);
                Assert.True(result.IsAuthenticated);
            }
        }

        [Fact]
        public void TestMust_TokenHashWrited()
        {
            var options = ServerDbContext.GetOptionsContextDbServer(true);
            var users = prepareUsers(options);

            using (var context = new ServerDbContext(options))
            {
                IdentityManager identityManager = new IdentityManager();
                var user = context.User.Find(users[0].UserId);
                var identity = identityManager.GetIdentity(user);

                JwtManager jwt = new JwtManager(options);
                var encodedJwt = jwt.GetEncodedJwt(identity);
                jwt.WriteJwtHashToDb(user, encodedJwt);
                user = context.User.Find(users[0].UserId);


                Assert.False(string.IsNullOrEmpty(user.TokenHash));
            }
        }

        private User[] prepareUsers(DbContextOptions<ServerDbContext> options)
        {
            var users = GetUsersFixture();

            using (var context = new ServerDbContext(options))
            {
                try
                {
                    foreach (var user in context.User)
                    {
                        context.User.Remove(user);
                    }
                    context.SaveChanges();
                }
                catch
                {
                }
                context.User.AddRange(users);
                context.SaveChanges();
            }

            return users;
        }

        private User[] GetUsersFixture()
        {
            return new[]
            {
                new User() {UserId = "1", Name = "user1", Password = "password1", Role = "Admin"},
                new User() {UserId = "2", Name = "user2", Password = "password2", Role = "Guest"},
                new User() {UserId = "3", Name = "user3", Password = "password3", Role = "Guest"}
            };
        }
    }
}

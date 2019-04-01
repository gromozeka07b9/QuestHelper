using QuestHelper.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuestHelper.Server.Managers;
using Xunit;
using Moq;
using QuestHelper.Server;
using QuestHelper.Server.Models;
//using Xamarin.Forms.Xaml;

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
            string userKey = "test";
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "name"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "role")
            };
            ClaimsIdentity testIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

            //var options = ServerDbContext.GetOptionsContextDbServer(true);
            JwtManager jwt = new JwtManager();
            string encodedStringJwt = jwt.GetEncodedJwt(testIdentity, userKey);

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
        public void TestMust_ValidateUserKey()
        {
            string userName = "user1";
            var options = ServerDbContext.GetOptionsContextDbServer(true);
            var users = prepareUsers(options);

            string userKey = users.Find(x=>x.Name == userName).TokenKey;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "name"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "role")
            };
            ClaimsIdentity testIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

            JwtManager jwt = new JwtManager();
            string encodedStringJwt = jwt.GetEncodedJwt(testIdentity, userKey);
            jwt = null;

            //проверим, что ключ тот же
            JwtManager jwtTest = new JwtManager();
            string userKeyTest = jwtTest.GetUserKeyFromToken(encodedStringJwt);

            Assert.Equal(userKey, userKeyTest);

            //и проведем валидацию
            ValidateUser validate = new ValidateUser(options, encodedStringJwt);
            Assert.True(validate.UserIsValid(userName));

            //и сбросим userkey и снова проведем валидацию
            var testUser = users.Find(x => x.Name == userName);
            testUser.TokenKey = Guid.NewGuid().ToString();
            UpdateUser(options, testUser);
            ValidateUser validateFault = new ValidateUser(options, encodedStringJwt);
            Assert.False(validateFault.UserIsValid(userName));
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
        private void UpdateUser(DbContextOptions<ServerDbContext> options, User user)
        {
            var users = GetUsersFixture();

            using (var context = new ServerDbContext(options))
            {
                try
                {
                    var entity = context.User.Find(user.UserId);
                    context.Entry(entity).CurrentValues.SetValues(user);
                    context.SaveChanges();
                }
                catch
                {
                }
            }
        }

        private User[] GetUsersFixture()
        {
            return new[]
            {
                new User() {UserId = "1", Name = "user1", Password = "password1", Role = "Admin", TokenKey = Guid.NewGuid().ToString()},
                new User() {UserId = "2", Name = "user2", Password = "password2", Role = "Guest", TokenKey = Guid.NewGuid().ToString()},
                new User() {UserId = "3", Name = "user3", Password = "password3", Role = "Guest", TokenKey = Guid.NewGuid().ToString()}
            };
        }
    }
}

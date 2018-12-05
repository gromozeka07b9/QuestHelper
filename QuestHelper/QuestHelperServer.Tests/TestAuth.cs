using QuestHelper.Server.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using Xunit;
using Moq;
using QuestHelper.Server;
using QuestHelper.Server.Models;

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

            string encodedStringJwt = JwtManager.GetEncodedJwt(testIdentity);

            Assert.False(string.IsNullOrEmpty(encodedStringJwt));
        }

        /*[Fact]
        public void TestMust_GetIdentityOk()
        {
            var users = new[]
            {
                new User() {Name = "user1", Password = "password1"},
                new User() {Name = "user2", Password = "password2"},
                new User() {Name = "user3", Password = "password3"}
            };

            //DbContextMockFactory.
            //var dbContext = new Mock<ServerDbContext>();
            //dbContext.Setup(x => x.User).Returns(new User() {Name = "name1"});
            //var dbContextMock = new TestDbContext(new DbContextOptions<DbContext>());
            //dbContextMock.Users.Add(new User(){Name = "name1", Password = "password1"});
            //var test = dbContextMock.Users.FirstOrDefault(x => x.Name == "name1");
            //IdentityManager identityManager = new IdentityManager();
            //Assert.False(test==null);
        }*/
    }
}

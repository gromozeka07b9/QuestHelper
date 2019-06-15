using System;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Auth
{
    public class UserCreator
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        public UserCreator()
        {
        }

        internal User Create(TokenRequest request, bool isDemoUser)
        {
            User user = new User();
            using (var _db = new ServerDbContext(_dbOptions))
            {
                user.UserId = Guid.NewGuid().ToString();
                user.Role = isDemoUser ? "demo" : "user";
                user.Name = request.Username;
                user.Email = request.Email;
                user.Password = isDemoUser ? user.Name : request.Password;
                user.Version = 1;
                user.CreateDate = DateTime.Now;
                user.TokenKey = user.Name;
                _db.User.Add(user);
                _db.SaveChanges();
                Console.WriteLine($"Created DB user: {user.Name}");
            }
            return user;
        }

        internal void AddAccessToDemoRoute(User user)
        {
            using (var _db = new ServerDbContext(_dbOptions))
            {
                RouteAccess access = new RouteAccess();
                access.RouteAccessId = Guid.NewGuid().ToString();
                access.RouteId = "dfdd6823-a44c-4f1a-8df8-2996deb4185c";//Демо-маршрут. Да, знаю.
                access.CanChange = false;
                access.CreateDate = DateTime.Now;
                access.UserId = user.UserId;
                _db.RouteAccess.Add(access);
                _db.SaveChanges();
                Console.WriteLine($"Added DB access: {user.Name}");
            }
        }
    }
}

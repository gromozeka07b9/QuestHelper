using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Route> Route { get; set; }
        public DbSet<RoutePoint> RoutePoint { get; set; }
        public DbSet<RoutePointMediaObject> RoutePointMediaObject { get; set; }
        public DbSet<User> User { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"Data Source=dbquesthelper.mysql.database.azure.com; Database=QuestHelper; User Id=sa@dbquesthelper; Password=Klim2002;");
        }
    }
}
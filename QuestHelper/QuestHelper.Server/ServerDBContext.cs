using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server
{
    public class ServerDbContext : DbContext
    {
        public DbSet<Route> Route { get; set; }
        public DbSet<RoutePoint> RoutePoint { get; set; }
        public DbSet<User> User { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"Data Source=dbquesthelper.mysql.database.azure.com; Database=QuestHelper; User Id=sa@dbquesthelper; Password=Klim2002;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Route>()
                .HasOne(p => p.User)
                .WithMany(t => t.Routes)
                .HasForeignKey(p => p.UserId);                
                */
        }
    }
}
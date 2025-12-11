using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Entities;

namespace Net.MusicApp.Data
{
    public class MusicAppDBContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public MusicAppDBContext(DbContextOptions<MusicAppDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u=>u.id);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Entities;

namespace Net.MusicApp.Data
{
    public class MusicAppDBContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Singer> Singers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlaylistSongs> PlaylistSongs { get; set; }

        public MusicAppDBContext(DbContextOptions<MusicAppDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);
                e.Property(u => u.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
            });

            // Singer
            modelBuilder.Entity<Singer>(e =>
            {
                e.HasKey(s => s.SingerId);
                e.Property(s => s.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
            });

            // Genre
            modelBuilder.Entity<Genre>(e =>
            {
                e.HasKey(g => g.GenreId);
                e.Property(g => g.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
            });

            // Playlist
            modelBuilder.Entity<Playlist>(e =>
            {
                e.HasKey(p => p.PlaylistId);
                e.Property(p => p.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
                e.HasOne(s => s.User)
     .WithMany(si => si.Playlists)
     .HasForeignKey(s => s.UserId)
     .OnDelete(DeleteBehavior.Cascade);

            });

            // Song
            modelBuilder.Entity<Song>(e =>
            {
                e.HasKey(s => s.SongId);
                e.Property(s => s.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
                e.HasOne(s => s.Singer)
     .WithMany(si => si.Songs)
     .HasForeignKey(s => s.SingerId)
     .OnDelete(DeleteBehavior.Restrict); // <-- Quan trọng: Đổi thành Restrict

                // Cấu hình GENRE: Restrict (Không cho xóa Genre nếu còn Song)
                e.HasOne(s => s.Genre)
                 .WithMany(g => g.Songs)
                 .HasForeignKey(s => s.GenreId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // PlaylistSongs
            modelBuilder.Entity<PlaylistSongs>(e =>
            {
                e.HasKey(ps => new {ps.PlaylistId,ps.SongId});
                e.Property(ps => ps.CreatedAt)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                 .ValueGeneratedOnAdd();
                e.HasOne(s => s.Song)
                .WithMany(si => si.PlaylistSongs)
                .HasForeignKey(s => s.SongId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(s => s.Playlist)
                .WithMany(si => si.PlaylistSongs)
                .HasForeignKey(s => s.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
                ;

            });

        }
    }
}

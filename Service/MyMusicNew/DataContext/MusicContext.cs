using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System.Threading.Tasks;
namespace DataContext
{
    public class MusicContext : DbContext, IContext
    {
        private readonly string _connection;
        public MusicContext(string connectionString)
        {
            _connection = connectionString;
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<AudioFeatures> AudioFeatures { get; set; }

        public async Task Save()
        {
            _ = await SaveChangesAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlServer(_connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----------------------
            // USER
            // ----------------------
            _ = modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            _ = modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            _ = modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            _ = modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            _ = modelBuilder.Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            _ = modelBuilder.Entity<User>()
                .HasMany(u => u.PlayHistories)
                .WithOne(ph => ph.User)
                .HasForeignKey(ph => ph.UserId);
            _ = modelBuilder.Entity<User>()
                .HasMany(u => u.Songs)
                .WithOne()
                .HasForeignKey("UserId")
                .IsRequired(false);
            // ----------------------
            // SONG
            // ----------------------
            _ = modelBuilder.Entity<Song>()
                .HasKey(s => s.SongId);

            _ = modelBuilder.Entity<Song>()
                .Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(255);

            _ = modelBuilder.Entity<Song>()
                .Property(s => s.Artist)
                .IsRequired()
                .HasMaxLength(255);

            _ = modelBuilder.Entity<Song>()
                .Property(s => s.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            _ = modelBuilder.Entity<Song>()
                .Property(s => s.Genre)
                .HasMaxLength(100);

            _ = modelBuilder.Entity<Song>()
                .Property(s => s.Mood)
                .HasMaxLength(100);

            _ = modelBuilder.Entity<Song>()
                .HasMany(s => s.PlayHistories)
                .WithOne(ph => ph.Song)
                .HasForeignKey(ph => ph.SongId);

            _ = modelBuilder.Entity<Song>()
                .HasOne(s => s.AudioFeatures)
                .WithOne(af => af.Song)
                .HasForeignKey<AudioFeatures>(af => af.SongId);

            _ = modelBuilder.Entity<Song>()
                .HasMany(s => s.PlaylistSongs)
                .WithOne(ps => ps.Song)
                .HasForeignKey(ps => ps.SongId);

            // ----------------------
            // PLAYLIST
            // ----------------------
            _ = modelBuilder.Entity<Playlist>()
                .HasKey(p => p.PlaylistId);

            _ = modelBuilder.Entity<Playlist>()
                .Property(p => p.PlaylistName)
                .IsRequired()
                .HasMaxLength(255);

            _ = modelBuilder.Entity<Playlist>()
                .HasMany(p => p.PlaylistSongs)
                .WithOne(ps => ps.Playlist)
                .HasForeignKey(ps => ps.PlaylistId);

            // ----------------------
            // PLAYLISTSONG (Many-to-Many)
            // ----------------------
            _ = modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            _ = modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId);

            _ = modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId);

            // ----------------------
            // PLAYHISTORY
            // ----------------------
            _ = modelBuilder.Entity<PlayHistory>()
                .HasKey(ph => ph.HistoryId);

            _ = modelBuilder.Entity<PlayHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PlayHistories)
                .HasForeignKey(ph => ph.UserId);

            _ = modelBuilder.Entity<PlayHistory>()
                .HasOne(ph => ph.Song)
                .WithMany(s => s.PlayHistories)
                .HasForeignKey(ph => ph.SongId);

            // ----------------------
            // AUDIOFEATURES (One-to-One with Song)
            // ----------------------
            _ = modelBuilder.Entity<AudioFeatures>()
                .HasKey(af => af.FeatureId);

            _ = modelBuilder.Entity<AudioFeatures>()
                .HasOne(af => af.Song)
                .WithOne(s => s.AudioFeatures)
                .HasForeignKey<AudioFeatures>(af => af.SongId);
            _ = modelBuilder.Entity<AudioFeatures>()
                .Property(af => af.Tempo)
                .HasColumnType("decimal(5,2)");

            _ = modelBuilder.Entity<AudioFeatures>()
                .Property(af => af.Energy)
                .HasColumnType("decimal(3,2)");
        }


    }
}

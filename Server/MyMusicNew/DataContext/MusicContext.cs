using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System.Threading.Tasks;
namespace DataContext
{
    public class MusicContext : DbContext, IContext
    {
        private readonly string _connection;
        //public MusicContext(string connectionString)
        //{
        //    _connection = connectionString;
        //}
        public MusicContext(DbContextOptions<MusicContext> options)
    : base(options)
        {
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
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    _ = optionsBuilder.UseSqlServer(_connection);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* ===================== User ===================== */
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique(); // הבטחה שכתובת המייל ייחודית
            });

            /* ===================== Song ===================== */
            modelBuilder.Entity<Song>(entity =>
            {
                // קשר בין שיר למשתמש (מי העלה את השיר)
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Songs)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // שינוי ל-Restrict כדי למנוע לופ של מחיקות
            });

            /* ===================== Playlist ===================== */
            modelBuilder.Entity<Playlist>(entity =>
            {
                // קשר בין פלייליסט למשתמש
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Playlists)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* ===================== PlaylistSong (טבלת קשר) ===================== */
            modelBuilder.Entity<PlaylistSong>(entity =>
            {
                entity.HasKey(ps => ps.PlaylistSongId); // הגדרת מפתח ראשי

                // קשר לפלייליסט
                entity.HasOne(ps => ps.Playlist)
                      .WithMany(p => p.PlaylistSongs)
                      .HasForeignKey(ps => ps.PlaylistId)
                      .OnDelete(DeleteBehavior.Cascade);

                // קשר לשיר
                entity.HasOne(ps => ps.Song)
                      .WithMany(s => s.PlaylistSongs)
                      .HasForeignKey(ps => ps.SongId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* ===================== AudioFeatures (1-to-1) ===================== */
            modelBuilder.Entity<AudioFeatures>(entity =>
            {
                entity.HasKey(af => af.FeatureId); // מפתח ראשי עצמאי כפי שהגדרת ב-Entity

                // הגדרת קשר אחד-לאחד מול שיר
                entity.HasOne(af => af.Song)
                      .WithOne(s => s.AudioFeatures)
                      .HasForeignKey<AudioFeatures>(af => af.SongId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* ===================== PlayHistory ===================== */
            modelBuilder.Entity<PlayHistory>(entity =>
            {
                entity.HasKey(ph => ph.HistoryId);

                // קשר לשיר שנוגן - חובה
                entity.HasOne(ph => ph.Song)
                      .WithMany(s => s.PlayHistories)
                      .HasForeignKey(ph => ph.SongId)
                      .OnDelete(DeleteBehavior.Cascade); // אם שיר נמחק, היסטוריית ההשמעות שלו נמחקת
            });
        }

    }
}

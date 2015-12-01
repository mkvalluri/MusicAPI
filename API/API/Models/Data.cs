namespace API.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Data : DbContext
    {
        public Data()
            : base("name=Data")
        {
        }

        public virtual DbSet<ActiveYear> ActiveYears { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<ArtistGenre> ArtistGenres { get; set; }
        public virtual DbSet<ArtistPopularity> ArtistPopularities { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .HasMany(e => e.ActiveYears)
                .WithRequired(e => e.Artist)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Artist>()
                .HasMany(e => e.Albums)
                .WithRequired(e => e.Artist)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Artist>()
                .HasMany(e => e.ArtistGenres)
                .WithRequired(e => e.Artist)
                .HasForeignKey(e => e.Artist_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Artist>()
                .HasMany(e => e.ArtistPopularities)
                .WithRequired(e => e.Artist)
                .HasForeignKey(e => e.Artist_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Genre>()
                .HasMany(e => e.ArtistGenres)
                .WithRequired(e => e.Genre)
                .HasForeignKey(e => e.Genre_Id)
                .WillCascadeOnDelete(false);
        }
    }
}

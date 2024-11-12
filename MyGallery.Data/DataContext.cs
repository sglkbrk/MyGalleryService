// MyGallery.Data/DataContext.cs
using Microsoft.EntityFrameworkCore;
using MyGallery.Domain;

namespace MyGallery.Data
{
    public class MyGalleryContext : DbContext
    {
        public MyGalleryContext(DbContextOptions<MyGalleryContext> options) : base(options) { }

        public DbSet<Projects> Projects { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoCaptions> PhotoCaptions { get; set; }
        public DbSet<ContantMe> ContantMe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Projects>()
                .Property(p => p.Slug)
                .HasMaxLength(40);
            // Projects tablosunda Slug ve CreatedAt alanlarına indeks ekleme
            modelBuilder.Entity<Projects>()
                .HasIndex(p => p.Slug)
                .HasDatabaseName("IX_Projects_Slug");

            modelBuilder.Entity<Projects>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Projects_CreatedAt");
            // Photo tablosunda ProjectsId alanına indeks ekleme
            modelBuilder.Entity<Photo>()
               .HasIndex(p => p.ProjectsId)
               .HasDatabaseName("IX_Photo_ProjectsId");
        }
    }
}

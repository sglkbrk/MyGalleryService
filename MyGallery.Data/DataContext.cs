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
    }
}

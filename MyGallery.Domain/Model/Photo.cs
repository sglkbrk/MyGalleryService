using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyGallery.Domain
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProjectsId { get; set; }
        public string? PhotoUrl { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Photographer { get; set; }

        public Category Category { get; set; }
        public int Size { get; set; }
        public Format Format { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Camera { get; set; }
        public string? Lens { get; set; }
        public string? FocalLength { get; set; }
        public string? Aperture { get; set; }
        public string? Iso { get; set; }
        public string? ShutterSpeed { get; set; }
        public string? Date { get; set; }
    }

    public enum Format
    {
        horizontal,
        vertical

    }
}
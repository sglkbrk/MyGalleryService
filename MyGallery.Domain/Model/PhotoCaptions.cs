using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyGallery.Domain
{
    public class PhotoCaptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PhotoId { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }

        public virtual Photo? Photo { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyGallery.Domain
{
    public class ContantMe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        public string subject { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
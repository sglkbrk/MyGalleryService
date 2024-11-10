using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGallery.Domain
{
    public class Projects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Client { get; set; }
        public string Photographer { get; set; }
        public string Camera { get; set; }
        public Category Category { get; set; }
        public string? MainImageUrl { get; set; }
        public string Slug { get; set; }
        public Status Status { get; set; }
        public bool HomePage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<Photo>? Photos { get; set; }
    }

    public enum Status
    {
        Active,
        Inactive
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyGallery.Domain
{
    public enum Category
    {
        portrait,
        landscape,
        food,
        nature,
        night,
        travel,
        street,
        CityandArchitecture,
        other
    }
}
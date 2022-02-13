using System.ComponentModel.DataAnnotations;
namespace Entities.Model
{
    public class GeoPoint
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }
}

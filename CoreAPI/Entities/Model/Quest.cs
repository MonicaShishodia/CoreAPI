using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Model
{
    public class Quest
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int GeoPointID { get; set; }
        [Required]
        public int LandUseID { get; set; }
        [Required]
        public int LandCoverDistanceID { get; set; }
        [Required]
        public int LandKindID { get; set; }
        [Required]
        [EmailAddress]
        public string UploadedBy { get; set; }
        [Required]
        public DateTime UploadedDateUTC { get; set; }
        public List<Image> Images { get; set; }
    }
}

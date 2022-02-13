using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModel
{
    public class QuestViewModel
    {
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longtitude { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public LandUseEnum LandUse { get; set; }
        [Required]
        public LandCoverDistanceEnum LandCoverDistance { get; set; }
        [Required]
        public LandKindEnum LandKind { get; set; }
        [Display(Name = "UploadedByUserEmail")]
        public string UploadedBy { get; set; }
        public DateTime UploadedDateUTC { get; set; }
        [Required]
        public IEnumerable<ImageViewModel> Images { get; set; }
    }
    public enum LandUseEnum
    {
        Water,
        Bare,
        Built,
        Vegetated
    }
    public enum LandKindEnum
    {
        Residential,
        Amenities,
        Recreation,
        Commerce,
        Construction,
        Transport,
        Industry,
        Agriculture,
        Forestry
    }
    public enum LandCoverDistanceEnum
    {
        //Swagger not displaying this enum's display name and shows name so request needs to be made by name such as _1Meter rather than '1.5 Meter'
        [Display(Name = "1.5 Meter")]
        _1Meter,
        [Display(Name = "1.5-10 Meter")]
        _10Meter,
        [Display(Name = "10-50 Meter")]
        _50Meter,
        [Display(Name = ">50 Meter")]
        _100Meter

    }
}

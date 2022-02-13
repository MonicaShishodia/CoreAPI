using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Entities.ViewModel
{
    public class ImageViewModel
    {
        public string Name { get; set; }

        public Int64 SizeInBytes { get; set; }

        public string Path { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public DirectionEnum Direction { get; set; }

        [Required]
        public IFormFile FileObject { get; set; }
        public int QuestID { get; set; }
        public byte[] ImageFile { get; set; }        
    }
    public enum DirectionEnum
    {
        North,
        South,
        East,
        West,
        Ground
    }
    public enum ImageSizeEnum
    {
        Thumbnail,
        Small,
        Large
    }
}

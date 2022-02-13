using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Model
{
    public class Image
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [Key]
        public string Name { get; set; }
        [Required]
        public Int64 SizeInBytes { get; set; }
        [Required]
        public string Path { get; set; }

        public int DirectionID { get; set; }
        public int QuestID { get; set; }
        public Quest Quest { get; set; }
    }
}

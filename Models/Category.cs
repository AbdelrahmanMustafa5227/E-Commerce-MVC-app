using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Category Name")]
        public string name { get; set; }

        [Range(1, 100, ErrorMessage = "Display Order must be between 1 & 100")]
        [DisplayName("Display Order")]
        public int displayOrder { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("ResourceUrls")]
    public class ResourceUrlModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CultureCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("Log")]
    public class LogModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(500)]
        public string Message { get; set; }
        [Column(TypeName = "ntext")]
        public string Details { get; set; }

    }
}

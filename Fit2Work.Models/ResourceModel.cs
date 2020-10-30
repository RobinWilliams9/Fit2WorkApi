using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("Resources")]
    public class ResourceModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CultureCode { get; set; }
        [Required]
        public string EmailHeader { get; set; }
        [Required]
        public string EmailFooter { get; set; }
        [Required]
        public string PrimaryEmailSubject { get; set; }
        [Required]
        public string PrimaryEmailBody { get; set; }
        [Required]
        public string SecondaryEmailSubject { get; set; }
        [Required]
        public string SecondaryEmailBody { get; set; }
        [Required]
        public string UserMessageText { get; set; }
        [Required]
        public string UserPrimaryMessageText { get; set; }
        [Required]
        public string UserSecondaryMessageText { get; set; }
        [Required]
        public string DownloadMessageText { get; set; }
    }
}

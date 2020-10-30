using System.ComponentModel.DataAnnotations;

namespace AnvilGroup.Applications.Fit2Work.Models {
    public class UploadModel {
        [Required]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        [Required(ErrorMessage ="Please choose a file to upload.")]
        public string File { get; set; }
    }
}
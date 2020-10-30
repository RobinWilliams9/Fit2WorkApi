using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("Client")]
    public class ClientModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        [JsonIgnore]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string MemberCode { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [StringLength(128)]
        [JsonIgnore]
        public string PrimaryEmailAddress { get; set; }
        [Required]
        [StringLength(128)]
        [JsonIgnore]
        public string SecondaryEmailAddress { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public virtual List<UserInfoModel> Users { get; set; }
    }
}
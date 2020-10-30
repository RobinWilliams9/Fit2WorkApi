using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("User")]
    public class UserInfoModel {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        [StringLength(128)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(128)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ClientModel Client { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime? SmsSentDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ReminderSettings { get; set; }

        public DateTime? ReminderSettingsUpdatedDate { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        public override string ToString() {            
            return $"Id: {Id}," + 
                $"ClientId: {ClientId}, " +
                $"FirstName: {FirstName}, " +
                $"LastName: {LastName}, " +
                $"PhoneNumber: {PhoneNumber} ";
        }
    }
}
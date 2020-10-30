using System.ComponentModel.DataAnnotations.Schema;

namespace AnvilGroup.Services.Fit2Work.Models {
    [NotMapped]
    public class UserAnswerModel {
        public string Question { get; set; }
        public string Answer { get; set; }

        public override string ToString() {
            return $"{Question}: {Answer}";
        }
    }
}

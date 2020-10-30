using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2Work.Models {
    [Table("UserQuestionnaire")]
    public class UserQuestionnaireModel {
        // TODO : Best approach to handling JSON https://stackoverflow.com/questions/44829824/how-to-store-json-in-an-entity-field-with-ef-core
        private string _questionAnswerData;
        public UserQuestionnaireModel() {
            QuestionsAndAnswers = new List<UserAnswerModel>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual UserInfoModel User { get; set; }
        public virtual ClientModel Client { get; set; }
        [NotMapped]
        public List<UserAnswerModel> QuestionsAndAnswers { get; set; }
        [Required]
        public string QuestionsAndAnswersData {
            get {
                if (string.IsNullOrEmpty(_questionAnswerData)) {
                    _questionAnswerData = JsonConvert.SerializeObject(QuestionsAndAnswers);
                }
                return _questionAnswerData;
            }
            set {
                _questionAnswerData = value;
                QuestionsAndAnswers = JsonConvert.DeserializeObject<List<UserAnswerModel>>(
                    string.IsNullOrEmpty(value) ? "[]" : value);
            }
        }
        /// <summary>
        ///  There is only a single condition for the user being fit to work
        ///  which is all answers being NO.  Everything else is NOT fit to work.
        /// </summary>
        [NotMapped]
        public bool IsFit2Work
        {
            get
            {
                return !QuestionsAndAnswers
                    .Any(q => q.Answer.ToUpper().Equals("YES"));
            }
        }
        public override string ToString() {
            return $"Id: {Id}, " +
                $"ClientId: {ClientId}, " +
                $"UserId: {UserId}";
        }
        public string ToStringQuestionsAndAnswers() {
            return string.Join(", ", QuestionsAndAnswers);
        }
    }
}
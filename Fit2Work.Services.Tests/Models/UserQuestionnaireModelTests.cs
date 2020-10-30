using AnvilGroup.Services.Fit2Work.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fit2Work.Services.Tests.Models {
    [TestClass]
    public class UserQuestionnaireModelTests {
        const string YES = "Yes";
        const string NO = "No";
        const string Fit2WorkCondition1 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition1 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition2 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition3 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition4 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition5 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition6 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition7 =
            @"[{ 'Question': 'Question1', 'Answer': 'No' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition8 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition9 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition10 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition11 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'No' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition12 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition13 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'No' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";
        const string NotFit2WorkCondition14 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'No' } ]";
        const string NotFit2WorkCondition15 =
            @"[{ 'Question': 'Question1', 'Answer': 'Yes' },
               { 'Question': 'Question2', 'Answer': 'Yes' },
               { 'Question': 'Question3', 'Answer': 'Yes' },
               { 'Question': 'Question4', 'Answer': 'Yes' } ]";

        [DataTestMethod]
        [DataRow(Fit2WorkCondition1, DisplayName = "Fit2WorkCondition1")]
        public void IsFit2Work_YesConditions(string qaData) {
            // Arrange
            var q = new UserQuestionnaireModel { QuestionsAndAnswersData = qaData };
            // Act
            var result = q.IsFit2Work;
            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(NotFit2WorkCondition1, DisplayName = "NotFit2WorkCondition1")]
        [DataRow(NotFit2WorkCondition2, DisplayName = "NotFit2WorkCondition2")]
        [DataRow(NotFit2WorkCondition3, DisplayName = "NotFit2WorkCondition3")]
        [DataRow(NotFit2WorkCondition4, DisplayName = "NotFit2WorkCondition4")]
        [DataRow(NotFit2WorkCondition5, DisplayName = "NotFit2WorkCondition5")]
        [DataRow(NotFit2WorkCondition6, DisplayName = "NotFit2WorkCondition6")]
        [DataRow(NotFit2WorkCondition7, DisplayName = "NotFit2WorkCondition7")]
        [DataRow(NotFit2WorkCondition8, DisplayName = "NotFit2WorkCondition8")]
        [DataRow(NotFit2WorkCondition9, DisplayName = "NotFit2WorkCondition9")]
        [DataRow(NotFit2WorkCondition10, DisplayName = "NotFit2WorkCondition10")]
        [DataRow(NotFit2WorkCondition11, DisplayName = "NotFit2WorkCondition11")]
        [DataRow(NotFit2WorkCondition12, DisplayName = "NotFit2WorkCondition12")]
        [DataRow(NotFit2WorkCondition13, DisplayName = "NotFit2WorkCondition13")]
        [DataRow(NotFit2WorkCondition14, DisplayName = "NotFit2WorkCondition14")]
        [DataRow(NotFit2WorkCondition15, DisplayName = "NotFit2WorkCondition15")]
        public void IsFit2Work_NoConditions(string qaData) {
            // Arrange
            var q = new UserQuestionnaireModel { QuestionsAndAnswersData = qaData };
            // Act
            var result = q.IsFit2Work;
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void QuestionsAndAnswersData_WithoutData_DeserializesToEmptyList() {
            // Arrange
            var q = new UserQuestionnaireModel { QuestionsAndAnswersData = string.Empty };
            // Act
            var qa = q.QuestionsAndAnswers;
            // Assert
            Assert.IsNotNull(qa);
            Assert.AreEqual(0, qa.Count, "Expected 0 question answers");
        }

        [DataTestMethod]
        [DataRow(Fit2WorkCondition1, "No,No,No,No", DisplayName = "Fit2WorkCondition1")]
        [DataRow(NotFit2WorkCondition1, "No,No,No,Yes", DisplayName = "NotFit2WorkCondition1")]
        [DataRow(NotFit2WorkCondition2, "No,No,Yes,No", DisplayName = "NotFit2WorkCondition2")]
        [DataRow(NotFit2WorkCondition3, "No,No,Yes,Yes", DisplayName = "NotFit2WorkCondition3")]
        [DataRow(NotFit2WorkCondition4, "No,Yes,No,No", DisplayName = "NotFit2WorkCondition4")]
        [DataRow(NotFit2WorkCondition5, "No,Yes,No,Yes", DisplayName = "NotFit2WorkCondition5")]
        [DataRow(NotFit2WorkCondition6, "No,Yes,Yes,No", DisplayName = "NotFit2WorkCondition6")]
        [DataRow(NotFit2WorkCondition7, "No,Yes,Yes,Yes", DisplayName = "NotFit2WorkCondition7")]
        [DataRow(NotFit2WorkCondition8, "Yes,No,No,No", DisplayName = "NotFit2WorkCondition8")]
        [DataRow(NotFit2WorkCondition9, "Yes,No,No,Yes", DisplayName = "NotFit2WorkCondition9")]
        [DataRow(NotFit2WorkCondition10, "Yes,No,Yes,No", DisplayName = "NotFit2WorkCondition10")]
        [DataRow(NotFit2WorkCondition11, "Yes,No,Yes,Yes", DisplayName = "NotFit2WorkCondition11")]
        [DataRow(NotFit2WorkCondition12, "Yes,Yes,No,No", DisplayName = "NotFit2WorkCondition12")]
        [DataRow(NotFit2WorkCondition13, "Yes,Yes,No,Yes", DisplayName = "NotFit2WorkCondition13")]
        [DataRow(NotFit2WorkCondition14, "Yes,Yes,Yes,No", DisplayName = "NotFit2WorkCondition14")]
        [DataRow(NotFit2WorkCondition15, "Yes,Yes,Yes,Yes", DisplayName = "NotFit2WorkCondition15")]
        public void QuestionsAndAnswersData_DeserializesToList(string qaData, string answers) {
            // Arrange
            var q = new UserQuestionnaireModel { QuestionsAndAnswersData = qaData };
            var expectedAnswers = answers.Split(',');
            // Act
            var qa = q.QuestionsAndAnswers;
            // Assert
            Assert.IsNotNull(qa);
            Assert.AreEqual(expectedAnswers.Length, qa.Count, "Expected count to match");
            for (var i = 0; i < expectedAnswers.Length; i++) {
                Assert.AreEqual(expectedAnswers[i], qa[i].Answer,
                    $"Expected answer {i} to match");
            }
        }
    }
}

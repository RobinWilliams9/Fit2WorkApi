using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2WorkApi.Filters;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {
    public class QuestionnaireController : ApiController 
    {
        private IUserService _userService;
        private ILoggingService _loggingService;
        public QuestionnaireController() {
            _userService = new UserService();
            _loggingService = new LoggingService();
        }
        public QuestionnaireController(IUserService userService, ILoggingService loggingService) {
            _userService = userService;
            _loggingService = loggingService;
        }
        /// <summary>
        /// Post: Submission of questionnaire by user.
        /// </summary>
        /// <returns>HttpStatusCode.OK</returns>
        [ValidateModel]
        [HttpPost]
        public HttpResponseMessage Post(UserQuestionnaireModel questionnaire) {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                var result = _userService.SubmitUserQuestionnaire(questionnaire);
                response.Content = new StringContent(JsonConvert.SerializeObject(result));
            } catch (Exception ex) {
                _loggingService.LogCriticalError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}

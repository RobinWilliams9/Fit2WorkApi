using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Helpers;
using AnvilGroup.Services.Fit2WorkApi.Filters;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {
    public class ReminderSettingsController : ApiController 
    {
        private IUserService _userService;
        private ILoggingService _loggingService;
        public ReminderSettingsController() {
            _userService = new UserService();
            _loggingService = new LoggingService();
        }
        public ReminderSettingsController(IUserService userService, ILoggingService loggingService) {
            _userService = userService;
            _loggingService = loggingService;
        }
        /// <summary>
        /// Post: Submission of questionnaire by user.
        /// </summary>
        /// <returns>HttpStatusCode.OK</returns>
        [ValidateModel]
        [HttpPost]
        public HttpResponseMessage Post(ReminderSettingsReceiver reminderSettings) {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                _userService.SubmitReminderSettings(reminderSettings);
                //response.Content = new StringContent(JsonConvert.SerializeObject(result));
            } catch (Exception ex) {
                //_loggingService.LogCriticalError(ex);
                //response = Request.CreateErrorResponse(
                //    HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }


        
    }


  
}

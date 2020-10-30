using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2WorkApi.Filters;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {    
    public class UserController : ApiController {
        private IUserService _userService;
        private ILoggingService _loggingService;

        public UserController() {
            _userService = new UserService();
            _loggingService = new LoggingService();
        }
        public UserController(IUserService userService, ILoggingService loggingService) {
            _userService = userService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Post: Registes an existing user.
        /// </summary>
        /// <returns>JSON representation of the created user.</returns>
        [ValidateModel]
        [HttpPost]
        public HttpResponseMessage Post(UserInfoModel userInfo) {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                _userService.RegisterUser(userInfo);
                response.Content = new StringContent(JsonConvert.SerializeObject(userInfo));                
            } catch (ArgumentOutOfRangeException ex) { // invalid phone number
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.NotAcceptable, ex.Message);
            } catch (ArgumentNullException ex) { // no user found
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.NotFound, ex.Message);
            } catch (ArgumentException ex) { // client is deleted
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.Conflict, ex.Message);
            } catch (Exception ex) {
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }
    }
}

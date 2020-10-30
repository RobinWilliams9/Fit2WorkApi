using System;
using System.Linq;
using System.Web.Mvc;
using AnvilGroup.Applications.Fit2Work.Helpers;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Providers;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class BaseController : Controller {
        public Fit2WorkDb Fit2WorkDb { get; internal set; }
        public IClientService ClientService { get; internal set; }
        public IUserService UserService { get; internal set; }
        public IMessageService MessageService { get; internal set; }
        public ILoggingService LoggingService { get; internal set; }
        public IResourceProvider ResourceProvider { get; internal set; }

        public BaseController() {
            Fit2WorkDb = new Fit2WorkDb();
            ClientService = new ClientService(Fit2WorkDb);
            UserService = new UserService(Fit2WorkDb);
            MessageService = new MessageService(Fit2WorkDb);
            LoggingService = new LoggingService(Fit2WorkDb);
            ResourceProvider = new ResourceProvider(Fit2WorkDb);
        }

        public BaseController(IClientService clientService,
            IUserService userService) {
            ClientService = clientService;
            UserService = userService;
            MessageService = new MessageService();
            LoggingService = new LoggingService();
            ResourceProvider = new ResourceProvider(new Fit2WorkDb());
        }

        public BaseController(IClientService clientService,
            IUserService userService,
            IMessageService messageService, 
            ILoggingService loggingService,
            IResourceProvider resourceProvider) {
            ClientService = clientService;
            UserService = userService;            
            MessageService = messageService;
            LoggingService = loggingService;
            ResourceProvider = resourceProvider;
        }

        protected ActionResult CreateActionResult(string content) {
            return Json(new {
                success = true,
                responseText = content
            });
        }

        protected JsonResult CreateJsonResult(string content) {
            return Json(new {
                success = true,
                responseText = content
            });
        }

        protected ActionResult CreateActionResult(Exception exception) {
            return Json(new {
                success = false,
                responseText = exception.HtmlMessage()
            });
        }

        protected JsonResult CreateJsonResult(Exception exception) {
            return Json(new {
                success = false,
                responseText = exception.HtmlMessage()
            });
        }

        protected ActionResult CreateValidationActionResult() {
            return Json(new {
                success = false,
                responseText = string.Join("; ",
                    ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            });
        }

        protected JsonResult CreateValidationJsonResult() {
            return Json(new {
                success = false,
                responseText = string.Join("; ",
                    ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            });
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                Fit2WorkDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
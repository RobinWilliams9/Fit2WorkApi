using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AnvilGroup.Applications.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using Microsoft.AspNet.SignalR;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class HomeController : BaseController {
        public HomeController() : base() { }
        public HomeController(IClientService clientService, IUserService userService)
            : base(clientService, userService) { }

        public ActionResult Index() {
            return View(ClientService.GetClients());
        }
        public ActionResult Upload(int id) {
            var client = ClientService.GetClientById(id);
            return View(new UploadModel {
                ClientId = client.Id,
                ClientName = client.Name
            });
        }
        [HttpPost]
        public JsonResult Upload(UploadModel model) {
            if (!ModelState.IsValid) {
                return CreateValidationJsonResult();
            }
            if (Request.Files == null || Request.Files.Count == 0) {
                return CreateJsonResult(
                    new ApplicationException("No file found."));
            }
            var postedFile = Request.Files[0];
            if (!postedFile.FileName.ToUpper().EndsWith(".CSV")) {
                return CreateJsonResult(
                    new ApplicationException("File must be CSV."));
            }
            try {
                UserService.ImportUsers(
                    model.ClientId,
                    Path.GetFileName(postedFile.FileName),
                    postedFile.InputStream,
                    LogProgress);
            } catch (Exception ex) {
                return CreateJsonResult(ex);
            }
            return CreateJsonResult("File uploaded");
        }

        [HttpPost]
        public ActionResult ViewSMS(ClientModel client) {
            var smsModel = new SMSModel {
                Client = client,
                Users = client.Users.Where(u => u.Selected).ToList(),
                MessageText = ResourceProvider.UserMessageText,
                DownloandLinkMessageText = ResourceProvider.DownloadMessageText
            };
            smsModel.Client.Users = null; // no point carrying these around

            // need to clear model state as a partial is being re-used in the view
            ModelState.Clear();
            return View(smsModel);
        }
        [HttpPost]
        public JsonResult SendSMS(SMSModel smsModel) {
            var results = new List<MessageResult>();
            var client = ClientService.GetClientById(smsModel.Client.Id);
            var messageText = MessageService.PopulatePlaceHoldersSms(client);
            var downloadMessageText = MessageService.GenerateDownLinksSMSText();

            LogProgress($"Sending SMS :: {messageText}");
            // Get user mobile numbers and loop through each one to send SMS
            foreach (var user in smsModel.Users) {
                try {
                    var result = MessageService.SendSms(user.PhoneNumber, messageText);
                    LogProgress($"{(result.IsOk ? "SUCCESS" : "FAILED")} :: SMS to {user.PhoneNumber}. " +
                        $"{(result.Exception == null ? string.Empty : result.Exception.Message)}");
                    if (result.IsOk) {
                        UserService.MarkSmsSent(user.Id);
                    }

                    result = MessageService.SendSms(user.PhoneNumber, downloadMessageText);

                    LogProgress($"{(result.IsOk ? "SUCCESS" : "FAILED")} :: Download SMS to {user.PhoneNumber}. " +
                        $"{(result.Exception == null ? string.Empty : result.Exception.Message)}");

                } catch (Exception ex) {
                    LogProgress($"Failed to send SMS to {user.PhoneNumber} : {ex.Message}");
                }
            }
            LogProgress($"Sending SMS complete!");
            return CreateJsonResult("Sending SMS complete!");            
        }

        private void LogProgress(string message) {
            var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            context.Clients.All.AddProgressMessage(message);
        }
        public class ProgressHub : Hub { }
    }
}
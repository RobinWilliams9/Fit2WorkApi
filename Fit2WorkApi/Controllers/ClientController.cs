using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using AnvilGroup.Services.Fit2Work.Services;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {
    public class ClientController : ApiController {
        private IClientService _clientService;
        private ILoggingService _loggingService;

        public ClientController() {
            _clientService = new ClientService();
            _loggingService = new LoggingService();
        }
        public ClientController(IClientService clientService, ILoggingService loggingService) {
            _clientService = clientService;
            _loggingService = loggingService;
        }
        /// <summary>
        /// GET: Validates the given member code.
        /// http://localhost:62780/api/client/test1
        /// </summary>
        /// <param name="id">Member code</param>
        /// <returns>JSON representation of Client</returns>
        public HttpResponseMessage Get(string id) {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                var client = _clientService.GetClientByCode(id);
                if(client == null) {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                response.Content = new StringContent(JsonConvert.SerializeObject(client));
            } catch(Exception ex) {
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, ex);
            }


            //string strFrom = ConfigurationManager.AppSettings["SupportEmail.FromAddress"];
            //string strTo = ConfigurationManager.AppSettings["SupportEmail.ToAddress"];
            //SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            //using (MailMessage mm = new MailMessage(strFrom, strTo))
            //{
            //    mm.Subject = "Health Assessments - Not Completed";
            //    mm.Body = id;
            //    mm.IsBodyHtml = false;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = smtpSection.Network.Host;
            //    smtp.EnableSsl = smtpSection.Network.EnableSsl;
            //    NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
            //    smtp.UseDefaultCredentials = smtpSection.Network.DefaultCredentials;
            //    smtp.Credentials = networkCred;
            //    smtp.Port = smtpSection.Network.Port;
            //    smtp.Send(mm);


            //}
            return response;
        }
    }
}

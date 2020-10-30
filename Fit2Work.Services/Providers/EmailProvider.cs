using System;
using System.Net.Mail;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public class EmailProvider : IEmailProvider {
        public MessageResult SendMail(string from, string to, string subject, string body) {
            var result = new MessageResult {
                Recipient = to
            };
            try {
                using (var smtpClient = new SmtpClient()) {
                    var mailMessage = new MailMessage(from, to, subject, body);
                    mailMessage.IsBodyHtml = true;
                    smtpClient.Send(mailMessage);
                }
            } catch (Exception ex) {
                result.IsOk = false;
                result.Exception = ex;
            }
            return result;
        }
    }
}

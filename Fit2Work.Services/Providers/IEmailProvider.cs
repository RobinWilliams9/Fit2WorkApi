using System.Net.Mail;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public interface IEmailProvider {
        MessageResult SendMail(string from, string to, string subject, string body);
    }
}
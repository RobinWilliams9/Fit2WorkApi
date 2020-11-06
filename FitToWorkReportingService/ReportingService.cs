using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;

namespace FitToWorkReportingService
{
    public partial class ReportingService : ServiceBase
    {

        System.Timers.Timer timer = new System.Timers.Timer();

        public ReportingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);

            var d = DateTime.Now;
            var dMidnight = new DateTime(d.Year, d.Month, d.Day).AddDays(1);
            //timer.Interval = 20000; //testing
            timer.Interval = (dMidnight - d).TotalMilliseconds - 300000;
            timer.Start();
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }


        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timer.Interval = 24 * 3600 * 1000; // 24 hours 
            //get all users and decode reminder json
            UserService userService = new UserService();
            ClientService clientService = new ClientService();
            var v = userService.GetUsersNoAssessmentToday();
            var userList = v.OrderBy(f=>f.FirstName + " " + f.LastName).ToList();
            var clientlist = clientService.GetClients();

            //for each client get list of users with no assessment that day and send email to the primary address
            foreach(var c in clientlist)
            {
                var clientuser = userList.Where(x => x.ClientId == c.Id).ToList();
                if(clientuser.Count() > 0) { 

                    string htmltable = "<table border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'><tr><td><img src = 'https://tag.anvilgroup.com/FitToWork/images/email-header.png' style = 'border:0;padding:0;margin:0;' /></td></ tr></table> " +
                        "<p style='font-family:'Open Sans', sans-serif;font-size:14px; padding:30px;' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'>The following people did not complete their health assessments on " + DateTime.Now.Day.ToString("N0") + "/" + DateTime.Now.Month.ToString("N0") + "/" + DateTime.Now.Year.ToString() + ".</p><table style='font-family:'Open Sans', sans-serif;font-size:14px; padding:30px;' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'>";
                    foreach (UserInfoModel u in clientuser)
                    {
                        htmltable += "<tr><td><ul><li>" + u.FirstName + " " + u.LastName + "</li></ul></td></tr>";
                    }
                    htmltable += "</table><table border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'><tr><td><img src = 'https://tag.anvilgroup.com/FitToWork/images/email-footer.png' style = 'border:0;padding:0;margin:0;'/></td></tr></table>";

                    SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                    using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["SupportEmail.FromAddress"], c.PrimaryEmailAddress))
                    {
                        mm.Subject = "Health Assessments - Not Completed";
                        mm.Body = htmltable;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = smtpSection.Network.Host;
                        smtp.EnableSsl = smtpSection.Network.EnableSsl;
                        NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                        smtp.UseDefaultCredentials = smtpSection.Network.DefaultCredentials;
                        smtp.Credentials = networkCred;
                        smtp.Port = smtpSection.Network.Port;
                        smtp.Send(mm);
                    }

                }

            }

        //ConfigurationManager.AppSettings["SupportEmail.ToAddress"])


        
            timer.Start();

        }
    }
}

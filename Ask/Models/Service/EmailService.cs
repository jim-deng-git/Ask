using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;
using WorkV3.Models.Repository;

namespace WorkV3.Models.Service
{
    public class EmailService: IEmailService
    {
        private ISiteMailSettingRepository<SiteMailSettingModel> siteMailSettingRepository = new SiteMailSettingRepository();

        public void SendMailWithFiles(long siteId, string mailToEmail, string mailToName, string mailSubject, string mailContent, ArrayList attachFile, object fromEmail = null, object fromName = null)
        {
            var setting = siteMailSettingRepository.GetItem(siteId, "SiteID");

            SetMailInfo(setting);
            WorkLib.uEMail.SendMailWithFiles(mailToEmail, mailToName, mailSubject, mailContent, attachFile, fromEmail, fromName);
        }

        public void SendEMail(long siteId, string mailToEmail, string mailToName, string mailSubject, string mailContent, object fromEmail = null, object fromName = null)
        {
            var setting = siteMailSettingRepository.GetItem(siteId, "SiteID");
            
            SetMailInfo(setting);
            WorkLib.uEMail.SendEMail(mailToEmail, mailToName, mailSubject, mailContent, fromEmail, fromName);
        }

        private void SetMailInfo(SiteMailSettingModel setting)
        {
            try
            {
                WorkLib.uEMail.MailFrom = setting.Account;
                WorkLib.uEMail.MailFromName = setting.Sender;
                WorkLib.uEMail.MailPort = setting.Port;
                WorkLib.uEMail.MailPassword = setting.Password;
                WorkLib.uEMail.EnableSSL = setting.UseSsl;
                WorkLib.uEMail.MailHost = setting.SmtpServer;
            }
            catch(Exception)
            {
                WorkLib.uEMail.MailFrom = WorkLib.uEMail.MailFromDefault;
                WorkLib.uEMail.MailFromName = WorkLib.uEMail.MailFromNameDefault;
                WorkLib.uEMail.MailPort = WorkLib.uEMail.MailPortDefault();
                WorkLib.uEMail.MailPassword = WorkLib.uEMail.MailPasswordDefault;
                WorkLib.uEMail.EnableSSL = WorkLib.uEMail.EnableSSLDefault();
                WorkLib.uEMail.MailHost = WorkLib.uEMail.MailHostDefault;
            }
        }
    }
}
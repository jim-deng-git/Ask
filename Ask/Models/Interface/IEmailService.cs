using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3.Models.Interface
{
    public interface IEmailService
    {
        //int CheckEmailsExist(string mailAddr);
        //bool EnableSSLDefault();
        //string GetMailServer(string mailAddr);
        void SendMailWithFiles(long siteId, string mailToEmail, string mailToName, string mailSubject, string mailContent, ArrayList attachFile, object fromEmail = null, object fromName = null);
        void SendEMail(long siteId, string mailToEmail, string mailToName, string mailSubject, string mailContent, object fromEmail = null, object fromName = null);
    }
}

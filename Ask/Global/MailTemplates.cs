using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.App_Code.Golbal
{
    public class MailTemplates
    {

        public static string[] GetMailTemp(string mailTempCode)
        {
            string title = "";
            string content = "";
            string tempPath= HttpContext.Current.Server.MapPath("~") + @"\App_Data\MailContent\" + mailTempCode + ".txt";
            if (File.Exists(tempPath))
            {
                string[] tempData = File.ReadAllLines(tempPath,System.Text.Encoding.UTF8);
                title = tempData[0];
                tempData[0] = "";
                content = string.Join("", tempData);
            }
            return new string[] { title, content };
        }

    }
}
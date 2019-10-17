using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Dapper;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace WorkV3.Models.DataAccess
{
    /// <summary>
    /// 網站地圖產製(sitemap)排程
    /// </summary>
    public class SiteCoverRecreateSchedule
    {
        private  string ApplicationPath = "";
        private static object _thisLock = new object();
        private static Timer _timer;
        //const int _StartDay = 1; //每月的幾號啟動
        //const int _StartHour = 1; //幾點啟動

        /// <summary>
        /// 開始排程
        /// </summary>
        public  SiteCoverRecreateSchedule(string applicationPath)
        {
            ApplicationPath = applicationPath;
        }
        /// <summary>
        /// 開始排程
        /// </summary>
        public void StartTimer()
        {
            TimeSpan StartDelayTime = new TimeSpan(0, 0, 10); // 應用程式起動後多久開始執行
            TimeSpan IntervalTime = new TimeSpan(1, 0, 0); // 應用程式起動後間隔多久重複執行
            TimerCallback ProcessMethod = new TimerCallback(BgProcess);  // 呼叫方法

            _timer = new Timer(ProcessMethod, null, StartDelayTime, IntervalTime);  // 產生 Timer
        }
        /// <summary>
        /// 背景處理
        /// </summary>
        /// <param name="Status"></param>
        private void BgProcess(object Status)
        {

            string exePath = "~/App_Consoles/WebsiteSnap/bin/Debug";
            if (!System.IO.Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(exePath)))
                return;

            //  string logFileName =System.Web.Hosting.HostingEnvironment.MapPath("~/SitemapCreateLog.txt");
            DateTime today = DateTime.Now;
            string exeHour = ((int)WorkLib.GetItem.appSet("SitemapCreateHour")).ToString("00");
            //if (today.ToString("HH") == exeHour) // 每天於固定時間自動產製
            //{
               
                var sites = Models.DataAccess.SitesDAO.GetDatas();
                if (sites != null && sites.Count > 0)
            {
                //WorkLib.WriteLog.Write(true, "Start Snap Exe");

                foreach (Models.SitesModels site in sites)
                {
                    string uploadPath = WorkV3.Golbal.UpdFileInfo.GetUPathBySiteID(site.Id, "SiteCover").TrimEnd('\\') + "\\Default.jpg";
                    
                    string url = string.Format("{0}/{1}/w/{2}/Index", WorkLib.GetItem.appSet("WebUrl").ToString(),
                            ApplicationPath, site.SN);
                    //WorkLib.WriteLog.Write(true, "Site : " + site.SN + " Snap Start ");
                    System.Diagnostics.Process snapProcess = new System.Diagnostics.Process();
                    snapProcess.StartInfo.Arguments = string.Format("{0} {1}", url, uploadPath);
                    // Set the directory   
                    snapProcess.StartInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath(exePath); //or wherever your file is
                                                                                                                                                    // Set the filename
                    snapProcess.StartInfo.FileName = System.Web.Hosting.HostingEnvironment.MapPath(exePath+"/WebsiteSnap.exe");
                    // Start the process    
                    snapProcess.Start();
                    //try
                    //{

                    //    string url = string.Format("{0}://{1}{2}{3}/w/{4}/Index", HttpContext.Current.Request.Url.Scheme,
                    //        HttpContext.Current.Request.Url.DnsSafeHost,
                    //        HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                    //        HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                    //        site.SN);

                    //    string uploadPath = WorkV3.Golbal.UpdFileInfo.GetUPathBySiteID(site.Id, "SiteCover").TrimEnd('\\') + "\\";

                    //    if (!System.IO.Directory.Exists(uploadPath))
                    //        System.IO.Directory.CreateDirectory(uploadPath);
                    //    string thumb = site.SN + ".jpg";
                    //    string coverFilePath = uploadPath + thumb;
                    //    if (System.IO.File.Exists(coverFilePath))
                    //        System.IO.File.Delete(coverFilePath);
                    //    Common.SnapCover.GetSnapCover(url, coverFilePath);
                    //    WorkLib.WriteLog.Write(true, "Site : " + site.SN + " success! ");
                    //}
                    //catch (Exception exp)
                    //{
                    //    WorkLib.WriteLog.Write(true, "Site : " + site.SN + " fail! "+ exp.Message);
                    //}

                }

            }
            //}
        }
        public void SnapCoverNow(string url, string photoSavePath)
        {
            string exePath = "~/App_Consoles/WebsiteSnap/bin/Debug";
            if (!System.IO.Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(exePath)))
                return;
            //WorkLib.WriteLog.Write(true, "Url : " + url + " Snap Start ");
            System.Diagnostics.Process snapProcess = new System.Diagnostics.Process();
            snapProcess.StartInfo.Arguments = string.Format("{0} {1}", url, photoSavePath);
            // Set the directory   
            snapProcess.StartInfo.WorkingDirectory = System.Web.Hosting.HostingEnvironment.MapPath(exePath); //or wherever your file is
                                                                                                                                            // Set the filename
            snapProcess.StartInfo.FileName = System.Web.Hosting.HostingEnvironment.MapPath(exePath+"/WebsiteSnap.exe");

            // Start the process    
            snapProcess.Start();
            snapProcess.Exited += (sender, e) => Process_Exited(sender, e, url);
        }
        protected void Process_Exited(object sender, EventArgs e, string url)
        {
            //WorkLib.WriteLog.Write(true, "Url : " + url + " Proecess Exit ");

        }
    }
}
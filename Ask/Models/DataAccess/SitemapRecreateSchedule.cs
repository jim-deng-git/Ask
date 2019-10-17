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
    public class SitemapRecreateSchedule
    {
        private static object _thisLock = new object();
        private static Timer _timer;
        //const int _StartDay = 1; //每月的幾號啟動
        //const int _StartHour = 1; //幾點啟動

        /// <summary>
        /// 開始排程
        /// </summary>
        public void StartTimer()
        {
            TimeSpan StartDelayTime = new TimeSpan(0, 0, 5); // 應用程式起動後多久開始執行
            TimeSpan IntervalTime = new TimeSpan(0, 30, 0); // 應用程式起動後間隔多久重複執行
            TimerCallback ProcessMethod = new TimerCallback(BgProcess);  // 呼叫方法

            _timer = new Timer(ProcessMethod, null, StartDelayTime, IntervalTime);  // 產生 Timer
        }
        /// <summary>
        /// 背景處理
        /// </summary>
        /// <param name="Status"></param>
        private void BgProcess(object Status)
        {
            string logFileName =System.Web.Hosting.HostingEnvironment.MapPath("~/SitemapCreateLog.txt");
            DateTime today = DateTime.Now;
            string exeHour = ((int)WorkLib.GetItem.appSet("SitemapCreateHour")).ToString("00");
            System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " exe "+ today.ToString("HH") + System.Environment.NewLine);
            System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " config value "+ exeHour + System.Environment.NewLine);
            if (today.ToString("HH") == exeHour) // 每天於固定時間自動產製
            {
                System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " time match " + System.Environment.NewLine);
                var sites = Models.DataAccess.SitesDAO.GetDatas();
                if (sites != null && sites.Count > 0)
                {
                    foreach (Models.SitesModels site in sites)
                    {
                        try
                        {
                            Golbal.PubFunc.CreateSitemap(site.Id, System.Web.Hosting.HostingEnvironment.MapPath("~/"));
                            System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " create map " + site.Title + System.Environment.NewLine);
                        }
                        catch(Exception exp) {
                            System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + " create map " + site.Title+ "  error " + System.Environment.NewLine);
                            System.IO.File.AppendAllText(logFileName, DateTime.Now.ToString() + exp.Message + System.Environment.NewLine);
                        }
                        
                    }
                }
            }
        }
    }
}
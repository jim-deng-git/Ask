using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ResourceVideosDAO
    {
        public static ResourceVideosModels GetItem(long sourceNo, string code) {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select Top(1) ID, Type, Link, SizeType, PlayMode, IsAuto, IsRepeat, ShowSpec, Spec, ShowDuration, Duration, ShowViews, ScreenshotIsCustom, Screenshot From ResourceVideos Where {0}";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceVideosModels>(string.Format(sql, string.Join(" And ", where))).SingleOrDefault();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class PagesView_LogDAO
    {
        public static bool AddPageLogs(PagesView_LogModel logModel)
        {
            PagesModels nData = new PagesModels();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = @" UPDATE PagesView_Log SET StaySeconds=LastPageViewLog.StaySeconds
                                FROM 
                                (
                                SELECT ID, DATEDIFF(SECOND, [AddTime], GETDATE()) AS StaySeconds
	                                FROM
	                                (
		                                SELECT top 1 * FROM PagesView_Log
		                                WHERE SessionID=@SessionID ORDER BY AddTime DESC
	                                ) 
	                                AS LastPageViewTMP
                                ) AS LastPageViewLog
                                WHERE LastPageViewLog.ID=PagesView_Log.ID;
                                INSERT PagesView_Log(PagesNo, ReferrerUrl, ReferrerUrlTitle, ReferrerUrlPageNo, Lang, Browser, UserAgent, SiteID, SessionID, MemberID, AddTime, IP, IPNum) 
                                        VALUES (@PagesNo, @ReferrerUrl, @ReferrerUrlTitle, @ReferrerUrlPageNo, @Lang, @Browser, @UserAgent, @SiteID, @SessionID, @MemberID, @AddTime, @IP, @IPNum);";
                
                conn.Execute(sql, new
                {
                    PagesNo = logModel.PagesNo,
                    ReferrerUrl = logModel.ReferrerUrl,
                    ReferrerUrlTitle = logModel.ReferrerUrlTitle,
                    ReferrerUrlPageNo = logModel.ReferrerUrlPageNo,
                    Lang = logModel.Lang,
                    Browser = logModel.Browser,
                    UserAgent = logModel.UserAgent,
                    SiteID = logModel.SiteID,
                    SessionID = logModel.SessionID,
                    MemberID = logModel.MemberID,
                    AddTime = logModel.AddTime,
                    IP = logModel.IP,
                    IPNum = logModel.IPNum
                });
            }
            return true;
        }
        public static bool UpdateLastStaySeconds(string sessionID)
        {
            PagesModels nData = new PagesModels();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = @"UPDATE PagesView_Log SET StaySeconds=LastPageViewLog.StaySeconds
                                FROM 
                                (
                                SELECT ID, DATEDIFF(SECOND, [AddTime], GETDATE()) AS StaySeconds
	                                FROM
	                                (
		                                SELECT top 1 * FROM PagesView_Log
		                                WHERE SessionID=@SessionID ORDER BY AddTime DESC
	                                ) 
	                                AS LastPageViewTMP
                                ) AS LastPageViewLog
                                WHERE LastPageViewLog.ID=PagesView_Log.ID;";

                conn.Execute(sql, new
                {
                    SessionID = sessionID
                });
            }
            return true;
        }
    }

}
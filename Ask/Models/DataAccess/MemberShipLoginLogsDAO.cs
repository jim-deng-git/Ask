using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class MemberShipLoginLogsDAO
    {
        public static bool AddLoginLogs(MemberShipLoginLogModel logModel)
        {

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = @" INSERT MemberShipLoginLogs(MemberShipID,Browser,UserAgent,IP,IPNum,AddTime) 
                                        VALUES (@MemberShipID,@Browser, @UserAgent, @IP, @IPNum, @AddTime);";
                
                conn.Execute(sql, new
                {
                    MemberShipID = logModel.MemberShipID,
                    Browser = logModel.Browser,
                    UserAgent = logModel.UserAgent,
                    IP = logModel.IP,
                    IPNum = logModel.IPNum,
                    AddTime = logModel.AddTime
                });
            }
            return true;
        }
    }

}
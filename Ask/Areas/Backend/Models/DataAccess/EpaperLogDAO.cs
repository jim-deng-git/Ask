using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class EpaperLogDAO
    {
        /// <summary>
        /// 寫入 Log
        /// </summary>
        /// <param name="Message">訊息</param>
        /// <param name="Remark">備註</param>
        /// <param name="Type">訊息類型</param>
        /// <param name="EpaperSendId"></param>
        /// <returns></returns>
        public static bool WriteLog(string Message = "", string Remark = "", EpaperLogType Type = EpaperLogType.Start, long EpaperSendId = 0)
        {
            try
            {
                using (var conn = new SqlConnection(WebInfo.Conn))
                {
                    string type = Type.ToString();
                    string sql = @" INSERT INTO EpaperLog(Type, Message, Remark, EpaperSendID, CreateTime) VALUES(@Type, @Message, @Remark, @EpaperSendID, CURRENT_TIMESTAMP) ";
                    conn.Execute(sql, new { type, Message, Remark, EpaperSendId });

                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
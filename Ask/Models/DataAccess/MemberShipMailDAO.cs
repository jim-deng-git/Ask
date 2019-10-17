using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Models.DataAccess {
    public class MemberShipMailDAO
    {
        public static MemberShipMailModel GetItem(long id) {
            return CommonDA.GetItem<MemberShipMailModel>("MemberShipSendMail", id);
        }

        public static IEnumerable<MemberShipMailModel> GetItems() {
            string sql = $"Select * From MemberShipSendMail Order By SendDate Desc";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<MemberShipMailModel>(sql);
            }
        }

        public static void SetItem(MemberShipMailModel item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("MemberShipSendMail");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From MemberShipSendMail Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("SendDate");
                
                tableObj.Update(item.ID);
            }
        }

        public static void WriteLog(long mailId, long memberId) {
            string sql = $"Insert MemberShipSendMailLog(MailID, MemberShipID) Values({ mailId }, { memberId })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static void WriteReadLog(long mailId, long memberId) {
            string sql = $"Update MemberShipSendMailLog Set ReadDate = GetDate() Where MailID = { mailId } AND MemberShipID = { memberId } AND ReadDate Is Null";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static IEnumerable<MemberShipMailLogModel> GetMailLogs(long mailId) {
            string sql = $"Select * From MemberShipSendMailLog Where MailID = { mailId }";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<MemberShipMailLogModel>(sql);
            }
        }
        public static IEnumerable<MemberShipModels> GetMemberMailLogItems(long mailId) {
            string sql = $"Select * From MemberShip WHERE ID IN (Select MemberShipID From MemberShipSendMailLog Where MailID = { mailId })";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<MemberShipModels>(sql);
            }
        }
    }
}
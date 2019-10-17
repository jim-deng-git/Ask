using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Models.DataAccess {
    public class FormMailDAO {
        public static FormMailModel GetItem(long id) {
            return CommonDA.GetItem<FormMailModel>("FormMail", id);
        }

        public static IEnumerable<FormMailModel> GetItems(long formId) {
            string sql = $"Select * From FormMail Where FormID = { formId } Order By SendDate Desc";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormMailModel>(sql);
            }
        }

        public static void SetItem(FormMailModel item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("FormMail");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From FormMail Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("FormID");
                tableObj.Remove("SendDate");
                
                tableObj.Update(item.ID);
            }
        }

        public static void WriteLog(long mailId, long formItemId) {
            string sql = $"Insert FormMailLog(MailID, FormItemID) Values({ mailId }, { formItemId })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static void WriteReadLog(long mailId, long formItemId) {
            string sql = $"Update FormMailLog Set ReadDate = GetDate() Where MailID = { mailId } AND FormItemID = { formItemId } AND ReadDate Is Null";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static IEnumerable<FormMailLogModel> GetMailLogs(long mailId) {
            string sql = $"Select * From FormMailLog Where MailID = { mailId }";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormMailLogModel>(sql);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class SEODAO
    {
        public static SEOModels GetItem(long sourceNo) {
            string sql = "Select * From SEO Where SourceNo = " + sourceNo;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {                
                return conn.Query<SEOModels>(sql).SingleOrDefault();
            }            
        }

        public static void SetItem(SEOModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("SEO");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From SEO Where SourceNo = " + item.SourceNo;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Add("Creator", WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id);
                tableObj.Add("CreateTime", DateTime.Now);

                tableObj.Insert();
            } else {
                tableObj.Remove("SourceNo");
                tableObj.Remove("MenuID");
                
                tableObj.Add("Modifier", WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id);
                tableObj.Add("ModifyTime", DateTime.Now);

                tableObj.Update(item.SourceNo);
            }
        }

        public static IEnumerable<string> GetKeywords(long siteId, long menuId) {
            string sql = $"SELECT MenuID, Keywords FROM SEO Where Keywords <> '' AND MenuID IN (SELECT ID FROM Menus WHERE SiteID = { siteId })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);

            Dictionary<string, int> wordList = new Dictionary<string, int>();
            foreach(DataRow dr in datas.Rows) {
                long mId = (long)dr["MenuID"];
                string[] words = dr["Keywords"].ToString().Trim().Split(';');
                foreach(string w in words) {
                    if (!wordList.ContainsKey(w))
                        wordList[w] = 0;
                    wordList[w] += (mId == menuId ? 3 : 2);
                }
            }

            return wordList.OrderByDescending(w => w.Value).Select(w => w.Key);
        }        
    }
}
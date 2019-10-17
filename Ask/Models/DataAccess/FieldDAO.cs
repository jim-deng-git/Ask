using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

namespace WorkV3.Models
{
    public class FieldDAO : BaseDAO<FieldModel>
    {
        public static FieldDAO Instance;

        static FieldDAO()
        {
            Instance = new FieldDAO();
        }

        public IEnumerable<FieldModel> GetFromParentID(IEnumerable<long> ParentIDs)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                try
                {
                    var param = new Dictionary<string, object>();
                    var commandText = $"SELECT * FROM [{TableName}] WHERE ParentID IN ({string.Join(",", ParentIDs)})";
                    return connection.Query<FieldModel>(commandText);
                }
                catch { throw; }
            }
        }

        public static IEnumerable<FieldModel> GetItems(long formId) {
            // string sql = $"SELECT F.* FROM Field F JOIN FieldList L ON F.ParentID = L.ID WHERE L.ParentID = { formId } ORDER BY L.SN, F.ParentID, F.SN";
            string sql = $"SELECT * FROM Field WHERE ParentID = { formId } ORDER BY SN";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FieldModel>(sql);
            }
        }

        public static IEnumerable<FieldModel> GetItems(IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return new List<FieldModel>();

            string sql = $"SELECT * FROM Field WHERE ID IN ({ string.Join(", ", ids) }) ORDER BY SN";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FieldModel>(sql);
            }
        }

        public static FieldModel GetItem(long id) {
            return DataAccess.CommonDA.GetItem<FieldModel>("Field", id);
        }

        public static void SetItem(FieldModel item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Field");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From Field Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                sql = "Select IsNull(Max(SN), 0) From Field Where ParentID = " + item.ParentID;
                tableObj["SN"] = (int)db.GetFirstValue(sql) + 1;

                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("ParentID");
                tableObj.Remove("SN");

                tableObj.Update(item.ID);
            }
        }

        public static string GetAddressFieldValue(FieldAddress item) {
            if (item == null)
                return string.Empty;

            if (item.Regions == null || item.Regions.Length == 0)
                return item.Address;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"Select ID, Name From WorldRegion Where ID In ({ string.Join(", ", item.Regions) })";
            IEnumerable<DataRow> datas = db.GetDataTable(sql).AsEnumerable();

            string address = string.Empty;
            foreach (int regionId in item.Regions)
                address += datas.Where(dr => (int)dr["ID"] == regionId).Select(dr => dr["Name"].ToString()).FirstOrDefault();

            return address + item.Address;
        }
    }
}
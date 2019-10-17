using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WorkLib;
using WorkV3.Common;

namespace WorkV3.Models
{
    public class FormDAO : BaseDAO<FormModel>
    {
        public static FormDAO Instance;

        static FormDAO()
        {
            Instance = new FormDAO();
        }

        public static FormModel GetItemFromSourceID(long sourceId) {
            string sql = "Select Top(1) * From Form Where SourceID = " + sourceId;
            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormModel>(sql).SingleOrDefault();
            }
        }

        public static FormModel GetItem(long id) {
            string sql = "Select * From Form Where ID = " + id;
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormModel>(sql).SingleOrDefault();
            }
        }

        public static void SetItem(FormModel item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Form");
            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From Form Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("SourceID");
                tableObj.Remove("IsRemove");
                
                tableObj.Update(item.ID);
            }
        }

        public static IEnumerable<FormAdmin> GetSystemAdmins(long siteId) {
            string sql = "Select ID MemberID, Name, Email, Img From Member Where Email <> ''";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormAdmin>(sql);
            }
        }

        public static IEnumerable<FormAdmin> GetItemAdmins(long formId) {
            string sql = $"SELECT FA.*, M.Name, M.Img FROM FormAdmin FA LEFT JOIN Member M ON FA.MemberID = M.ID WHERE FA.FormID = { formId } ORDER BY FA.Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormAdmin>(sql);
            }
        }

        public static void SetItemAdmins(long formId, IEnumerable<FormAdmin> admins) {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($"Delete FormAdmin Where FormID = { formId }");

            if(admins?.Count() > 0) {
                string fmt = $"Insert FormAdmin(ID, FormID, Email, MemberID, Sort) Values({{0}}, { formId }, N'{{1}}', {{2}}, {{3}})\n";
                int index = 0;
                foreach(FormAdmin item in admins) {
                    if (item.ID == 0)
                        item.ID = WorkLib.GetItem.NewSN();
                    sql.AppendFormat(fmt, item.ID, item.Email.Replace("'", "''"), item.MemberID == null ? "NULL" : item.MemberID.ToString(), ++index);
                }
            }

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql.ToString());
        }

        public static void SetItemFields(long formId, IEnumerable<FieldDesignItem> items) {
            StringBuilder sql = new StringBuilder();
            if(items?.Count() > 0) {
                sql.AppendLine($"DELETE Field WHERE ParentID = { formId } AND ID Not IN ({ string.Join(", ", items.Select(item => long.Parse(item.ID))) })");
                string fmt = "Update Field Set Enable = {0}, SN = {1} WHERE ID = {2}\n";
                int sort = 0;
                foreach(FieldDesignItem item in items) {
                    sql.AppendFormat(fmt, item.Enable ? 1 : 0, ++sort, long.Parse(item.ID));
                }
            } else {
                sql.AppendLine("DELETE Field WHERE ParentID = " + formId);
            }

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql.ToString());
        }
        
        /// <summary>
        /// 僅用於表單模組，活動中的報名不能用
        /// </summary>
        /// <param name="id">表單ID</param>
        /// <returns></returns>
        public static SitePage GetFormPage(long id) {
            string sql = "SELECT TOP(1) P.SiteID, S.SN SiteSN, P.No PageNo, P.SN PageSN, P.MenuID FROM Menus M JOIN Pages P ON M.ID = P.MenuID JOIN Sites S ON M.SiteID = S.ID " +
                $"WHERE M.ID = (Select SourceID From Form Where ID = { id })";
            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<SitePage>(sql).FirstOrDefault();
            }
        }

        public static void SaveTemplate(long id, string name) {
            if (string.IsNullOrWhiteSpace(name))
                return;

            FormModel item = GetItem(id);
            item.ID = WorkLib.GetItem.NewSN();
            item.SourceID = null;
            SetItem(item);

            string sql = $"Update Form Set TemplateName = N'{ name.Replace("'", "''") }' Where ID = { item.ID }";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);

            IEnumerable<FormAdmin> admins = GetItemAdmins(id);
            foreach(FormAdmin admin in admins) 
                admin.ID = 0;
            SetItemAdmins(item.ID, admins);

            IEnumerable<FieldModel> fields = FieldDAO.GetItems(id);
            foreach(FieldModel field in fields) {
                field.ID = WorkLib.GetItem.NewSN();
                field.ParentID = item.ID;
                FieldDAO.SetItem(field);
            }                
        }

        public static void Delete(long id) {
            string sql =
                $"Delete Form Where ID = { id }\n" +
                $"Delete FormAdmin Where FormID = { id }\n" +
                $"Delete FieldValue Where FormItemID IN (Select ID From FormItem Where FormID = { id })\n" +
                $"Delete FormItem Where FormID = { id }\n" +
                $"Delete Field Where ParentID = { id }\n";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static FormModel CreateFromTemplate(long sourceId, long formId, long templateId) {
            FormModel form = GetItem(templateId);
            if (form == null)
                return null;
            
            Delete(formId);
            form.ID = formId;
            form.SourceID = sourceId;
            SetItem(form);

            IEnumerable<FormAdmin> admins = GetItemAdmins(templateId);
            foreach (FormAdmin admin in admins)
                admin.ID = 0;
            SetItemAdmins(formId, admins);

            IEnumerable<FieldModel> fields = FieldDAO.GetItems(templateId);
            foreach (FieldModel field in fields) {
                field.ID = WorkLib.GetItem.NewSN();
                field.ParentID = formId;
                FieldDAO.SetItem(field);
            }

            return form;
        }

        public static IEnumerable<IDValue> GetTemplates(long siteId) {
            string sql = "Select ID, TemplateName Value From Form Where TemplateName <> '' Order By TemplateName";
            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<IDValue>(sql);
            }
        }

        public static FormSet getFormSet(long CardNo)
        {
            string sql = $"Select * From FormSet Where CardNo = {CardNo}";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<FormSet>(sql).SingleOrDefault();
            }
        }

        public static void SvaeFormSetting(long FormID, long CardNo)
        {
            string sql = $"Delete FormSet Where CardNo = {CardNo}";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);

            sql = $"Insert FormSet(CardNo, FormID) Values({CardNo}, {FormID})";
            db.ExecuteNonQuery(sql);
        }

        public static FormModel GetItemFromCardNo(long CardNo)
        {
            string sql = $@"SELECT * FROM FormSet AS fs 
                            LEFT JOIN Form AS f ON fs.formID = f.ID
                            WHERE CardNo = {CardNo}";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<FormModel>(sql).SingleOrDefault();
            }
        }
    }
}
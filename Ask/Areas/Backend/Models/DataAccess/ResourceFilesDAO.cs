using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Golbal;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ResourceFilesDAO
    {

        #region 查詢

        #region GetInfo
        public static ResourceFilesModels GetInfo(long SiteID, long SourceNo, byte SourceType, int AreaID, long Id, int Ver)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "Select * From ResourceFiles Where Id=@Id and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID";

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@ID", Id);
            para.Add("@SiteID", SiteID);
            para.Add("@SourceNo", SourceNo);
            para.Add("@SourceType", SourceType);
            para.Add("@Ver", Ver);
            para.Add("@AreaID", AreaID);
            DataTable dt = db.GetDataTable(Sql, para);


            ResourceFilesModels _TempRow = null;
            if (dt.Rows.Count > 0)
            {
                _TempRow = new ResourceFilesModels();
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }

        public static ResourceFilesModels GetItem(long id) {
            return CommonDA.GetItem<ResourceFilesModels>("ResourceFiles", id);
        }

        #endregion

        public static List<ResourceFilesModels> GetInfoAll(long SiteID, long SourceNo, long SourceType, int AreaID, int Ver)
        {

            string Sql = "Select * From ResourceFiles Where SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@SiteID", SiteID);
            para.Add("@SourceNo", SourceNo);
            para.Add("@SourceType", SourceType);
            para.Add("@Ver", Ver);
            para.Add("@AreaID", AreaID);
            DataTable dt = db.GetDataTable(Sql, para);

            List<ResourceFilesModels> nLists = new List<ResourceFilesModels>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++) {
                    ResourceFilesModels _TempRow = new ResourceFilesModels();
                    _TempRow = CreateData(dt.Rows[i]);
                    nLists.Add(_TempRow);
                }
            }
            return nLists;
        }

        public static IEnumerable<ResourceFilesModels> GetItems(long sourceNo, string code = null) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ResourceFiles Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceFilesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }
        #endregion

        #region 刪除      

        public static void DelAll(long SiteID, long SourceNo, long SourceType, int Ver, int AreaID)
        {
            //
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Delete ResourceFiles Where SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID";

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@SiteID", SiteID);
            para.Add("@SourceNo", SourceNo);
            para.Add("@SourceType", SourceType);
            para.Add("@Ver", Ver);
            para.Add("@AreaID", AreaID);

            db.ExecuteNonQuery(sql, para);
        }

        public static int DeleteItemsExcept(long sourceNo, string code, IEnumerable<long> exceptIds = null) {
            string sql = "Delete ResourceFiles Where {0}";

            List<string> where = new List<string>();
            where.Add("SourceNo = " + sourceNo);

            if (!string.IsNullOrWhiteSpace(code))
                where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

            if (exceptIds != null && exceptIds.Count() > 0)
                where.Add(string.Format("ID Not In ({0})", string.Join(", ", exceptIds)));

            sql = string.Format(sql, string.Join(" And ", where));

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Execute(sql);
            }
        }

        public static int DeleteItems(long sourceNo, string code) {
            return DeleteItemsExcept(sourceNo, code);
        }
        #endregion

        public static void SetItem(ResourceFilesModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ResourceFiles");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ResourceFiles Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["FileType"] = ResourceFileType.inFile;
                tableObj["FileMimeType"] = item.GetMimeType();
                tableObj["ClickType"] = 0;
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                string[] removeFields = { "ID", "SiteID", "SourceNo", "SourceType", "Ver", "AreaID", "FileType", "FileMimeType", "ClickType", "Creator", "CreateTime" };
                foreach (string f in removeFields)
                    tableObj.Remove(f);

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@ID", item.Id);
                keys.Add("@SiteID", item.SiteID);
                keys.Add("@SourceNo", item.SourceNo);
                keys.Add("@SourceType", item.SourceType);
                keys.Add("@Ver", item.Ver);
                keys.Add("@AreaID", item.AreaID);

                tableObj.Update(keys);
            }
        }

        #region  檔案選單
        /// <summary>
        /// 新增檔案選單
        /// </summary>
        /// <param name="data"></param>
        public static void Save_Menu(ResourceFilesModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string Sql =
                "IF EXISTS (SELECT 1 FROM ResourceFiles WHERE ID=@ID and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID)" +

                " UPDATE ResourceFiles SET FileType=@FileType, FileInfo=@FileInfo, FileSize=@FileSize, FileMimeType=@FileMimeType, Descriptions=@Descriptions, Detail=@Detail, ClickType=@ClickType, Modifier=@Modifier, ModifyTime=GetDate() WHERE ID=@ID and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID" +

                " ELSE" +

                " INSERT INTO ResourceFiles(ID,SiteID,SourceNo, SourceType,Ver,AreaID,FileType,FileInfo,FileSize,FileMimeType,Descriptions,Detail,ClickType,Creator,CreateTime)VALUES(@ID,@SiteID,@SourceNo,@SourceType,@Ver,@AreaID,@FileType,@FileInfo,@FileSize,@FileMimeType,@Descriptions,@Detail,@ClickType,@Creator,GetDate())";



            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@ID", data.Id);
            para.Add("@SiteID", data.SiteID);
            para.Add("@SourceNo", data.SourceNo);
            para.Add("@SourceType", data.SourceType);
            para.Add("@Ver", data.Ver);
            para.Add("@AreaID", data.AreaID);
            para.Add("@FileType", data.FileType);
            para.Add("@FileInfo", data.FileInfo);
            para.Add("@FileSize", data.FileSize);
            para.Add("@FileMimeType", data.FileMimeType);
            para.Add("@Descriptions", data.Descriptions);
            para.Add("@Detail", data.Detail);
            para.Add("@ClickType", data.ClickType);
            para.Add("@Creator", MemberDAO.SysCurrent.Id);
            para.Add("@Modifier", MemberDAO.SysCurrent.Id);
            db.ExecuteNonQuery(Sql, para);

        }


        #endregion

        
        #region CreateDataRow        
        private static ResourceFilesModels CreateData(DataRow dr)
        {

            ResourceFilesModels nData = new ResourceFilesModels
            {
                Id = (long)dr["id"],
                SiteID = (long)dr["SiteID"],             
                SourceNo = (long)dr["SourceNo"],
                SourceType = (byte)dr["SourceType"],
                Ver = (int)dr["Ver"],
                AreaID = (byte)dr["AreaID"],
                FileType = dr["FileType"].ToString().Trim(),
                FileInfo = dr["FileInfo"].ToString().Trim(),
                FileSize = dr["FileSize"] as long?,
                FileMimeType = dr["FileMimeType"].ToString().Trim(),
                Descriptions = dr["Descriptions"].ToString().Trim(),
                Detail = dr["Detail"].ToString().Trim(),
                ClickType = (int)dr["ClickType"],            
                Creator = (long)dr["Creator"],
                CreateTime = dr["CreateTime"] as DateTime?,
                Modifier = dr["Modifier"] as long?,
                ModifyTime = dr["ModifyTime"] as DateTime?             
            };

            return nData;
        }
        #endregion

        #region Jquery fileuploader Format

        public static string fileuploader(ResourceFilesModels newFile,string vPath) {

            if (newFile != null)
            {
                UpdFileInfo.fileuploaderFile Files = new UpdFileInfo.fileuploaderFile();
                Files.name = newFile.FileInfo;
                Files.type = newFile.FileMimeType.Replace(@"/", @"\/");
                Files.size = newFile.FileSize.ToString();
                Files.file = (vPath.Replace(@"/", @"\/") + newFile.FileInfo);            
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(Files).Replace(@"\\/", @"\/");
               
                return output;
                //ViewBag.FileExists = new HtmlString(output);
            }
            return "";
        }

        #endregion
    }
}
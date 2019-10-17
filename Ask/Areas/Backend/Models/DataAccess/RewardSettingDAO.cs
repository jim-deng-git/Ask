using Dapper;
using Newtonsoft.Json.Linq;
using SQLData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class RewardSettingDAO : BaseDAO<RewardSettingModel>
    {

        public static RewardSettingModel GetItem(long id)
        {
            return CommonDA.GetItem<RewardSettingModel>("RewardSetting", id);
        }
        public static RewardPlaceModel GetItemPlace(long id)
        {
            return CommonDA.GetItem<RewardPlaceModel>("RewardPlace", id);
        }
        public static List<RewardTextModel> GetItemText(long? SiteID)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> where = new List<string>();
                if (SiteID.HasValue)
                {
                    where.Add("[SiteID] = @SiteID");
                    param.Add("@SiteID", SiteID);
                }

             

                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [RewardText] ");
                sql.Append($"WHERE { string.Join(" AND ", where) } ");
                sql.Append($"ORDER BY  ID ");



                return cn.Query<RewardTextModel>(sql.ToString(), param).ToList();



            }
            
        }
        public static void SetItem(RewardSettingModel item)
        {
            item.Color = item.Color ?? string.Empty;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("RewardSetting");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From RewardSetting Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;
                tableObj.Update(item.ID);
            }
        }
        public static void SetItemPlace(RewardPlaceModel item)
        {
           

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("RewardPlace");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From RewardPlace Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }
        public static void SetItemText(RewardTextModel item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("RewardText");
            tableObj.GetDataFromObject(item);

            string sql = "Select top 1 * From RewardText Where ID = @ID and SiteID= @SiteID ";
            ParameterCollection a1 = new ParameterCollection();
            a1.Add("ID", item.ID);
            a1.Add("SiteID", item.SiteID);
            bool isNew = db.GetFirstValue(sql,a1) == null;
            if (isNew)
            {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                //tableObj.Remove("SiteID");
                //tableObj.Remove("Creator");
                //tableObj.Remove("CreateTime");

                //tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                //tableObj["ModifyTime"] = DateTime.Now;
                string a = "UPDATE RewardText  set Text =@Text ,Modifier=@Modifier,ModifyTime=@ModifyTime Where ID = @ID2 and SiteID= @SiteID2";

                SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
                paraList.Add("ID2", item.ID);
                paraList.Add("SiteID2", item.SiteID);
                paraList.Add("Text", item.Text);
                paraList.Add("Modifier", WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id);
                paraList.Add("ModifyTime", DateTime.Now);
                int exeCount = db.ExecuteNonQuery(a, paraList);

            }
        }
        public static RewardSettingAndPlaceModel Get(long? SiteID = 0, bool? IsIssue = null, SearchModel Search = null)
        {
            RewardSettingAndPlaceModel c = new RewardSettingAndPlaceModel();
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> where = new List<string>();
                if (SiteID.HasValue)
                {
                    where.Add("[SiteID] = @SiteID");
                    param.Add("@SiteID", SiteID);
                }

                if (IsIssue.HasValue)
                {
                    where.Add("[IsIssue] = @IsIssue");
                    param.Add("@IsIssue", IsIssue);

                }

                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [RewardSetting] ");
                sql.Append($"WHERE { string.Join(" AND ", where) } ");
                sql.Append($"ORDER BY Sort, ID ");

               
             
                c.RewardSettingModel =  cn.Query<RewardSettingModel>(sql.ToString(), param).ToList();
              
              

            }
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                List<string> where = new List<string>();
                if (SiteID.HasValue)
                {
                    where.Add("[SiteID] = @SiteID");
                    param.Add("@SiteID", SiteID);
                }

                if (IsIssue.HasValue)
                {
                    where.Add("[IsIssue] = @IsIssue");
                    param.Add("@IsIssue", IsIssue);

                }

              
                StringBuilder sql2 = new StringBuilder();
                sql2.Append($"SELECT * FROM [RewardPlace] ");
                sql2.Append($"WHERE { string.Join(" AND ", where) } ");
                sql2.Append($"ORDER BY Sort, ID ");
               
                c.RewardPlaceModel = cn.Query<RewardPlaceModel>(sql2.ToString(), param).ToList();
               

            }
            return c;
        }

        public static void Sort(long SiteID, IEnumerable<SortItem> items)
        {
            CommonDA.Sort("RewardSetting", items, "SiteID = " + SiteID);
        }

        public static void Delete(IEnumerable<long> IDs)
        {
            CommonDA.Delete("RewardSetting", IDs);
        }

        public static void ToggleIssue(long ID)
        {
            CommonDA.ToggleIssue("RewardSetting", ID);
        }
        public static void SortPlace(long SiteID, IEnumerable<SortItem> items)
        {
            CommonDA.Sort("RewardPlace", items, "SiteID = " + SiteID);
        }

        public static void DeletePlace(IEnumerable<long> IDs)
        {
            CommonDA.Delete("RewardPlace", IDs);
        }

        public static void ToggleIssuePlace(long ID)
        {
            CommonDA.ToggleIssue("RewardPlace", ID);
        }
    }
}
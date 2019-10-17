using Dapper;
using Newtonsoft.Json.Linq;
using SQLData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class RewardDAO : BaseDAO<RewardSettingModel>
    {
        public static RewardModel GetItem(long id)
        {
            return CommonDA.GetItem<RewardModel>("Reward", id);
        }
        public static IEnumerable<RewardModel> GetUnassociatedRewards(long SiteId, long signUpId)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT * FROM (");
                sql.Append(" SELECT * FROM Reward ");
                sql.Append($" WHERE SiteID = {SiteId} AND ID NOT IN ( SELECT DISTINCT RewardID FROM EventSignUp WHERE RewardID is not null ) ");
                sql.Append(" UNION ");
                sql.Append(" SELECT * FROM Reward ");
                sql.Append($" WHERE ID = ( SELECT RewardID FROM EventSignUp WHERE ID = {signUpId}) ");
                sql.Append(") temp ");
                sql.Append(" ORDER by ID ");
                return cn.Query<RewardModel>(sql.ToString());
            }
        }

        public static RewardModel GetItemByCardNo(long cardNo)//wei 20180912 SEO套件
        {
            string sql = "Select * From Reward Where CardNo = " + cardNo;
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<RewardModel>(sql).SingleOrDefault();
            }
        }

        public static RewardFieldModel GetField(long id)
        {
            return CommonDA.GetItem<RewardFieldModel>("RewardField", id);
        }
        public static List<RewardFieldModel> GetFields()
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT * FROM [RewardField] ");
                sql.Append($"ORDER by ID ");
                return cn.Query<RewardFieldModel>(sql.ToString()).ToList();
            }
        }
        //public static IEnumerable<FormAdmin> GetItemAdmins(long formId)
        //{
        //    string sql = $"SELECT FA.*, M.Name, M.Img FROM FormAdmin FA LEFT JOIN Member M ON FA.MemberID = M.ID WHERE FA.FormID = { formId } ORDER BY FA.Sort";
        //    using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
        //    {
        //        return conn.Query<FormAdmin>(sql);
        //    }
        //}
        public static List<RewardModel> Get(long? SiteID = 0, bool? IsIssue = null, JObject Search = null)
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

                if (IsIssue.HasValue)
                {
                    where.Add("[IsIssue] = @IsIssue");
                    param.Add("@IsIssue", IsIssue);

                }
                if (Search["key"].ToString()!="")
                {
                    where.Add("(([Title] like '%'+@key+'%')or([ContentText] like '%'+@key+'%')or(m.Name like '%'+@key+'%'))");
                    param.Add("@key", Search["key"].ToString());
                }
                if (Search["Types"].ToString() != "")
                {
                    //ViewBag.Search["Types"].ToString().Spilt(',').Contains(item.ID) 
                    List<string> s = new List<string>();

                    foreach (var item in Search["Types"].ToString().Split(','))
                    {
                        s.Add("(','+[RewardKind]+',' like '%,"+ item.Replace("'","''") + ",%')");
                    }
                    where.Add("("+ string.Join(" or ", s) + ")");
                }
                if (Search["IssueStart"].ToString() != "")
                {
                    where.Add("[IssueStart] >= @IssueStart");
                    param.Add("@IssueStart", Search["IssueStart"].ToString());
                }
                if (Search["IssueEnd"].ToString() != "")
                {
                    where.Add("[IssueEnd] <= @IssueEnd");
                    param.Add("@IssueEnd", Search["IssueEnd"].ToString());
                }
                if (Search["BeginDate"].ToString() != "")
                {
                    where.Add("[BeginDate] >= @BeginDate");
                    param.Add("@BeginDate", Search["BeginDate"].ToString());
                }
                if (Search["EndDate"].ToString() != "")
                {
                    where.Add("[EndDate] <= @EndDate");
                    param.Add("@EndDate", Search["EndDate"].ToString());
                }
                StringBuilder sql = new StringBuilder();
                sql.Append($"SELECT Reward.*,m.Name CreatorName FROM [Reward] left join Member m on m.id=Reward.Creator ");
                sql.Append($"WHERE { string.Join(" AND ", where) } ");
                sql.Append($"ORDER BY Sort ASC, ID DESC ");//wei 20180726 排序更改，原因修改排序因此造成問題
             
                return  cn.Query<RewardModel>(sql.ToString(), param).ToList();
            }
          
        }
        public static JObject GetChart(long SiteID , long RewardID , string BD, string ED)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" select dt,sum(CompleteCount)CompleteCount,sum(PersonCount)PersonCount,sum(PlaceCount)PlaceCount 
                                from( select convert(varchar,CreateTime,111) dt ,sum(1) CompleteCount,0 PersonCount,0 PlaceCount 
	                                  from RewardCollectInfo where SiteID= 1 and RewardID=@RewardID and IsComplete = 1
	                                  group by convert(varchar,CreateTime,111)
                                union all
                                select convert(varchar,RewardCollect.CreateTime,111) dt,0 CompleteCount,0 PersonCount,sum(1) PlaceCount 
                                from RewardCollect   
                                JOIN RewardCollectInfo on (RewardCollect.RewardCollectInfoID = RewardCollectInfo.ID AND RewardCollectInfo.RewardID = @RewardID)
                                group by convert(varchar,RewardCollect.CreateTime,111)
                                union all
                                select dt,0 CompleteCount,sum(co) PersonCount,0 PlaceCount 
                                from(select convert(varchar,RewardCollect.CreateTime,111) dt,1 co 
	                                from RewardCollect                                              
	                                JOIN RewardCollectInfo on (RewardCollect.RewardCollectInfoID = RewardCollectInfo.ID AND RewardCollectInfo.RewardID = @RewardID)
	                                group by convert(varchar,RewardCollect.CreateTime,111),KeyValue
	                                ) a group by dt)b 
                                WHERE dt BETWEEN @BD AND @ED
                                group by dt  ";

                cn.Open();
                SqlCommand cmd = new SqlCommand(sql, cn);

                cmd.Parameters.AddWithValue("@SiteID", SiteID);

                cmd.Parameters.AddWithValue("@RewardID", RewardID);

                cmd.Parameters.AddWithValue("@BD", BD);

                cmd.Parameters.AddWithValue("@ED", ED);

                SqlDataReader reader = cmd.ExecuteReader();
                JArray rs = new JArray();
                while (reader.Read())
                {
                    int fields = reader.FieldCount;
                    JObject rr = new JObject();
                    for (int i = 0; i < fields; i++)
                    {
                        Int64 testint64 = 0;
                        if (Int64.TryParse(reader[i].ToString(), out testint64))
                        {
                            Int32 testint32 = 0;
                            if (Int32.TryParse(reader[i].ToString(), out testint32))
                            {
                                rr.Add(new JProperty(reader.GetName(i), reader[i]));
                                continue;
                            }
                            else
                            {
                                rr.Add(new JProperty(reader.GetName(i), reader[i].ToString()));
                                continue;
                            }
                        }
                        rr.Add(new JProperty(reader.GetName(i), reader[i]));

                    }

                    rs.Add(rr);
                    // SQLString = (String)reader["SQLString"];

                }
                JObject rsp = new JObject(new JProperty("Data", rs));
                reader.Close();
                cn.Close();

                return rsp;
            }

        }
        public static void SetItem(RewardModel item)
        {          
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Reward");
            object objSort = db.GetFirstValue(" SELECT TOP 1 Sort FROM Reward ORDER BY Sort DESC ");
            int maxSort = (objSort == null || objSort == DBNull.Value)? 1: (int)objSort;
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From Reward Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj["Sort"] =  1;//wei 20180726 拿掉 maxSort +

                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj.Remove("Sort");

                tableObj["Modifier"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;
                tableObj.Update(item.ID);
            }
        }
        public static void Sort(long SiteID, IEnumerable<SortItem> items)
        {
            CommonDA.Sort("Reward", items, "SiteID = " + SiteID);
        }

        public static void Delete(IEnumerable<long> IDs)
        {
            CommonDA.Delete("Reward", IDs);
        }

        public static void ToggleIssue(long ID)
        {
            CommonDA.ToggleIssue("Reward", ID);
        }
        public static void ToggleIssueDate(long ID,DateTime? B=null, DateTime? E = null)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                string upda = "";

                SqlParameter para = new SqlParameter();
                if (B.HasValue)
                    upda += " IssueStart=@IssueStart, ";        
                else
                    upda += " IssueStart=null, ";

                if (E.HasValue)
                    upda += " IssueEnd=@IssueEnd ";       
                else
                    upda += " IssueEnd=null ";

                string sql = "update Reward set "+ upda + " where ID=@ID";
                cn.Open();
                SqlCommand cmd = new SqlCommand(sql, cn);
                if (B.HasValue)
                    cmd.Parameters.AddWithValue("@IssueStart", B);
                if (E.HasValue)
                    cmd.Parameters.AddWithValue("@IssueEnd", E);
                cmd.Parameters.AddWithValue("@ID", ID);
                
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public static long GetMenuID(long siteId)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"Select Top(1) ID From Menus Where SiteID = { siteId } And DataType = 'Reward'";
            long? menuId = db.GetFirstValue(sql) as long?;
            if (menuId.HasValue)
                return menuId.Value;

            WorkV3.Areas.Backend.Models.MenusModels menus = new Areas.Backend.Models.MenusModels
            {
                SiteID = siteId,
                Title = "集點",
                SN = "Reward",
                DataType = "Reward",
                ShowStatus = 2,
                Sort = 1,
                Creator = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id,
                CreateTime = DateTime.Now
            };
            WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.SaveInfo(menus);

            return (long)db.GetFirstValue(sql);
        }

        public static string GetDefaultTerms()
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT Text ");
                sql.Append(" FROM RewardText ");
                sql.Append(" WHERE ID = 'DefaultTerms' ");
                RewardTextModel text =  cn.Query<RewardTextModel>(sql.ToString()).SingleOrDefault();
                return text.Text;
            }
        }

        //wei20180808 複製功能抄徵件
        public static void Copy(long siteId, IEnumerable<long> ids)
        {
            if (ids == null || ids.Count() == 0)
                return;

            long menuId = RewardDAO.GetMenuID(siteId);
            MenusModels menu = DataAccess.MenusDAO.GetInfo(siteId, menuId);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            foreach (long id in ids)
            {
                SQLData.TableObject tableObj = db.GetTableObject("Reward", id);
                if (tableObj.Count == 0)
                    continue;

                long newId = WorkLib.GetItem.NewSN();
                tableObj["ID"] = newId;
                //tableObj["CardNo"] = Golbal.PubFunc.AddPage(siteId, menuId, menu.SN, "Reward", "Index", true);
                tableObj["Sort"] = 1;
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;
                tableObj.Remove("Modifier");
                tableObj.Remove("ModifyTime");

                tableObj.Insert();

               

              
            }
        }
    }
}
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ManagerDAO
    {
        public static bool HasSupremeAuthorityManager(long siteId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                WorkV3.Models.SitesModels site = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteId);
                string account = $"admin-{site.SN}";

                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = @" SELECT 1 FROM Member WHERE  LoginID = @LoginID AND IsSupremeAuthority = 1 ";
                param.Add("@LoginID", account);

                return conn.Query<MemberModels>(sql, param).Count() > 0;
            }
        }

        /// <summary>
        /// 產生該 site 的最高管理者
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns>1：新增完成；0：已存在最高管理員，不新增。</returns>
        public static int CreateSupremeAuthorityManagerForSite(long siteId)
        {
            WorkV3.Models.SitesModels site = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteId);
            string account = $"admin-{site.SN}";

            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                string sql = @" SELECT 1 FROM Member WHERE  LoginID = @LoginID AND IsSupremeAuthority = 1 ";
                param.Add("@LoginID", account);

                string password = $"{site.SN}{DateTime.Now.ToString("yyyy")}";

                param = new Dictionary<string, object>();
                sql = $@" INSERT INTO [Member]([ID],[LoginID],[Name],[Email],[MStatus],[Password],[GroupID] , IsSupremeAuthority, isSysOnly, [AddDate]) 
                               VALUES({ WorkLib.GetItem.NewSN()},@LoginID,@Name, @Email, @MStatus,@Password ,@GroupID, 1, 1, getdate()) ";
                param.Add("@LoginID", account);
                param.Add("@Name", "最高管理員");
                param.Add("@Email", "");
                param.Add("@MStatus", 0);
                param.Add("@Password", password);
                param.Add("@GroupID", 1);
                return conn.Query<int>(sql, param).SingleOrDefault();
            }
        }

        public static List<MemberModels> GetItems(int pageSize, int pageIndex, out int recordCount, MemberSearch search)
        {
            search.Groups = search.Groups ?? new long[0];
            search.MStatus = search.MStatus ?? new int[0];
            string groups = String.Join(",", search.Groups);
            string status = String.Join(",", search.MStatus);

            return GetItems(pageSize, pageIndex, out recordCount, GroupID: groups, MStatus: status, KW: search.Keyword);
        }

        public static List<MemberModels> GetAll(MemberSearch search)
        {
            search.Groups = search.Groups ?? new long[0];
            search.MStatus = search.MStatus ?? new int[0];
            string groups = String.Join(",", search.Groups);
            string status = String.Join(",", search.MStatus);
            int recordCount;

            return GetItems(int.MaxValue, 1, out recordCount, GroupID: groups, MStatus: status);
        }

        public static List<MemberModels> GetItems(int pageSize, int pageIndex, out int recordCount, string KW = "", string HospId = "", string GroupID = "", string MStatus = "")
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                List<MemberModels> items = new List<MemberModels>();

                string sql = " SELECT m.*,g.Name as GroupName FROM Member m LEFT JOIN [Group] g ON m.GroupID=g.ID WHERE 1=1 ";
                if (!string.IsNullOrEmpty(KW))
                {
                    sql += $" AND (m.LoginID like '%{KW.Replace("'", "''")}%' or m.Name like '%{KW.Replace("'", "''")}%' or m.Email like '%{KW.Replace("'", "''")}%' ) ";
                }

                if (!string.IsNullOrEmpty(GroupID))
                {
                    sql += " AND  m.GroupID IN ('" + GroupID.TrimEnd(',').Replace(",", "','") + "') ";
                }

                if (!string.IsNullOrEmpty(MStatus))
                {
                    sql += " AND  m.MStatus IN ('" + MStatus.TrimEnd(',').Replace(",", "','") + "') ";
                }

                return CommonDA.GetPageData<MemberModels>(sql, pageSize, pageIndex, out recordCount, orderClause: " ORDER BY IsSupremeAuthority DESC, Sort, ID DESC ").ToList();
            }
        }

        public static MemberModels GetItem(long id)
        {
            MemberModels m= CommonDA.GetItem<MemberModels>("[Member]", id);
            return m;
        }




        public static int DeleteManager(IEnumerable<long> ids)
        {
            //IEnumerable<long> supremeAuthorityUserIds = MemberDAO.GetSupremeAuthorityUsers().Select(x => x.Id);

            //ids = ids.Except(supremeAuthorityUserIds);

            if (ids?.Count() == 0)
                return 0;

            string sql =
                //"Delete [Member] Where ID In ({0})";
                "update Member set MStatus =1 where id IN  ({0})";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static void SetItem(MemberModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select 1 From [Member] Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
              
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {

                    sql = $"IF not EXISTS (SELECT 1 FROM Member WHERE LoginID = @LoginID)";
                    sql += $@" INSERT INTO [Member]([ID],[LoginID],[Name],[Email],[MStatus],[Password],[GroupID] ,isSysOnly, [AddDate], [Phone], [Mobile], [Img], 
                            ArriveDate, PersonalNote, Salary, SalaryPaymentType, LaborAllowance, LaborPension, JobAdditionPay, ExecutionFee, LaborInsurance, HealthInsurance, Welfare, LeaveFee) 
                            VALUES({ WorkLib.GetItem.NewSN()},@LoginID,@Name, @Email, @MStatus,@Password ,@GroupID, 1, getdate(), @Phone, @Mobile, @Img,
                            @ArriveDate, @PersonalNote, @Salary, @SalaryPaymentType, @LaborAllowance, @LaborPension, @JobAdditionPay, @ExecutionFee, @LaborInsurance, @HealthInsurance, @Welfare, @LeaveFee) ";
                    conn.Execute(sql, item);
                }
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    sql = @"UPDATE Member SET [Name]=@Name, [Email]=@Email,[MStatus]=@MStatus,[GroupID]=@GroupID, [Phone]=@Phone, [Mobile]=@Mobile, [Img]=@Img, 
                        [ArriveDate]=@ArriveDate, [PersonalNote]=@PersonalNote, [Salary]=@Salary, [SalaryPaymentType]=@SalaryPaymentType, [LaborAllowance]=@LaborAllowance, [LaborPension]=@LaborPension, [JobAdditionPay]=@JobAdditionPay, [ExecutionFee]=@ExecutionFee, [LaborInsurance]=@LaborInsurance, [HealthInsurance]=@HealthInsurance, [Welfare]=@Welfare, [LeaveFee]=@LeaveFee";
                    if (!string.IsNullOrEmpty(item.Password))
                    {
                        sql += ",[Password]=@Password";
                    }
                    sql += " WHERE ID=@ID ";
                    conn.Execute(sql, item);
                }
            }
        }
        public static void SetPersonalItem(MemberModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select 1 From [Member] Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
                return;
            else
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    sql = @"UPDATE Member SET [Name]=@Name, [Email]=@Email, [IsChangedPassword]=@IsChangedPassword";
                    if (!string.IsNullOrEmpty(item.Password))
                    {
                        sql += ",[Password]=@Password";
                    }
                    sql += " WHERE ID=@ID ";
                    conn.Execute(sql, item);
                }
            }
        }

        public static IEnumerable<MemberToCompanyModel> GetMemberToCompany(long memberId)
        {
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $@"Select M.CompanyID, M.DepartmentID,  
                            C.Name CompanyName, D.Name DepartmentName  
                            From MemberToCompany M 
                            Left Join Company C On M.CompanyID = C.ID 
                            Left Join Company D On M.DepartmentID = C.ID 
                            Where M.MemberID = {memberId}";

                return conn.Query<MemberToCompanyModel>(sql);
            }
        }

        public static void SetMemberToCompany(long memberId, IEnumerable<MemberToCompanyModel> companyItems)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.AppendLine($"Delete MemberToCompany Where MemberID = {memberId}");

            if (companyItems != null && companyItems.Count() > 0)
            {
                foreach (var item in companyItems)
                {
                    string fmt = $"Insert MemberToCompany Values ({memberId}, {item.CompanyID}, {item.DepartmentID})";
                    sql.AppendLine(fmt);
                }
            }

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                conn.Execute(sql.ToString());
            }
        }
    }
}
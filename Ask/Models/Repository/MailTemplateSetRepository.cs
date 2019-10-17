using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using Dapper;

namespace WorkV3.Models.Repository
{
    public class MailTemplateSetRepository : RepositoryBase<MailTemplateSetModels>
    {
        public MailTemplateSetRepository() {
            SetTableName("MailTemplateSet");
        }

        /// <summary>
        /// 設定寄件者為網站設定之名稱
        /// </summary>
        /// <param name="MailFromName"></param>
        public void SetMailFromName(string mailFromName)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = " update MailTemplateSet set MailFromName = @MailFromName ";

                conn.Query<MailTemplateSetModels>(sql,new {
                    MailFromName= mailFromName
                });
            }
        }

    }
}
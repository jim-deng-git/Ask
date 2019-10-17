using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ArticleIntroDAO
    {
        public static ArticleIntroModels GetItem(long cardNo)
        {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select Top(1) ID, Title, IssueDate, RemarkText, Icon, IsIssue From ArticleIntro Where CardNo = " + cardNo;
                return conn.Query<ArticleIntroModels>(sql).SingleOrDefault();
            }
        }        
    }
}
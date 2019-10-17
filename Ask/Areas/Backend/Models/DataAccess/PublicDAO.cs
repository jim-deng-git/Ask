using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class PublicDAO
    {
        /// <summary>
        /// 取縣市資料
        /// </summary>
        /// <returns></returns>
        public List<TwZipCodeModel> GetCityList()
        {
            string sql = "select * from TwZipCode where parentID=0;";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<TwZipCodeModel>(sql).ToList();
            }
        }

        /// <summary>
        /// 取行政區資料
        /// </summary>
        /// <param name="ZipCode"></param>
        /// <returns></returns>
        public List<TwZipCodeModel> GetBoroughList(int ZipCode)
        {
            string sql = string.Format("select * from TwZipCode where parentID={0};", ZipCode);
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<TwZipCodeModel>(sql).ToList();
            }
        }

        /// <summary>
        /// 取縣市或行政區名稱
        /// </summary>
        /// <param name="ZipCode"></param>
        /// <returns></returns>
        public string GetCityName(int ZipCode)
        {
            string sql = string.Format("select * from TwZipCode where ZipCode={0};", ZipCode);
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                return conn.Query<TwZipCodeModel>(sql).FirstOrDefault().Title;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WorkLib;

namespace WorkV3.Controllers
{
    public class WorldRegionController : Controller
    {
        public string Import()
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            TwHandler(db, sql);
            
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(Server.MapPath("~/LocListTW.xml"));
            
            System.Xml.XmlNodeList countryNodes = doc.SelectNodes("/Location/CountryRegion");
            int countrySort = 1;
            foreach(System.Xml.XmlNode countryNode in countryNodes) {
                if (countryNode?.Attributes?["Name"] == null)
                    continue;

                sql.Append(GetInsertSql(null, 1, countryNode.Attributes["Name"].Value, ++countrySort));                
                int countryId = id;

                int stateSort = 1;
                foreach(System.Xml.XmlNode stateNode in countryNode.ChildNodes) {
                    if (stateNode?.Attributes?["Name"] == null)
                        continue;

                    sql.Append(GetInsertSql(countryId, 2, stateNode.Attributes["Name"].Value, ++stateSort));
                    int stateId = id;

                    int citySort = 1;
                    foreach (System.Xml.XmlNode cityNode in stateNode.ChildNodes) {
                        if (cityNode?.Attributes?["Name"] == null)
                            continue;

                        sql.Append(GetInsertSql(stateId, 3, cityNode.Attributes["Name"].Value, ++citySort));
                        int cityId = id;

                        int regionSort = 1;
                        foreach (System.Xml.XmlNode regionNode in cityNode.ChildNodes) {
                            if(regionNode?.Attributes?["Name"] != null)
                                sql.Append(GetInsertSql(cityId, 4, regionNode.Attributes["Name"].Value, ++regionSort));
                        }
                    }
                }
            }

            return sql.ToString();
        }

        private void TwHandler(SQLData.Database db, StringBuilder sql) {
            sql.Append(GetInsertSql(null, 1, "台灣", 1));
            int twId = id;

            string query = "Select Title, ZipCode From TwZipCode Where ParentId = 0";
            DataTable datas = db.GetDataTable(query);
            int citySort = 0;
            foreach(DataRow dr in datas.Rows) {
                sql.Append(GetInsertSql(twId, 3, dr["Title"].ToString().Trim(), ++citySort));
                int cityId = id;
                query = "Select Title, ZipCode From TwZipCode Where ParentID = " + dr["ZipCode"].ToString();
                DataTable districts = db.GetDataTable(query);

                int countySort = 0;
                foreach (DataRow districtDr in districts.Rows)
                    sql.Append(GetInsertSql(cityId, 4, districtDr["Title"].ToString().Trim(), ++countySort));
            }
        }

        private int id = 0;
        private string GetInsertSql(int? parentId, int level, string name, int sort) {
            string parentIdStr = parentId == null ? "NULL" : parentId.ToString();
            return $"Insert WorldRegion(ID, ParentID, Levels, Name, Sort) Values({ ++id }, { parentIdStr }, { level }, N'{ name.Replace("'", "''") }', { sort })<br />";
        }
    }
}
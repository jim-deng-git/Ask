using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class CardsTypeDAO
    {
        public static List<CardsTypeModels> GetData()
        {
            string Sql = "Select * from CardsType";
        
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            List<CardsTypeModels> nLists = new List<CardsTypeModels>();
            DataTable dt = db.GetDataTable(Sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CardsTypeModels _TempRow = CreateData(dt.Rows[i]);
                    nLists.Add(_TempRow);
                }
            }
            return nLists;

        }


        #region CreateDataRow


        private static CardsTypeModels CreateData(DataRow dr)
        {

            CardsTypeModels nData = new CardsTypeModels
            {
                Code = dr["Code"].ToString(),
                Title = dr["Title"].ToString().Trim(),
                Icon = dr["Icon"].ToString().Trim(),
                Sort = (int)dr["Sort"],
                isOpenCreate = (bool)dr["isOpenCreate"],
                isIndexCards = (bool)dr["isIndexCards"],
                isNeedSN = (bool)dr["isNeedSN"],
                Types = (int)dr["Types"],
                URLAction = dr["URLAction"].ToString(),
                EditURLAction = dr["EditURLAction"].ToString(),
                iFrameH = (int)dr["iFrameH"]
            };

            return nData;
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using Dapper;
using System.Data.SqlClient;

namespace WorkV3.Models.DataAccess
{
    public class WorldRegionDAO
    {
        public static IEnumerable<WorldRegionModels> GetRegionsByParentId(int? parentId = null)
        {
            string sql = "Select ID, Levels, Name From WorldRegion Where ParentID " + (parentId == null ? " Is Null" : " = " + parentId) + " Order By Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<WorldRegionModels>(sql);
            }
        }

        public static IEnumerable<WorldRegionModels> GetRegions(IEnumerable<int> ids)
        {
            if (ids?.Count() == 0)
                return null;

            string sql = $"Select ID, Levels, Name From WorldRegion Where ID IN ({ string.Join(", ", ids) }) Order By Levels, Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                return conn.Query<WorldRegionModels>(sql);
            }
        }

        public static WorldRegionModels GetRegionByName(string name) {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            string sql = $"Select Top(1) ID, Levels, Name From WorldRegion Where Name = N'{ name.Replace("'", "''") }' Order By Levels, Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<WorldRegionModels>(sql).SingleOrDefault();
            }
        }

        public static int[] GetRegionHierarchy(int id) {
            List<int> hierarchy = new List<int>();
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            int? parentId = id;
            while(parentId != null) {
                hierarchy.Insert(0, (int)parentId);
                parentId = db.GetFirstValue("Select ParentID From WorldRegion Where ID = " + (int)parentId) as int?;
            }

            return hierarchy.ToArray();
        }

        public static IEnumerable<WorldRegionModels> GetRegions(int level, int? parentId)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                var commandText = $"SELECT * FROM [WorldRegion] WHERE [Levels] = @Level AND [ParentID] {(parentId.HasValue ? "= @ParentID" : "IS NULL")} ORDER BY [Sort];";
                var param = new Dictionary<string, object>();
                param.Add("@Level", level);
                param.Add("@ParentID", parentId);

                return connection.Query<WorldRegionModels>(commandText, param);
            }
        }

        public static IEnumerable<WorldRegionModels> GetRegionsByLevel(int level)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                var commandText = $"SELECT * FROM [WorldRegion] WHERE [Levels] = @Level ORDER BY [Sort];";
                var param = new Dictionary<string, object>();
                param.Add("@Level", level);

                return connection.Query<WorldRegionModels>(commandText, param);
            }
        }

        public static WorldRegionModels GetByID(int ID)
        {
            using (var connection = new SqlConnection(WebInfo.Conn))
            {
                var commandText = $"SELECT * FROM [WorldRegion] WHERE [ID] = @ID;";
                var param = new Dictionary<string, object>();
                param.Add("@ID", ID);

                return connection.Query<WorldRegionModels>(commandText, param).FirstOrDefault();
            }
        }
    }
}
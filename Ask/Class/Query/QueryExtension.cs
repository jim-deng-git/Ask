using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3
{
    /// <summary>
    /// Query 擴充物件
    /// </summary>
    public static class QueryExtension
    {
        public static string CreateCommand(this Query query, out Dictionary<string, object> param)
        {
            StringBuilder command = new StringBuilder();
            param = new Dictionary<string, object>();

            #region 篩選 WHERE

            List<string> where = new List<string>();

            foreach (var item in query.Where)
            {
                var id = GetID();
                where.Add($"[{item.Field}] {item.ComparisonOperator} @{item.Field}{id}");
                param.Add($"@{item.Field}{id}", item.Obj);
            }

            if (where.Count > 0)
            {
                command.Append($" WHERE {string.Join(" AND ", where)} ");
            }

            #endregion

            #region 排序 ORDER BY

            List<string> orderBy = new List<string>();

            foreach (var item in query.OrderBy)
            {
                orderBy.Add($"[{item.Field}] {item.OrderByOperator}");
            }

            if (orderBy.Count > 0)
            {
                command.Append($" ORDER BY {string.Join(",", orderBy)} ");

                #region 筆數 Limit Index

                if (query.Limit.HasValue)
                {
                    var index = 0;

                    if (query.Index.HasValue)
                    {
                        if (query.Index.Value >= 0)
                        {
                            index = query.Index.Value;
                        }
                    }

                    command.Append($" OFFSET {query.Limit.Value * index} ROWS FETCH NEXT {query.Limit.Value} ROWS ONLY ");
                }

                #endregion
            }

            #endregion

            return command.ToString();
        }

        #region 私有方法

        private static int GetID()
        {
            Random Counter = new Random(Guid.NewGuid().GetHashCode());
            return Counter.Next();
        }

        #endregion
    }
}

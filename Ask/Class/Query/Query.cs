using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkV3
{
    public class Query
    {
        public List<QWhere> Where { get; set; } = new List<QWhere>();

        public List<QOrderBy> OrderBy { get; set; } = new List<QOrderBy>();

        public int? Index { get; set; } = null;

        public int? Limit { get; set; } = null;

        public void Clear()
        {
            Where = new List<QWhere>();
            OrderBy = new List<QOrderBy>();
            Index = null;
            Limit = null;
        }
    }

    public class QWhere
    {
        public string Field { get; set; }

        public string ComparisonOperator { get; set; }

        public object Obj { get; set; }

        public QWhere(string field, string comparisonOperator, object obj)
        {
            Field = field;
            ComparisonOperator = comparisonOperator;
            Obj = obj;
        }
    }

    public class QOrderBy
    {
        public string Field { get; set; }

        public string OrderByOperator { get; set; }

        public QOrderBy(string field, string orderByOperator)
        {
            Field = field;
            OrderByOperator = orderByOperator;
        }
    }
}

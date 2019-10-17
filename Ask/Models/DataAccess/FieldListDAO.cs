using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WorkV3.Models
{
    public class FieldListDAO : BaseDAO<FieldListModel>
    {
        public static FieldListDAO Instance;

        static FieldListDAO()
        {
            Instance = new FieldListDAO();
        }
    }
}
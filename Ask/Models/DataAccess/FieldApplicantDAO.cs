using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WorkV3.Models
{
    public class FieldApplicantDAO : BaseDAO<FieldApplicantModel>
    {
        public static FieldApplicantDAO Instance;

        static FieldApplicantDAO()
        {
            Instance = new FieldApplicantDAO();
        }
    }
}
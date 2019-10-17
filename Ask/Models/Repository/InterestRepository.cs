using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace WorkV3.Models.Repository
{
    public class InterestRepository : RepositoryBase<InterestModel>, Interface.IInterestRepository
    {
        public InterestRepository()
        {
            SetTableName("Interests");
        }

        public IEnumerable<MemberShipModels> GetMemberShips(long interestId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT b.* 
                                FROM MemberShipInterest a  
                                JOIN MemberShip b ON(a.MemberShipID = b.ID)
                                WHERE a.InterestID = @InterestID ";

                return conn.Query<MemberShipModels>(sql, new { InterestID = interestId });
            }
        }
    }
}
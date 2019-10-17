using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace WorkV3.Models.Repository
{
    public class MemberShipRepository : RepositoryBase<MemberShipModels>, Interface.IMemberShipRepository<MemberShipModels>
    {
        public MemberShipRepository()
        {
            SetTableName("MemberShip");
        }

        public string GetFullAddress(string regions, string address)
        {
            if (string.IsNullOrWhiteSpace(regions))
                return address;

            IEnumerable<int> regionIds = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<int>>(regions);
            if (regionIds.Count() == 0)
                return address;

            IEnumerable<WorldRegionModels> regionsItem = DataAccess.WorldRegionDAO.GetRegions(regionIds);
            return string.Join(string.Empty, regionsItem.Select(r => r.Name)) + address;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class ZoneRepository : RepositoryBase<ZonesModels>, Interface.IZoneRepository<ZonesModels>
    {
        public ZoneRepository()
        {
            SetTableName("Zones");
        }
    }
}
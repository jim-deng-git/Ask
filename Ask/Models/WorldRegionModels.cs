using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class WorldRegionModels
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public byte Levels { get; set; }
        public string Name { get; set; } 

        public IEnumerable<WorldRegionModels> GetSubRegions() {
            return DataAccess.WorldRegionDAO.GetRegionsByParentId(ID);
        }
    }
}
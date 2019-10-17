using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class AddressAnswerModel
    {
        public int? Level1 { get; set; }

        public int? Level2 { get; set; }

        public int? Level3 { get; set; }

        public int? Level4 { get; set; }

        public string Address { get; set; }

        public string ToAddressString()
        {
            var address = "";

            if (Level1.HasValue)
            {
                var region = WorldRegionDAO.GetByID(Level1.Value);
                if (region != null)
                {
                    address += region.Name;
                }
            }

            if (Level2.HasValue)
            {
                var region = WorldRegionDAO.GetByID(Level2.Value);
                if (region != null)
                {
                    address += region.Name;
                }
            }

            if (Level3.HasValue)
            {
                var region = WorldRegionDAO.GetByID(Level3.Value);
                if (region != null)
                {
                    address += region.Name;
                }
            }

            if (Level4.HasValue)
            {
                var region = WorldRegionDAO.GetByID(Level4.Value);
                if (region != null)
                {
                    address += region.Name;
                }
            }

            address += Address;

            return address;
        }
    }
}
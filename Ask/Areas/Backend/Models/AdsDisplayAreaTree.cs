using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 廣告顯示位置的選單樹
    /// reference db.Menus
    /// </summary>
    public class AdsDisplayAreaTree
    {
        public long ID { get; set; } //Menus.ID
        public long SiteID { get; set; } //Menus.SiteID
        public long ParentID { get; set; }
        public long AreaID { get; set; }
        public string Title { get; set; }
        public string SN { get; set; }
        public string DataType { get; set; }
        public string DataTypeValue { get; set; }
        public int Sort { get; set; }
        public int level { get; set; }
        public long? AreaSetID { get; set; } //AdsDisplayAreaSet.ID

        public IEnumerable<AdsDisplayAreaSetModel> ChildDataList
        {
            get
            {
                if (AreaSetID != null)
                    return WorkV3.Areas.Backend.Models.DataAccess.AdvertisementDAO.GetAreaSetChildByAreaSetID((long)AreaSetID);
                else
                    return new List<AdsDisplayAreaSetModel>();

            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class SitesModels
    {
        public long Id { get; set; }
        public string SN { get; set; }
        public string Lang { get; set; }
        public string Domin { get; set; }
        public string Title { get; set; }
        public string Descriptions { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string LogoMobile { get; set; }
        public string Logo { get; set; }
        public string Logoshrink { get; set; }
        public string ICO { get; set; }
        public string HeaderCont { get; set; }
        public string FooterCont { get; set; }
        public string DefaultImage { get; set; }

        public int ClickType { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool Issue { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public bool HeaderCustomized { get; set; } = false;
        public bool FooterCustomized { get; set; } = false;
        public long? ParentID { get; set; }
        public bool IsLang { get; set; } = false;
        public bool IsSearchEnabled { get; set; } = false;
        public bool IsPathBread { get; set; } = false;
        public string Favicon { get; set; }

        #region Google GA
        public string GAInfo { get; set; }
        public List<GAInfoCont> GAInfos { get; set; }
        public class GAInfoCont
        {
            public string GAKey { get; set; }
            public string DominName { get; set; }
        }
        public int CreateSiteManager()
        {
            if(!WorkV3.Areas.Backend.Models.DataAccess.ManagerDAO.HasSupremeAuthorityManager(Id))
                return WorkV3.Areas.Backend.Models.DataAccess.ManagerDAO.CreateSupremeAuthorityManagerForSite(Id);

            return 0;
        }
        #endregion

        #region Google GTM
        public string GTMKey { get; set; }
        #endregion

        public string socialSetting { get; set; }
        public List<socialSettingCont> socialSettings { get; set; }
        public class socialSettingCont
        {
            public string icon { get; set; }
            public string URL { get; set; }
            public int IsOpenNew { get; set; }

        }


    }
}
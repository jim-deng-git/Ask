using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class SiteMailSettingRepository : RepositoryBase<SiteMailSettingModel>, Interface.ISiteMailSettingRepository<SiteMailSettingModel>
    {
        public SiteMailSettingRepository()
        {
            SetTableName("SiteMailSetting");
        }
    }
}
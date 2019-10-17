using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class StorePlanContractRespository : RepositoryBase<StorePlanContractModel>, Interface.IStorePlanContractRespository<StorePlanContractModel>
    {
        public StorePlanContractRespository()
        {
            SetTableName("StorePlanContract");
        }
    }
}
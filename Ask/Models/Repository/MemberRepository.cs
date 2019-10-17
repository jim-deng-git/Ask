using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Repository
{
    public class MemberRepository:RepositoryBase<WorkV3.Areas.Backend.Models.MemberModels>
    {
        public MemberRepository()
        {
            SetTableName("Member");
        }

    }
}
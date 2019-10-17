using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class PermissionModels
    {
        public long Id { get; set; }
        public long MenuId { get; set; }
        public long GroupId { get; set; }
    }
}
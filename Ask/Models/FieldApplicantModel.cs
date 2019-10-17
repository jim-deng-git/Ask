using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    [Table(Name = "FieldApplicant")]
    public class FieldApplicantModel
    {
        public long ParentID { get; set; }

        public long FieldID { get; set; }

        public int Sort { get; set; }
    }
}
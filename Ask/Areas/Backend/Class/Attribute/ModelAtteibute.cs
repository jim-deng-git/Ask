using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend
{
    public class ModelAtteibute : Attribute
    {
        public string TableName { get; set; }
    }
}
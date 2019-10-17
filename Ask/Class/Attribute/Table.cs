using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3
{
    public class Table : Attribute
    {
        public string Name { get; set; }
    }
}
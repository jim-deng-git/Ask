using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MenuStructure
    {
        public StructureType Type { get; set; }
        public string ParentID { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
    }
    public enum StructureType
    {
        Page,
        Menu
    }
}
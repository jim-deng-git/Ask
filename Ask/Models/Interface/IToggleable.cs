using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Interface
{
    public interface IToggleable
    {
        void Toggle(long id, string columnName = "IsIssue", string specificColumn = "ID");
    }
}
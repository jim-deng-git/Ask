using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    [Table(Name = "FieldList")]
    public class FieldListModel
    {
        /// <summary>
        /// 欄位識別碼
        /// </summary>
        [PKey]
        public long ID { get; set; }

        /// <summary>
        /// 父層識別碼
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int SN { get; set; }
    }
}
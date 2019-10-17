using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    [Table(Name = "FieldAnswer")]
    public class FieldAnswerModel
    {
        /// <summary>
        /// 答案識別碼
        /// </summary>
        [PKey]
        public long ID { get; set; }

        /// <summary>
        /// 欄位識別碼
        /// </summary>
        [PKey]
        public long FieldID { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }
    }
}
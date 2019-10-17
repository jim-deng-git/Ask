using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    [Table(Name = "Form")]
    public class FormModel
    {
        /// <summary>
        /// 表單識別碼
        /// </summary>
        [PKey]
        public long ID { get; set; }

        /// <summary>
        /// 表單標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 表單說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 表單圖片
        /// </summary>
        public string Image { get; set; }
        public long? SiteID { get; set; }
        public long? SourceID { get; set; }
        public bool ShowDescription { get; set; }
        public string Statement { get; set; }
        public bool ForceStatement { get; set; }
        public bool HasCaptcha { get; set; }
        public bool NotifyAdmin { get; set; }
        public string TemplateName { get; set; }
        public string Match { get; set; }
        public string VideoType { get; set; }
        public string Video { get; set; }
        public string VideoPhoto { get; set; }
        public string VideoCustomPhoto { get; set; }
        public bool IsVideoPhotoCustom { get; set; }
        public string VideoID { get; set; }
        public bool ShowResult { get; set; }

        public IEnumerable<FormAdmin> GetAdmins() {
            return FormDAO.GetItemAdmins(ID);
        }

        public void SetAdmins(IEnumerable<FormAdmin> admins) {
            FormDAO.SetItemAdmins(ID, admins);
        }

        public IEnumerable<FieldModel> GetFields() {
            return FieldDAO.GetItems(ID);
        }

        public void SetFields(IEnumerable<FieldDesignItem> fields) {
            FormDAO.SetItemFields(ID, fields);
        }
    }

    public class FormAdmin {
        public long ID { get; set; }
        public long FormID { get; set; }
        public string Email { get; set; }
        public long? MemberID { get; set; }
        public string Name { get; set; } 
        public string Img { get; set; }
    }
}
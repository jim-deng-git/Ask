using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;

namespace WorkV3.Models 
{
    public class FormItem {
        public long ID { get; set; }
        public long FormID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string IDCard { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public int? SN { get; set; }
        public bool IsTemp { get; set; } = true;
        public bool IsBack { get; set; }
        public byte CheckStatus { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Remark { get; set; }
        public DateTime? CheckInDate { get; set; }
        public bool IsProcess { get; set; }
        public string ProcessRemark { get; set; }
        public DateTime? ProcessTime { get; set; }

        public IEnumerable<FieldValue> GetValues() {
            return FieldValueDAO.GetItems(ID);
        }

        public IEnumerable<string> GetFlags(long siteId) {
            return DataAccess.UserFlagDAO.GetFlags(siteId, Email, Mobile, Phone, IDCard);
        }

        public void Complete(Common.FormCheckStatus checkStatus) {
            FormItemDAO.Complete(ID, checkStatus);
        }
    }
    
    public class FieldValue {
        public long FormItemID { get; set; }
        public long FieldID { get; set; }
        public string Value { get; set; }
    }

    public class FormItemSearch {
        public long FormID { get; set; }
        public string Key { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public Common.FormCheckStatus[] CheckStatusList { get; set; }        
        public int[] FillModes { get; set; }
        public int[] RegisterStatus { get; set; }
        public string Order { get; set; }
    }

    public class FormFieldValues {
        public IEnumerable<FieldModel> FieldList { get; set; }
        public IEnumerable<FieldValue> ValueList { get; set; }
        public string UploadUrl { get; set; }
    }
}
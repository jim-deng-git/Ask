using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models {
    public class FormMailModel {
        public long ID { get; set; }
        public long FormID { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string Sender { get; set; }
        public string SenderEmail { get; set; }
        public string Files { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;

        public IEnumerable<FormMailLogModel> GetSendLogs() {
            return DataAccess.FormMailDAO.GetMailLogs(ID);
        }
    }

    public class FormMailLogModel {
        public long MailID { get; set; }
        public long FormItemID { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
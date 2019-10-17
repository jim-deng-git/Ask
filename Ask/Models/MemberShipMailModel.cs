using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models {
    public class MemberShipMailModel
    {
        public long ID { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string Sender { get; set; }
        public string SenderEmail { get; set; }
        public string Files { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;

        public IEnumerable<MemberShipMailLogModel> GetSendLogs() {
            return DataAccess.MemberShipMailDAO.GetMailLogs(ID);
        }
    }

    public class MemberShipMailLogModel
    {
        public long MailID { get; set; }
        public long MemberShipID { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
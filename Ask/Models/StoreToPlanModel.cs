using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class StoreToPlanModel
    {
        public long ID { get; set; }
        public long StoreID { get; set; }
        public long PlanID { get; set; }
        public DateTime? ContractDateStart { get; set; }
        public DateTime? ContractDateEnd { get; set; }
        public string ContractTitle { get; set; }
        public int ContractLang { get; set; }
        public string ContractIntro { get; set; }
        public bool IsContractSave { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
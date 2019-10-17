using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.Models
{
    public class RewardModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        public string RewardKind { get; set; }
        public IEnumerable<string> RewardKindArray { get; set; }
        public string ContentText { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RewardPlace { get; set; }
        public bool ShowDescription { get; set; } = true;//wei 20180820 加眼睛
        public IEnumerable<string> RewardPlaceArray { get; set; }
        public bool LimitLogin { get; set; }
        public long CardNo { get; set; }//wei 20180912 SEO 
        public string Limit { get; set; }
        public int? CompletePlace { get; set; }
        public string NeedCol { get; set; }
        public IEnumerable<NeedColModel> NeedColArray { get; set; }
        public bool ShowTerms { get; set; }
        public string TermsText { get; set; }
        public string RewardingText { get; set; }
        public string RewardedText { get; set; }
        public int? Sort { get; set; }
        public long? Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool IsIssue { get; set; }
        public DateTime? IssueStart { get; set; }
        public DateTime? IssueEnd { get; set; }
        public string SendMail { get; set; }
        public string Admins { get; set; }
        public bool ForcedEnd { get; set; }
        public IEnumerable<FormAdmin> GetAdmins()
        {
            return FormDAO.GetItemAdmins(ID);
        }
        public string Img { get; set; }
        public string VideoImg { get; set; }
        public bool VideoImgIsCustom { get; set; }
        public string VideoID { get; set; }
        public string videoType { get; set; }
        public string videoDefaultImg { get; set; }
        public string CreatorName { get; set; }
        public string UseType { get; set; }
        public bool IsDrawn { get; set; }
        public string Fields { get; set; }

        public List<RewardField> GetFields()
        {
            if (string.IsNullOrWhiteSpace(Fields))
                return new List<RewardField>();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<RewardField>>(Fields);
        }

    }

    public class RewardField
    {
        public string Name { get; set; }
        public bool IsShow { get; set; }
        public string Limit { get; set; }
        public bool Required { get; set; }
    }

    public class NeedColModel
    {
        public long ID { get; set; }
        public bool Chk { get; set; }
        public bool Needed { get; set; }
        public int Sort { get; set; }
    }

    }
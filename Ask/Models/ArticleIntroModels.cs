using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.DataAccess;

namespace WorkV3.Models
{
    public class ArticleIntroModels
    {
        public long ID { get; set; }        
        public string Title { get; set; }
        public DateTime? IssueDate { get; set; }
        public string RemarkText { get; set; } = string.Empty;
        public string Icon { get; set; }
        public bool IsIssue { get; set; }
        
        public IEnumerable<ParagraphModels> GetParagraphs() {
            return ParagraphDAO.GetItems(ID);
        }
    }
}
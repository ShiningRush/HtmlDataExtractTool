using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.ExtractStrategy.SearchModels
{
    public class WechatSearchInfo
    {
        public string Account { get; set; }
        public string StartFile { get; set; }
        public List<string> FriendsFiles { get; set; }
        public List<string> GroupsFiles { get; set; }

        public WechatSearchInfo()
        {
            FriendsFiles = new List<string>();
            GroupsFiles = new List<string>();
        }
    }
}

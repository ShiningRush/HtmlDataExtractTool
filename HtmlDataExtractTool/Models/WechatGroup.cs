using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.Models
{
    public class WechatGroup
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public List<WechatAccountInfo> Members { get; set; }
    }
}

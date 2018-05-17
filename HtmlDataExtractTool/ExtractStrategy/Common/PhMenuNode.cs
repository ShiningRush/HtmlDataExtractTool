using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.ExtractStrategy.Common
{
    public class PhMenuNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public List<PhMenuNode> Childs { get; set; }
    }
}

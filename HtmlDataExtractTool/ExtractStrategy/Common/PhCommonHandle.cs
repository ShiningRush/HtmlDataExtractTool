using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlDataExtractTool.ExtractStrategy.Common
{
    public class PhCommonHandle : ICommonHandle
    {
        private List<PhMenuNode> lastResult;
        public string GetMenuFile(string srcDir)
        {
            var menuRegex = new Regex(@"PingHangReport_.+\.html$");
            var allPages = Directory.GetFiles(Path.Combine(srcDir, "Content"));
            return allPages.First(p=>menuRegex.Match(p).Success);
        }

        public List<PhMenuNode> CreateMenuNodes(string menuFileContent, bool getLastResult)
        {
            if (getLastResult && lastResult != null)
                return lastResult;

            lastResult = GetNodeChilds(menuFileContent, new PhMenuNode() { Id = "-1" }).Childs;
            return lastResult;
        }

        private PhMenuNode GetNodeChilds(string menuFileContent, PhMenuNode parentNode)
        {
            var matchResults = Regex.Matches(menuFileContent, $@"{{id:(\d*), pId: {parentNode.Id}, name:'(.*)', url:'(.*)',.*icon:'(.*)'");
            var phMenuNodes = new List<PhMenuNode>();
            foreach (Match aMatchResult in matchResults)
            {
                var newPhMenuNode = GetTheNodeWithMatch(aMatchResult);
                GetNodeChilds(menuFileContent, newPhMenuNode);

                phMenuNodes.Add(newPhMenuNode);
            }
            parentNode.Childs = phMenuNodes;

            return parentNode;
        }

        private PhMenuNode GetTheNodeWithMatch(Match aMatch)
        {
            var newPhMenuNode = new PhMenuNode();
            newPhMenuNode.Id = aMatch.Groups[1].Value;
            newPhMenuNode.Name = Encoding.UTF8.GetString(Convert.FromBase64String(aMatch.Groups[2].Value));
            newPhMenuNode.Url = aMatch.Groups[3].Value;
            newPhMenuNode.Icon = aMatch.Groups[4].Value;

            return newPhMenuNode;
        }
    }
}

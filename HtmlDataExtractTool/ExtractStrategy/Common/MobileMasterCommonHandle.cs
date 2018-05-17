using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlDataExtractTool.ExtractStrategy.Common
{
    public class MobileMasterCommonHandle : ICommonHandle
    {
        private string[] noNeedGetChild = { "" };
        private static List<PhMenuNode> lastResult;

        public string GetMenuFile(string srcDir)
        {
            var menuRegex = new Regex(@"Catalog\.html$");
            var allPages = Directory.GetFiles(Path.Combine(srcDir, "Report"));
            return allPages.First(p => menuRegex.Match(p).Success);
        }

        public List<PhMenuNode> CreateMenuNodes(string menuFileContent, bool getLastResult)
        {
            if (getLastResult && lastResult != null)
                return lastResult;

            lastResult = GetNodeChilds(menuFileContent, new PhMenuNode() { Id = "-3" }, 0).Childs;
            if(lastResult.Count == 0)
                lastResult = GetNodeChilds(menuFileContent, new PhMenuNode() { Id = "-1" }, 0).Childs;
            return lastResult;
        }

        private PhMenuNode GetNodeChilds(string menuFileContent, PhMenuNode parentNode, int deep)
        {
            var matchResults = Regex.Matches(menuFileContent, $@"{{id:(.*), pId:{parentNode.Id}, name:'(.*)', url:'(.*)',.*icon:'(.*)'");
            var phMenuNodes = new List<PhMenuNode>();
            foreach (Match aMatchResult in matchResults)
            {
                var newPhMenuNode = GetTheNodeWithMatch(aMatchResult);
                if(deep < 8 )
                    if(parentNode.Name == null || !parentNode.Name.Contains("文件列表"))
                        GetNodeChilds(menuFileContent, newPhMenuNode, deep + 1);

                phMenuNodes.Add(newPhMenuNode);
            }
            parentNode.Childs = phMenuNodes;

            return parentNode;
        }

        private PhMenuNode GetTheNodeWithMatch(Match aMatch)
        {
            var newPhMenuNode = new PhMenuNode();
            newPhMenuNode.Id = aMatch.Groups[1].Value;
            newPhMenuNode.Name = aMatch.Groups[2].Value;
            newPhMenuNode.Url = aMatch.Groups[3].Value;
            newPhMenuNode.Icon = aMatch.Groups[4].Value;

            return newPhMenuNode;
        }
    }
}

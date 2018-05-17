using HtmlDataExtractTool.ExtractStrategy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlDataExtractTool.Common
{
    public class CommonFunc
    {
        private static readonly Lazy<CommonFunc> lazy = new Lazy<CommonFunc>();
        public static CommonFunc Intance => lazy.Value;


        public List<PhMenuNode> CreateMenuNodes(string menuFileContent, string rootId, string regexInfo)
        {
            return GetNodeChilds(menuFileContent, new PhMenuNode() { Id = rootId }, regexInfo).Childs;
        }

        private PhMenuNode GetNodeChilds(string menuFileContent, PhMenuNode parentNode, string regexInfo)
        {
            var matchResults = Regex.Matches(menuFileContent, regexInfo.Replace("_id", parentNode.Id));
            var phMenuNodes = new List<PhMenuNode>();
            foreach (Match aMatchResult in matchResults)
            {
                var newPhMenuNode = GetTheNodeWithMatch(aMatchResult);
                GetNodeChilds(menuFileContent, newPhMenuNode, regexInfo);

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

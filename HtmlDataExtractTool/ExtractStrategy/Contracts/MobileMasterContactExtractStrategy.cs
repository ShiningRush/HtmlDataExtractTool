using HtmlDataExtractTool.Common;
using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlDataExtractTool.ExtractStrategy.Contracts
{
    public class MobileMasterContactExtractStrategy : ContactExtractStrategy
    {
        public MobileMasterContactExtractStrategy(ICommonHandle commonHandle) : base(commonHandle) { }

        public override List<Contact> GetAllContactsInOneFile(string filePath)
        {
            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            var infoMatch = Regex.Match(fileContent, @"手机\(\d*\)</a></th></tr></table></div><table(.*?)</table>((\s<div)|(<br>))").Groups[1].Value;
            var rowMatch = Regex.Matches(infoMatch, @"<tr><td> \d*</td><td> (.*?)</td><td> (.*?)</td><td> (.*?)</td><td> (.*?)</td> <td>(.*?)</td></tr>");
            var contacts = new List<Contact>();

            foreach (Match aMatch in rowMatch)
            {
                var newContact = new Contact();
                newContact.Name = aMatch.Groups[1].Value;
                newContact.Mobile = aMatch.Groups[2].Value;
                newContact.Group = aMatch.Groups[3].Value;
                newContact.Remark = aMatch.Groups[4].Value;
                newContact.IsDelete = aMatch.Groups[5].Value;
                contacts.Add(newContact);
            }
            return contacts;
        }

        public override string GetContractFile(string menuFile)
        {
            var menuNodes = _commonHandle.CreateMenuNodes(File.ReadAllText(menuFile), true);
            if (menuNodes[0].Childs.First(p => p.Name.Contains("证据列表")).Childs.First().Childs.First(p => p.Name.Contains("手机信息"))
                .Childs.Count(p => p.Name.Contains("通讯录")) == 0)
                return "";

            var node = menuNodes[0].Childs.First(p => p.Name.Contains("证据列表")).Childs.First().Childs.First(p => p.Name.Contains("手机信息"))
                .Childs.First(p => p.Name.Contains("通讯录")).Childs.First(p => p.Name.Contains("手机"));

            return Path.Combine(Path.GetDirectoryName(menuFile), Regex.Match(node.Url, @"Contents(\d*)\.html").Value);
        }
    }
}

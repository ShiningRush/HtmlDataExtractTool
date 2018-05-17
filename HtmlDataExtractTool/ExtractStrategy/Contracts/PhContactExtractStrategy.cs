using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlDataExtractTool.Common;
using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.Models;

namespace HtmlDataExtractTool.ExtractStrategy.Contacts
{
    public class PhContactExtractStrategy : ContactExtractStrategy
    {
        public PhContactExtractStrategy(ICommonHandle commonHandle) : base(commonHandle) { }

        public override List<Contact> GetAllContactsInOneFile(string filePath)
        {
            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            var tableMatch = Regex.Match(fileContent, @"<a class = 'title'.{0,50}?>通讯录.{0,50}?</a>.{0,20}?<table .*?>(.*?)</table>").Groups[1].Value;
            var contactMatch = Regex.Matches(tableMatch, "<tr .*?>(.*?)</tr>");
            var contacts = new List<Contact>();
            foreach (Match aMatch in contactMatch)
            {
                var newContact = new Contact();
                var contactInfoMatches = Regex.Matches(aMatch.Value, "<td>.*?</td>");
                // newContact.Name = Regex.Match(contactInfoMatches[1].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols(); Id
                // newContact.Name = Regex.Match(contactInfoMatches[2].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols(); 昵称
                newContact.Name = Regex.Match(contactInfoMatches[3].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                newContact.Mobile = Regex.Match(contactInfoMatches[4].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                // newContact.Name = Regex.Match(contactInfoMatches[5].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols(); 邮箱
                // newContact.Name = Regex.Match(contactInfoMatches[6].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols(); 归属地

                contacts.Add(newContact);
            }

            return contacts;
        }

        public override string GetContractFile(string menuFile)
        {
            var menuNodes = _commonHandle.CreateMenuNodes(File.ReadAllText(menuFile), true);
            return Path.Combine(Path.GetDirectoryName(menuFile), Regex.Match(menuNodes.First(p => p.Name.Contains("通讯录")).Url, @"(PingHangReportPage_.*_)(\d*)\.html").Value) ;
        }
    }
}

using HtmlDataExtractTool.Common;
using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.ExtractStrategy.SearchModels;
using HtmlDataExtractTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlDataExtractTool.ExtractStrategy.Wechat
{
    public class PhWechatExtractStrategy : WechatExtractStrategy
    {
        public PhWechatExtractStrategy(ICommonHandle commonHandle) : base(commonHandle) { }

        public override List<WechatAccountInfo> GetFriendsInOneFile(string filePath, FileOrder order)
        {
            var regexString = @"好友统计\(\d*\)</a>[\s\S]*?<table[\s\S]*?>[\s\S]*?</table>";
            if (order == FileOrder.Normal)
            {
                regexString = "<table.*?abbr=\"真实姓名\".*?</table>";
            }
            else if (order == FileOrder.Last)
            {
                regexString = "<table.*?abbr=\"真实姓名\".*?</table>";
            }

            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            var friendContent = Regex.Match(fileContent, regexString).Value;
            var friendMatches = Regex.Matches(friendContent, @"<tr .*?>(.*?)</tr>");

            var wechatAccountInfoList = new List<WechatAccountInfo>();
            foreach (Match aMatch in friendMatches)
            {
                var newWechatAccountInfo = new WechatAccountInfo();
                var matches = Regex.Matches(aMatch.Value, @"<td>.*?</td>");

                if(matches.Count > 1)
                    newWechatAccountInfo.Id = Regex.Match(matches[1].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                if (matches.Count > 2)
                    newWechatAccountInfo.Account = Regex.Match(matches[2].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                if (matches.Count > 3)
                    newWechatAccountInfo.Name = Regex.Match(matches[3].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                // newWechatAccountInfo.Name = Regex.Match(matches[4].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value; 真实姓名
                if (matches.Count > 5)
                    newWechatAccountInfo.Mobile = Regex.Match(matches[5].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                if (matches.Count > 6)
                    newWechatAccountInfo.Email = Regex.Match(matches[6].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                if (matches.Count > 7)
                    newWechatAccountInfo.Count = Regex.Match(matches[7].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value;
                wechatAccountInfoList.Add(newWechatAccountInfo);
            }

            return wechatAccountInfoList;
        }

        public override List<WechatGroup> GetGroupsInOneFile(string filePath)
        {
            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            // </div><a class = 'subtitle4'.*?>(.*?)\\(\\d*\\)
            var tableMatches = Regex.Matches(fileContent, "<a class = 'subtitle4'.{0,100}<table(.{0,200}?)abbr=\"群账号\"(.*?)</table>");
            var qqIndex = GetQQIndex(fileContent);

            var groupList = new List<WechatGroup>();
            foreach (Match aMatch in tableMatches)
            {
                if (qqIndex > 0 && fileContent.IndexOf(aMatch.Value) > qqIndex)
                    continue;

                var memberMatches = Regex.Matches(aMatch.Value, @"<tr .*?>(.*?)</tr>");
                var newGroup = new WechatGroup();
                newGroup.Members = new List<WechatAccountInfo>();
                foreach (Match aMembenMatch in memberMatches)
                {
                    var aNewMember = new WechatAccountInfo();
                    var memberInfoMatch = Regex.Matches(aMembenMatch.Value, "<td>.*?</td>");
                    if(string.IsNullOrEmpty(newGroup.Account))
                        newGroup.Account = Regex.Match(memberInfoMatch[1].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                    if (string.IsNullOrEmpty(newGroup.Name))
                        newGroup.Name = Regex.Match(memberInfoMatch[2].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();

                    aNewMember.Account = Regex.Match(memberInfoMatch[3].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                    aNewMember.Name = Regex.Match(memberInfoMatch[4].Value, "<td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
                    newGroup.Members.Add(aNewMember);
                }

                groupList.Add(newGroup);
            }
            return groupList;
        }

        public override List<WechatSearchInfo> GetSearchInfo(string menuFilePath)
        {
            var menuFileContent = File.ReadAllText(menuFilePath);
            var menuNodes = _commonHandle.CreateMenuNodes(menuFileContent);

            var wechatSearchInfo = new List<WechatSearchInfo>();
            var accountNodes = menuNodes.First(p => p.Name.Contains("社交聊天")).Childs.Where(p => p.Name.Contains("微信")).SelectMany(p=>p.Childs);
            foreach (var aMenuNode in accountNodes)
            {
                wechatSearchInfo.Add(CreateSearchInfoWithAccountNode(Path.GetDirectoryName(menuFilePath), aMenuNode));
            }

            return wechatSearchInfo;
        }

        public override WechatAccountInfo GetWechatInfoWithSearchInfo(WechatSearchInfo searchInfo)
        {
            var newAccountInfo = new WechatAccountInfo();
            var startFileContent = File.ReadAllText(searchInfo.StartFile);
            newAccountInfo.Account = Regex.Match(startFileContent, @"账号&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Name = Regex.Match(startFileContent, @"昵称&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.pa = Regex.Match(startFileContent, @"密码&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value;
            newAccountInfo.Email = Regex.Match(startFileContent, @"邮箱&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Sex = Regex.Match(startFileContent, @"性别&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Location = Regex.Match(startFileContent, @"地址&nbsp;<br/></td><td>(.*)&nbsp;<br/></td>").Groups[1].Value.FilterHtmlSpecialSymbols();

            return newAccountInfo;
        }

        private int GetQQIndex(string fileContent)
        {
            var match = Regex.Match(fileContent, @"<a class = 'subtitle'.{0,300}?>QQ\(\d*/\d*\)</a>");
            if (match.Success)
                return fileContent.IndexOf(match.Value);

            return 0;
        }

        private WechatSearchInfo CreateSearchInfoWithAccountNode(string srcPageDir, PhMenuNode accountNode)
        {
            var aNewInfo = new WechatSearchInfo();
            aNewInfo.Account = Regex.Match(accountNode.Name, @"账号:(.*)\(.*\)").Groups[1].Value;
            aNewInfo.StartFile = Path.Combine(srcPageDir, Regex.Match(accountNode.Url, @".*\.html").Value);

            var friendNode = accountNode.Childs.First(p => p.Name.Contains("好友统计"));
            var groupNode = accountNode.Childs.First(p => p.Name.Contains("群联系人"));

            aNewInfo.FriendsFiles = MakeFilesPath(srcPageDir, accountNode, friendNode);
            aNewInfo.GroupsFiles = MakeFilesPath(srcPageDir, accountNode, groupNode);

            return aNewInfo;
        }

        private List<string> MakeFilesPath(string srcPageDir, PhMenuNode parentNode, PhMenuNode aNode)
        {
            var startNumber = Convert.ToInt32(Regex.Match(aNode.Url, @"PingHangReportPage_.*_(\d*)\.html").Groups[1].Value);
            var endNumber = Convert.ToInt32(Regex.Match(parentNode.Childs[parentNode.Childs.IndexOf(aNode) + 1].Url, @"(PingHangReportPage_.*_)(\d*)\.html").Groups[2].Value);
            var baseDir = Path.Combine(srcPageDir, Regex.Match(aNode.Url, @"(PingHangReportPage_.*_)(\d*)\.html").Groups[1].Value + "{0}.html"); ;

            var listString = new List<string>();
            for (var i = startNumber; i <= endNumber; i++)
            {
                listString.Add(string.Format(baseDir, i));
            }

            return listString;
        }
    }
}

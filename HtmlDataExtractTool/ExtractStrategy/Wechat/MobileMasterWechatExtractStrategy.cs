using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlDataExtractTool.Common;
using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.ExtractStrategy.SearchModels;
using HtmlDataExtractTool.Models;

namespace HtmlDataExtractTool.ExtractStrategy.Wechat
{
    public class MobileMasterWechatExtractStrategy : WechatExtractStrategy
    {
        public MobileMasterWechatExtractStrategy(ICommonHandle commonHandle) : base(commonHandle) { }

        public override List<WechatAccountInfo> GetFriendsInOneFile(string filePath, FileOrder order)
        {
            var wechatAccountInfoList = new List<WechatAccountInfo>();
            var regexString = @"好友列表\(\d*\)</a></th></tr></table></div><table.*?</table>((\s<div)|(<br>))";
            if (order == FileOrder.Normal)
            {
                regexString = @"<table.*?</table>(<br>|\s</body>)";
            }
            else if (order == FileOrder.Last)
            {
                regexString = @"<table.*?</table>\s<div";
            }

            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            var friendContent = Regex.Match(fileContent, regexString).Value;

            // check if it is public account
            if (order == FileOrder.Last)
            {
                var paIndex = GetWechatPublicAccountIndex(fileContent);

                if (paIndex > 0 && fileContent.IndexOf(friendContent) >= paIndex)
                {
                    return wechatAccountInfoList;
                }
            }

            var friendMatches = Regex.Matches(friendContent, @"<tr\S.*?>(.*?)</table>");

            foreach (Match aMatch in friendMatches)
            {
                var newWechatAccountInfo = new WechatAccountInfo();
                newWechatAccountInfo.Id = Regex.Match(aMatch.Value,@"</td><td>(.*?)</td><td .*?><div").Groups[1].Value;
                var matches = Regex.Matches(Regex.Match(aMatch.Value, @"<table.*?>(.*?)</table>").Groups[1].Value, @"<tr><td> .*?</td><td> (.*?)</td></tr>");
                if (matches.Count < 2)
                {
                    continue;
                }
                newWechatAccountInfo.Account = matches[0].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Name = matches[1].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Mobile = matches[2].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Sex = matches[3].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Description = matches[4].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Remark = matches[5].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Location = matches[6].Groups[1].Value.FilterHtmlSpecialSymbols();
                // newWechatAccountInfo. = matches[7].Groups[1].Value.FilterHtmlSpecialSymbols(); 头像
                newWechatAccountInfo.BindQQ = matches[8].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.BindQQName = matches[9].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.Email = matches[10].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.BindQQDescription = matches[11].Groups[1].Value.FilterHtmlSpecialSymbols();
                newWechatAccountInfo.BindQQTip = matches[12].Groups[1].Value.FilterHtmlSpecialSymbols();

                wechatAccountInfoList.Add(newWechatAccountInfo);
            }

            return wechatAccountInfoList;
        }

        private int GetWechatPublicAccountIndex(string fileContent)
        {
            var match = Regex.Match(fileContent, @"<table.{0,150}?公众号\(\d*\)</a></th></tr></table>");
            if (match.Success)
                return fileContent.IndexOf(match.Value);

            return 0;
        }

        public override List<WechatGroup> GetGroupsInOneFile(string filePath, FileOrder order)
        {
            var fileContent = File.ReadAllText(filePath).ReplaceNoNeedSymbols();
            var tableMatches = Regex.Matches(fileContent, @"<table .{0,350}?成员(帐|账)号</td>.*?</table>((\s<div)|(<br>)|(\s</body>))");

            var groupList = new List<WechatGroup>();
            foreach (Match aMatch in tableMatches)
            {


                var newGroup = new WechatGroup();
                newGroup.Members = new List<WechatAccountInfo>();
                var memberMatches = Regex.Matches(aMatch.Value, @"<tr\S.*?>(.*?)</table></div></td></tr>");
                foreach (Match aMembenMatch in memberMatches)
                {
                    var aNewMember = new WechatAccountInfo();
                    newGroup.Account = Regex.Match(aMembenMatch.Value, @"</td><td>(.*?)</td><td .*?><div").Groups[1].Value;
                    var memberInfoMatch = Regex.Matches(aMembenMatch.Value, @"<tr><td> .*?</td><td> (.*?)</td></tr>");
                    if (string.IsNullOrEmpty(newGroup.Name))
                        newGroup.Name = memberInfoMatch[0].Groups[1].Value.FilterHtmlSpecialSymbols();

                    aNewMember.Name = memberInfoMatch[1].Groups[1].Value.FilterHtmlSpecialSymbols();
                    aNewMember.Account = memberInfoMatch[2].Groups[1].Value.FilterHtmlSpecialSymbols();
                    aNewMember.Description = memberInfoMatch[3].Groups[1].Value.FilterHtmlSpecialSymbols();
                    aNewMember.Location = memberInfoMatch[4].Groups[1].Value.FilterHtmlSpecialSymbols();
                    // aNewMember.Account = memberInfoMatch[5].Groups[1].Value.FilterHtmlSpecialSymbols(); 头像
                    if (memberInfoMatch.Count > 6)
                        aNewMember.Inway = memberInfoMatch[6].Groups[1].Value.FilterHtmlSpecialSymbols();

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
            if(menuNodes[0].Childs.First(p => p.Name.Contains("证据列表")).Childs.First().Childs.First(p => p.Name.Contains("即时通讯"))
                                        .Childs.Count(p => p.Name.Contains("微信")) > 0)
            {
                var accountNodes = menuNodes[0].Childs.First(p => p.Name.Contains("证据列表")).Childs.First().Childs.First(p => p.Name.Contains("即时通讯"))
                            .Childs.First(p => p.Name.Contains("微信")).Childs.First(p => p.Name.Contains("微信")).Childs;
                foreach (var aMenuNode in accountNodes)
                {
                    if (!aMenuNode.Name.Contains("(删除数据)"))
                        wechatSearchInfo.Add(CreateSearchInfoWithAccountNode(Path.GetDirectoryName(menuFilePath), aMenuNode));
                }
            }

            return wechatSearchInfo;
        }

        public override WechatAccountInfo GetWechatInfoWithSearchInfo(WechatSearchInfo searchInfo)
        {
            var newAccountInfo = new WechatAccountInfo();
            var startFileContent = File.ReadAllText(searchInfo.StartFile).ReplaceNoNeedSymbols();
            var infoMatch = Regex.Match(startFileContent, @"帐号信息\(\d*\)</a></th></tr></table></div><table.{0,200}?<td>微信号(.{0,400}?" +  searchInfo.Account +@".*?)</table>\s<div").Groups[1].Value;
            var rowMatch = Regex.Matches(infoMatch, @"<tr\S.*?>(.*?</table>)")[0].Groups[1].Value;

            newAccountInfo.Account = Regex.Match(rowMatch, @"</td><td>(.*?)</td><td .*?><div").Groups[1].Value;
            var matches = Regex.Matches(Regex.Match(rowMatch, @"<table.*?>(.*?)</table>").Groups[1].Value, @"<tr><td> .*?</td><td> (.*?)</td></tr>");
            newAccountInfo.Name = matches[0].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Id = matches[1].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.BindQQ = matches[2].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Account = matches[3].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Sex = matches[4].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Description = matches[5].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Remark = matches[6].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Account = matches[7].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Account = matches[8].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Account = matches[9].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Email = matches[10].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Email = matches[11].Groups[1].Value.FilterHtmlSpecialSymbols();
            //newAccountInfo.Email = matches[12].Groups[1].Value.FilterHtmlSpecialSymbols();
            newAccountInfo.Mobile = matches[13].Groups[1].Value.FilterHtmlSpecialSymbols();


            return newAccountInfo;
        }

        private WechatSearchInfo CreateSearchInfoWithAccountNode(string srcPageDir, PhMenuNode accountNode)
        {
            var aNewInfo = new WechatSearchInfo();
            aNewInfo.Account = Regex.Match(accountNode.Name, @"(.*)/.*").Groups[1].Value;
            if(string.IsNullOrEmpty(aNewInfo.Account))
                aNewInfo.Account = Regex.Match(accountNode.Name, @"(.*)-.*").Groups[1].Value;
            aNewInfo.StartFile = Path.Combine(srcPageDir, Regex.Match(accountNode.Url, @".*\.html").Value);

            if (accountNode.Childs.First(p => p.Name.Contains("通讯录")).Childs.Count(p => p.Name.Contains("好友列表")) > 0)
            {
                var friendNode = accountNode.Childs.First(p => p.Name.Contains("通讯录")).Childs.First(p => p.Name.Contains("好友列表"));
                aNewInfo.FriendsFiles = MakeFilesPath(srcPageDir, accountNode.Childs.First(p => p.Name.Contains("通讯录")), friendNode);
            }

            if (accountNode.Childs.Count(p => p.Name.Contains("群成员列表")) > 0)
            {
                var groupNode = accountNode.Childs.First(p => p.Name.Contains("群成员列表"));
                aNewInfo.GroupsFiles = MakeFilesPath(srcPageDir, accountNode, groupNode);
            }

            return aNewInfo;
        }

        private List<string> MakeFilesPath(string srcPageDir, PhMenuNode parentNode, PhMenuNode aNode)
        {
            var startNumber = Convert.ToInt32(Regex.Match(aNode.Url, @"Contents(\d*)\.html").Groups[1].Value);
            var endNumber = Convert.ToInt32(Regex.Match(parentNode.Childs[parentNode.Childs.IndexOf(aNode) + 1].Url, @"(Contents)(\d*)\.html").Groups[2].Value);
            var baseDir = Path.Combine(srcPageDir, Regex.Match(aNode.Url, @"(Contents)(\d*)\.html").Groups[1].Value + "{0}.html"); ;

            var listString = new List<string>();
            for (var i = startNumber; i <= endNumber; i++)
            {
                listString.Add(string.Format(baseDir, i));
            }

            return listString;
        }
    }
}

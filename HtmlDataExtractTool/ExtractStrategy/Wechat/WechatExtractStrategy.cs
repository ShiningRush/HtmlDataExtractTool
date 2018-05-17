using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.ExtractStrategy.SearchModels;
using HtmlDataExtractTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.ExtractStrategy
{
    public enum FileOrder
    {
        First,
        Normal,
        Last
    }

    public abstract class WechatExtractStrategy
    {
        
        public abstract List<WechatSearchInfo> GetSearchInfo(string menuFilePath);
        public abstract List<WechatAccountInfo> GetFriendsInOneFile(string filePath, FileOrder order);
        public abstract List<WechatGroup> GetGroupsInOneFile(string filePath);

        public abstract WechatAccountInfo GetWechatInfoWithSearchInfo(WechatSearchInfo searchInfo);

        protected readonly ICommonHandle _commonHandle;
        public WechatExtractStrategy(ICommonHandle commonHandle)
        {
            _commonHandle = commonHandle;
        }

        public List<WechatAccountInfo> BeginExtract(string srcFilePath)
        {
            var menuFile = _commonHandle.GetMenuFile(srcFilePath);
            var allWechatInfo = new List<WechatAccountInfo>();
            foreach (var aWechatSearchInfo in GetSearchInfo(menuFile))
            {
                var newAccountInfo = GetWechatInfoWithSearchInfo(aWechatSearchInfo);
                newAccountInfo.Friends = ExtractFriends(aWechatSearchInfo);
                newAccountInfo.Groups = ExtractGroups(aWechatSearchInfo);

                allWechatInfo.Add(newAccountInfo);
            }

            return allWechatInfo;
        }

        public List<WechatAccountInfo> ExtractFriends(WechatSearchInfo wechatAccountinfo)
        {
            var allFriends = new List<WechatAccountInfo>();
            for (var i = 0; i < wechatAccountinfo.FriendsFiles.Count(); i++)
            {
                var aFile = wechatAccountinfo.FriendsFiles[i];
                if (i == 0)
                {
                    allFriends.AddRange(GetFriendsInOneFile(aFile, FileOrder.First));
                }
                else if (i == wechatAccountinfo.FriendsFiles.Count() - 1)
                {
                    allFriends.AddRange(GetFriendsInOneFile(aFile, FileOrder.Last));
                }
                else
                {
                    allFriends.AddRange(GetFriendsInOneFile(aFile, FileOrder.Normal));
                }
            }

            return allFriends;
        }

        public List<WechatGroup> ExtractGroups(WechatSearchInfo wechatAccountinfo)
        {
            var allGroups = new List<WechatGroup>();
            foreach (var aFile in wechatAccountinfo.GroupsFiles)
            {
                var oneFilesGroups = GetGroupsInOneFile(aFile);
                var lastGroupMembers = oneFilesGroups.FirstOrDefault(p=> string.IsNullOrEmpty(p.Name))?.Members;

                allGroups.AddRange(oneFilesGroups);
            }

            return allGroups;
        }
    }
}

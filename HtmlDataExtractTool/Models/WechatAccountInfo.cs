using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.Models
{
    public class WechatAccountInfo
    {
        [ExcelColumnIndex(4)]
        public string Id { get; set; }
        [ExcelColumnIndex(5)]
        public string Account { get; set; }

        [ExcelColumnIndex(6)]
        public string Name { get; set; }
        [ExcelColumnIndex(7)]
        public string Mobile { get; set; }
        [ExcelColumnIndex(8)]
        public string Sex { get; set; }
        [ExcelColumnIndex(9)]
        public string Description { get; set; }
        [ExcelColumnIndex(10)]
        public string Remark { get; set; }
        [ExcelColumnIndex(11)]
        public string Count { get; set; }
        [ExcelColumnIndex(12)]
        public string Location { get; set; }
        [ExcelColumnIndex(13)]
        public string BindQQ { get; set; }
        [ExcelColumnIndex(14)]
        public string BindQQName { get; set; }
        [ExcelColumnIndex(15)]
        public string Email { get; set; }
        [ExcelColumnIndex(16)]
        public string BindQQDescription { get; set; }
        [ExcelColumnIndex(17)]
        public string BindQQTip { get; set; }

        public string Inway { get; set; }


        public List<WechatAccountInfo> Friends { get; set; }
        public List<WechatGroup> Groups { get; set; }
    }
}

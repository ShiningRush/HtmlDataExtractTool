using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.Models
{
    public class Contact
    {
        [ExcelColumnIndex(3)]
        public string Name { get; set; }
        [ExcelColumnIndex(4)]
        public string Mobile { get; set; }
        [ExcelColumnIndex(5)]
        public string Group { get; set; }
        [ExcelColumnIndex(6)]
        public string Remark { get; set; }
        [ExcelColumnIndex(7)]
        public string IsDelete { get; set; }
    }
}

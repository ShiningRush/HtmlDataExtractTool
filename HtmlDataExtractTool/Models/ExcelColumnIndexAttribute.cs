using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.Models
{
    public class ExcelColumnIndexAttribute : Attribute
    {
        public int ColIndex { get; set; }

        public ExcelColumnIndexAttribute(int colIdx)
        {
            ColIndex = colIdx;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.Common
{
    public static class StringExtensions
    {
        public static string FilterHtmlSpecialSymbols(this string @this)
        {
            return @this.Replace("&nbsp;", " ");
        }

        public static string ReplaceNoNeedSymbols(this string @this)
        {
            return @this.Replace("\n", "").Replace("\r\n", "");
        }
    }
}

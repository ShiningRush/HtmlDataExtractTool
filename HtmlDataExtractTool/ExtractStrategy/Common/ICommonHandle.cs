using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.ExtractStrategy.Common
{
    public interface ICommonHandle
    {
        string GetMenuFile(string srcDir);
        List<PhMenuNode> CreateMenuNodes(string menuFileContent, bool getLastResult = false);
    }
}

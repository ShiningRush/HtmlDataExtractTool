using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlDataExtractTool.ExtractStrategy
{
    public abstract class ContactExtractStrategy
    {
        public abstract string GetContractFile(string menuFile);
        public abstract List<Contact> GetAllContactsInOneFile(string filePath);

        protected readonly ICommonHandle _commonHandle;
        public ContactExtractStrategy(ICommonHandle commonHandle)
        {
            _commonHandle = commonHandle;
        }

        public List<Contact> BeginExtract(string srcDir)
        {
            var menuFile = _commonHandle.GetMenuFile(srcDir);
            var contractFile = GetContractFile(menuFile);
            var allContacts = new List<Contact>();
            if (!string.IsNullOrEmpty(contractFile))
                allContacts  = GetAllContactsInOneFile(contractFile);

            return allContacts;
        }
    }
}

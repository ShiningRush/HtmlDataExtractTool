using HtmlDataExtractTool.ExtractStrategy;
using HtmlDataExtractTool.ExtractStrategy.Common;
using HtmlDataExtractTool.ExtractStrategy.Contacts;
using HtmlDataExtractTool.ExtractStrategy.Contracts;
using HtmlDataExtractTool.ExtractStrategy.Wechat;
using HtmlDataExtractTool.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlDataExtractTool
{
    public partial class Form1 : Form
    {
        private bool _isLoading = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectImportDir_Click(object sender, EventArgs e)
        {
            if (isLoading())
                return;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                txtImportDir.Text = dialog.SelectedPath;
            }

        }

        private void btnSelectExportDir_Click(object sender, EventArgs e)
        {
            if (isLoading())
                return;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtExportDir.Text = dialog.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isLoading())
                return;

            if (string.IsNullOrEmpty(txtImportDir.Text))
            {
                MessageBox.Show("请选择源文件路径");
                return;
            }

            if (string.IsNullOrEmpty(txtExportDir.Text))
            {
                MessageBox.Show("请选择导出路径");
                return;
            }

            var allChildFolders = Directory.GetDirectories(txtImportDir.Text);
            var phFolder = allChildFolders.Count(p => p.Contains("Content"));
            var mobileMasterFolder = allChildFolders.Count(p => p.Contains("Report"));


            if (phFolder == 0 && mobileMasterFolder == 0)
            {
                MessageBox.Show("选择的源文件路径错误，请选择报告文件的根目录");
                return;
            }

            _isLoading = true;
            WechatExtractStrategy extractStrategy;
            ContactExtractStrategy contactStrategy;
            btnStart.Text = "搜集中...";
            btnStart.Enabled = false;

            new Task(() =>
            {
                try
                {
                    if (phFolder != 0)
                    {
                        extractStrategy = new PhWechatExtractStrategy(new PhCommonHandle());
                        contactStrategy = new PhContactExtractStrategy(new PhCommonHandle());
                    }
                    else
                    {
                        extractStrategy = new MobileMasterWechatExtractStrategy(new MobileMasterCommonHandle());
                        contactStrategy = new MobileMasterContactExtractStrategy(new MobileMasterCommonHandle());
                    }

                    var result = extractStrategy.BeginExtract(txtImportDir.Text);
                    var contacts = contactStrategy.BeginExtract(txtImportDir.Text);

                    var fileName = Regex.Match(Directory.GetFiles(txtImportDir.Text).First(), @"(.*)\.html").Groups[1].Value;
                    ExportDataAsExcel(fileName, result, contacts);

                    MessageBox.Show("搜集完成!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("搜集过程中发生意料外的错误，请联系开发者");
                }
                finally
                {
                    _isLoading = false;
                    MethodInvoker mi = new MethodInvoker(() =>
                    {
                        btnStart.Text = "开始搜集";
                        btnStart.Enabled = true;
                    });
                    this.BeginInvoke(mi);

                }
            }).Start();

        }

        private bool isLoading()
        {
            if (_isLoading)
            {
                MessageBox.Show("程序正在搜集数据中，请不要点击按钮");
                return true;
            }

            return false;
        }

        #region ExportExcel

        private void ExportDataAsExcel(string fileName, List<WechatAccountInfo> accountInfos, List<Contact> contactInfos)
        {
            var templateExcelFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sources", "template.xlsx");
            using (var excelPage = new ExcelPackage(new FileInfo(templateExcelFilePath)))
            {
                var wsFriendSheet = excelPage.Workbook.Worksheets["微信好友"];
                var wsGroupsMember = excelPage.Workbook.Worksheets["微信群"];
                var wsContact = excelPage.Workbook.Worksheets["通讯录"];

                int friendSheetRow = 2, groupSheetRow = 2;
                foreach (var aAccountInfo in accountInfos)
                {
                    friendSheetRow = WriteFriendsToExcel(friendSheetRow, aAccountInfo.Name, aAccountInfo.Friends, wsFriendSheet);
                    groupSheetRow = WriteGroupMembersToExcel(groupSheetRow, aAccountInfo.Name, aAccountInfo.Groups, wsGroupsMember);
                }

                WriteContactsToExcel(contactInfos, wsContact);
                excelPage.SaveAs(new FileInfo(Path.Combine(txtExportDir.Text, Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx")));
            }
        }

        private int WriteFriendsToExcel(int startRow, string wechatName, List<WechatAccountInfo> friends, ExcelWorksheet friendSheet)
        {
            var order = 0;
            foreach (var aFriendInfo in friends)
            {
                var rowIndex = startRow + order;
                friendSheet.SetValue(rowIndex, 1, txtExportDir.Text);
                friendSheet.SetValue(rowIndex, 2, wechatName);
                friendSheet.SetValue(rowIndex, 3, order + 1);
                WriteRowWithObject(rowIndex, aFriendInfo, friendSheet);
                order++;
            }

            return startRow + order;
        }

        private int WriteGroupMembersToExcel(int startRow, string wechatName, List<WechatGroup> groups, ExcelWorksheet groupSheet)
        {
            var order = 0;
            foreach (var aGroup in groups)
            {
                foreach (var aMember in aGroup.Members)
                {
                    var rowIndex = startRow + order;
                    groupSheet.SetValue(rowIndex, 1, txtExportDir.Text);
                    groupSheet.SetValue(rowIndex, 2, wechatName);
                    groupSheet.SetValue(rowIndex, 3, order + 1);
                    groupSheet.SetValue(rowIndex, 4, aGroup.Account);
                    groupSheet.SetValue(rowIndex, 5, aGroup.Name);
                    groupSheet.SetValue(rowIndex, 6, aMember.Account);
                    groupSheet.SetValue(rowIndex, 7, aMember.Name);
                    groupSheet.SetValue(rowIndex, 8, aMember.Description);
                    groupSheet.SetValue(rowIndex, 9, aMember.Location);
                    groupSheet.SetValue(rowIndex, 10, aMember.Inway);
                    order++;
                }
            }

            return startRow + order;
        }

        private void WriteContactsToExcel(List<Contact> contact, ExcelWorksheet contactSheet)
        {
            int order = 1, startRow = 1;
            foreach (var aFriendInfo in contact)
            {
                var rowIndex = startRow + order;
                contactSheet.SetValue(rowIndex, 1, txtExportDir.Text);
                contactSheet.SetValue(rowIndex, 2, order);
                WriteRowWithObject(rowIndex, aFriendInfo, contactSheet);
                order++;
            }
        }

        private void WriteRowWithObject(int rowIndex, object aObject, ExcelWorksheet aSheet)
        {
            var propMapping = MakePropertyMap(aObject);
            var allProperties = aObject.GetType().GetProperties();
            foreach (var aProperty in allProperties)
            {
                int colIdx;
                if (propMapping.TryGetValue(aProperty, out colIdx))
                {
                    aSheet.SetValue(rowIndex, colIdx, aProperty.GetValue(aObject, null));
                }
            }
        }

        private Dictionary<PropertyInfo, int> MakePropertyMap(object aObject)
        {
            var result = new Dictionary<PropertyInfo, int>();
            var allProperties = aObject.GetType().GetProperties();
            foreach (var aProperty in allProperties)
            {
                var excelAttr = aProperty.GetCustomAttributes(typeof(ExcelColumnIndexAttribute), true);
                if (excelAttr.Count() != 0)
                {
                    result[aProperty] = ((ExcelColumnIndexAttribute)excelAttr[0]).ColIndex;
                }
            }

            return result;
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}

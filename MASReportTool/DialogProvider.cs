using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MASReportTool
{
    class DialogProvider
    {
        private static readonly SaveFileDialog SaveFileDialog = new SaveFileDialog();
        private static readonly OpenFileDialog OpenFileDialog = new OpenFileDialog();

        public static void ShowSaveDocxFileDialog(string initialFileName, out bool? dialogResult, out string resultFileName)
        {
            SaveFileDialog.Filter = "Word文件|*.docx";
            SaveFileDialog.DefaultExt = ".docx";
            SaveFileDialog.FileName = initialFileName;
            dialogResult = SaveFileDialog.ShowDialog();
            resultFileName = SaveFileDialog.FileName;
        }

        public static void ShowSaveJsonrFileDialog(out bool? dialogResult, out string resultFileName)
        {
            SaveFileDialog.Filter = "MAS報告|*.jsonr";
            SaveFileDialog.DefaultExt = ".jsonr";
            dialogResult = SaveFileDialog.ShowDialog();
            resultFileName = SaveFileDialog.FileName;
        }

        public static void ShowOpenJsonrFileDialog(out bool? dialogResult, out string resultFileName)
        {
            OpenFileDialog.Filter = "MAS報告|*.jsonr";
            OpenFileDialog.DefaultExt = ".jsonr";
            dialogResult = OpenFileDialog.ShowDialog();
            resultFileName = OpenFileDialog.FileName;
            var extension = Path.GetExtension(resultFileName).ToLower();
            if (extension != ".jsonr")
            {
                dialogResult = false;
                MessageBox.Show("不支援此檔案格式");
            }
        }

        public static void ShowOpenPicsFileDialog(out bool? dialogResult, out string[] resultFiles)
        {
            OpenFileDialog.Filter = "圖片檔|*.jpg; *.png; *.gif";
            OpenFileDialog.Multiselect = true;
            dialogResult = OpenFileDialog.ShowDialog();
            resultFiles = OpenFileDialog.FileNames;
        }
    }
}

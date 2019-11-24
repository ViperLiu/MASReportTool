using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

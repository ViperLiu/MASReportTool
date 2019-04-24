using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MASReportTool.ViewModels
{
    class FunctionBarViewModel
    { 
        public Report Report { get; }

        public FunctionBarViewModel(Report report)
        {
            this.Report = report;
        }

        public ICommand SaveFile
        {
            get { return new RelayCommand(SaveFileExecute, CanSaveFileExecute); }
        }

        void SaveFileExecute()
        {
            Console.WriteLine(Report.CurrentOpenedFile);
            //如果檔案存在就直接存檔
            if (File.Exists(Report.CurrentOpenedFile))
            {
                Save(Report.CurrentOpenedFile);
                return;
            }

            //檔案不存在就開啟存檔視窗
            SaveAsNewFile();
        }
        bool CanSaveFileExecute()
        {
            return true;
        }
        private void Save(string file)
        {
            Console.WriteLine("saved : " + file);
            JsonFileController json = new JsonFileController(file);
            json.SaveFile(Report);
            Report.MarkAsSaved();
            Report.CurrentOpenedFile = file;
        }
        private void SaveAsNewFile()
        {
            SaveFileDialog saveJsonrDialog = new SaveFileDialog();
            saveJsonrDialog.Filter = "MAS報告|*.jsonr";
            saveJsonrDialog.DefaultExt = ".jsonr";
            saveJsonrDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var result = saveJsonrDialog.ShowDialog();
            var file = saveJsonrDialog.FileName;
            if (result == true)
            {
                Save(file);
            }
        }
    }
}

using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MASReportTool.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        //private Dictionary<string, RuleContents> RuleContents { get; set; }
        public static Dictionary<string, RuleContents> RuleContent;

        private ObservableCollection<TabViewModel> _TabItems;
        public ObservableCollection<TabViewModel> TabItems
        {
            get{ return _TabItems; }
            set
            {
                if (value != _TabItems)
                {
                    _TabItems = value;
                    OnPropertyChanged("TabItems");
                }
            }
        }

        private TabViewModel _currentSelectedTab;
        public TabViewModel CurrentSelectedTab
        {
            get { return _currentSelectedTab; }
            set
            {
                if(value != _currentSelectedTab)
                {
                    _currentSelectedTab = value;
                    OnPropertyChanged("CurrentSelectedTab");
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveJsonFile
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {
                        //如果檔案存在就直接存檔
                        if (File.Exists(CurrentSelectedTab.Report.CurrentOpenedFile))
                        {
                            SaveFile(CurrentSelectedTab.Report.CurrentOpenedFile);
                            return;
                        }

                        //檔案不存在就開啟存檔視窗
                        SaveAsNewFile();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand NewReport
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {
                        TabItems.Add(new TabViewModel());
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand LoadJsonFile
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {

                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "jsonr檔|*.jsonr"
                        };
                        var result = openFileDialog.ShowDialog();
                        string file = openFileDialog.FileName;
                        string extension = Path.GetExtension(file).ToLower();
                        if (result == true)
                        {
                            if (extension == ".jsonr")
                            {
                                JsonFileController json = new JsonFileController(file);
                                CurrentSelectedTab.Report = json.LoadFile();
                                CurrentSelectedTab.Report.RegistPropertyChangedEvent();
                                Console.WriteLine("[INFO] 載入檔案：" + CurrentSelectedTab.Report.CurrentOpenedFile);
                            }
                            else
                            {
                                MessageBox.Show("不支援此檔案格式");
                            }
                        }
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand BuildReport
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {
                        SaveFileDialog outputReportDialog = new SaveFileDialog
                        {
                            Filter = "Word文件|*.docx",
                            DefaultExt = ".docx",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                            FileName = Path.GetFileNameWithoutExtension(TabItems[0].Report.CurrentOpenedFile)
                        };
                        var result = outputReportDialog.ShowDialog();
                        var file = outputReportDialog.FileName;

                        if (result == false)
                            return;

                        MASReport reportFile = new MASReport(TabItems[0].Report);
                        try
                        {
                            reportFile.BuildReport(file);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message + "\r\n");
                            Console.WriteLine(exception.StackTrace + "\r\n");
                            return;
                        }
                        Process.Start(file);
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand SelectedTabChanged
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {
                        var indexBefore = TabItems.IndexOf(CurrentSelectedTab);
                        var indexAfter = (int)obj;
                        TabItems[indexBefore].DisableClassChangedCommand();
                        Console.WriteLine("ClassChangedDisable, Index : " + indexBefore);
                        if (!TabItems[indexAfter].IsClassChangedEnable)
                        {
                            TabItems[indexAfter].EnableClassChangedCommand();
                            Console.WriteLine("ClassChangedEnable, Index : " + indexAfter);
                        }
                        CurrentSelectedTab = TabItems[indexAfter];
                        Console.WriteLine("Tabs changed");
                    },
                    () => { return true; }
                    );
            }
        }


        public MainViewModel()
        {
            RuleContent = LoadRuleContents();
            TabItems = new ObservableCollection<TabViewModel>();
            TabItems.Add(new TabViewModel());
            CurrentSelectedTab = TabItems[0];
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static Dictionary<string, RuleContents> LoadRuleContents()
        {
            Dictionary<string, RuleContents> ruleContents = new Dictionary<string, RuleContents>();
            JObject o = JsonFileController.LoadJsonFile("assets\\基準30.json");
            foreach (var rule in o)
            {
                RuleContents r = new RuleContents(
                    rule.Value["Title"].ToString(),
                    rule.Value["NotFitText"].ToString(),
                    rule.Value["PassCondition"].ToString(),
                    (int)rule.Value["Class"]
                    );

                int number = 1;
                foreach (JObject sub in rule.Value["SubRuleList"])
                {
                    SubRuleContents content = new SubRuleContents(
                        sub["Description"].ToString(),
                        sub["DefaultAcceptText"].ToString(),
                        sub["DefaultFailText"].ToString(),
                        number);
                    r.SubRuleContentsList.Add(content);
                    number++;
                }
                ruleContents.Add(rule.Key, r);
            }
            return ruleContents;
        }

        

        private void SaveFile(string file)
        {
            Console.WriteLine("[INFO] 儲存檔案 : " + file);
            JsonFileController json = new JsonFileController(file);
            json.SaveFile(CurrentSelectedTab.Report);
            CurrentSelectedTab.Report.CurrentOpenedFile = file;
            CurrentSelectedTab.Report.MarkAsSaved();
        }

        private void SaveAsNewFile()
        {
            SaveFileDialog saveJsonrDialog = new SaveFileDialog
            {
                Filter = "MAS報告|*.jsonr",
                DefaultExt = ".jsonr",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };
            var result = saveJsonrDialog.ShowDialog();
            var file = saveJsonrDialog.FileName;
            if (result == true)
            {
                SaveFile(file);
            }
        }

    }
}

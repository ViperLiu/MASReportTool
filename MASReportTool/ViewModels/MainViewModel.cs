using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        private Report _Report;
        public Report Report
        {
            get { return _Report; }
            private set
            {
                if(value != _Report)
                {
                    _Report = value;
                    OnPropertyChanged("Report");
                    TreeViewItems = TreeViewItemsViewModel.GetTreeViewItems(_Report);
                    CurrentSelectedRule = TreeViewItems[0].Items[0].RuleResult;
                }
            }
        }

        private List<TreeViewItemsViewModel> _TreeViewItems;
        public List<TreeViewItemsViewModel> TreeViewItems
        {
            get { return _TreeViewItems; }
            private set
            {
                if (value != _TreeViewItems)
                {
                    this._TreeViewItems = value;
                    OnPropertyChanged("TreeViewItems");
                }
            }
        }

        private Picture _CurrentSelectedPic;
        public Picture CurrentSelectedPic
        {
            get { return _CurrentSelectedPic; }
            set
            {
                if (value != _CurrentSelectedPic)
                {
                    this._CurrentSelectedPic = value;
                    OnPropertyChanged("CurrentSelectedPic");
                }
            }
        }

        private bool _IsPicturePanelShown = false;
        public bool IsPicturePanelShown
        {
            get { return _IsPicturePanelShown; }
            set
            {
                if (value != _IsPicturePanelShown)
                {
                    this._IsPicturePanelShown = value;
                    OnPropertyChanged("IsPicturePanelShown");
                }
            }
        }

        private RuleResults _CurrentSelectedRule;
        public RuleResults CurrentSelectedRule
        {
            get { return _CurrentSelectedRule; }
            set
            {
                if (value != _CurrentSelectedRule)
                {
                    _CurrentSelectedRule = value;
                    OnPropertyChanged("CurrentSelectedRule");
                }
            }
        }

        private SubRuleResult _CurrentSelectedSubRule;
        public SubRuleResult CurrentSelectedSubRule
        {
            get { return _CurrentSelectedSubRule; }
            set
            {
                if (value != _CurrentSelectedSubRule)
                {
                    _CurrentSelectedSubRule = value;
                    OnPropertyChanged("CurrentSelectedSubRule");
                }
            }
        }

        private string _currentRuleNumber;
        public string CurrentRuleNumber
        {
            get { return _currentRuleNumber; }
            set
            {
                if (value != _currentRuleNumber)
                {
                    _currentRuleNumber = value;
                    OnPropertyChanged("CurrentRuleNumber");
                }
            }
        }

        private int _currentSubRuleIndex;
        public int CurrentSubRuleIndex
        {
            get { return _currentSubRuleIndex; }
            set
            {
                if (value != _currentSubRuleIndex)
                {
                    _currentSubRuleIndex = value;
                    OnPropertyChanged("CurrentSubRuleIndex");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SelectedItemChanged
        {
            get {
                return new RelayCommand(
                    (object obj) => {
                        var item = obj as TreeViewItemsViewModel;
                        if (item.Level == 0)
                            return;
                        CurrentSelectedRule = item.RuleResult;
                        Console.WriteLine(item.Title);
                    }, 
                    () => {
                        return true;
                    }
                    );
            }
        }

        public ICommand ClassChanged
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var rb = obj as RadioButton;
                        if (rb.IsChecked == false)
                            return;
                        Report.Class = int.Parse(rb.Tag.ToString());
                        TreeViewItems = TreeViewItemsViewModel.GetTreeViewItems(Report);
                        CurrentSelectedRule = TreeViewItems[0].Items[0].RuleResult;
                        Console.WriteLine(rb.Tag);
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand RuleAccepted
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        CurrentSelectedRule.Accept();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand RuleFailed
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        CurrentSelectedRule.Fail();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand RuleNotfit
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        CurrentSelectedRule.NotFit();
                    },
                    () => {
                        if (CurrentSelectedRule.Content.NotFitText == "None")
                            return false;
                        else
                            return true;
                    }
                    );
            }
        }

        public ICommand SubRuleResultChanged
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var rb = obj as RadioButton;
                        if (rb.IsChecked == false)
                            return;
                        var btnStr = rb.Content.ToString();
                        var subRule = rb.Tag as SubRuleResult;
                        if (btnStr == "符合")
                            subRule.Accept();
                        else if (btnStr == "不符合")
                            subRule.Fail();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand PictureButtonClicked
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var subRule = obj as SubRuleResult;
                        CurrentSelectedSubRule = subRule;
                        CurrentSelectedPic = subRule.Pictures.Count < 1 ? null : subRule.Pictures[0];
                        IsPicturePanelShown = true;
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand AddPicture
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {

                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "圖片檔|*.jpg; *.png; *.gif",
                            Multiselect = true
                        };
                        var result = openFileDialog.ShowDialog();
                        var files = openFileDialog.FileNames;

                        if (result == false)
                            return;

                        CurrentSelectedSubRule.AddPictures(files);
                        CurrentSelectedPic = CurrentSelectedSubRule.Pictures.Last();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand ClosePicturePanel
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        IsPicturePanelShown = false;
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand ThumbnailClicked
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        if (Mouse.RightButton == MouseButtonState.Pressed)
                            return;
                        var picture = obj as Picture;
                        CurrentSelectedPic = picture;
                        Console.WriteLine(picture.FullPath);
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand PictureMoveUp
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var picture = obj as Picture;
                        var index = picture.Index;
                        CurrentSelectedSubRule.SwapPictures(index, index - 1);
                        Console.WriteLine("Picture Up");
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand PictureMoveDown
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var picture = obj as Picture;
                        var index = picture.Index;
                        CurrentSelectedSubRule.SwapPictures(index, index + 1);
                        Console.WriteLine("Picture Down");
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand PictureDeleted
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var picture = obj as Picture;
                        var index = picture.Index;
                        CurrentSelectedSubRule.DeletePicture(index);
                        Console.WriteLine("[INFO] 刪除圖片：" + picture.FullPath);
                        CurrentSelectedPic = null;
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand SaveJsonFile
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        //如果檔案存在就直接存檔
                        if (File.Exists(Report.CurrentOpenedFile))
                        {
                            SaveFile(Report.CurrentOpenedFile);
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
                    (object obj) => {
                        if (Report.IsSaved)
                        {
                            Report.Reset();
                            return;
                        }
                        var result = MessageBox.Show(
                            "開始新的報告前，是否先存檔？", "尚未存檔", MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                            SaveJsonFile.Execute(new object());
                        else if (result == MessageBoxResult.No)
                            Report.Reset();
                        else if (result == MessageBoxResult.Cancel)
                            return;
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
                    (object obj) => {

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
                                this.Report = json.LoadFile();
                                this.Report.RegistPropertyChangedEvent();
                                Console.WriteLine("[INFO] 載入檔案：" + Report.CurrentOpenedFile);
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
                    (object obj) => {
                        SaveFileDialog outputReportDialog = new SaveFileDialog
                        {
                            Filter = "Word文件|*.docx",
                            DefaultExt = ".docx",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                            FileName = Path.GetFileNameWithoutExtension(Report.CurrentOpenedFile)
                        };
                        var result = outputReportDialog.ShowDialog();
                        var file = outputReportDialog.FileName;

                        if (result == false)
                            return;

                        MASReport reportFile = new MASReport(Report);
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


        public MainViewModel()
        {
            RuleContent = LoadRuleContents();
            Report = new Report();
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

        public void PicturePanelDragEnter(object sender, DragEventArgs e)
        {
            var file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            var extension = System.IO.Path.GetExtension(file).ToLower();
            if (extension == ".jpg" || extension == ".png")
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

        }

        public void PicturePanelDrop(object sender, DragEventArgs e)
        {
            var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
            CurrentSelectedSubRule.AddPictures(files);
            CurrentSelectedPic = CurrentSelectedSubRule.Pictures.Last();
        }

        private void SaveFile(string file)
        {
            Console.WriteLine("[INFO] 儲存檔案 : " + file);
            JsonFileController json = new JsonFileController(file);
            json.SaveFile(Report);
            Report.CurrentOpenedFile = file;
            Report.MarkAsSaved();
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

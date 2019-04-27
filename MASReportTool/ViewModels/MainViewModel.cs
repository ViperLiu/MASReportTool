using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Report Report { get; private set; }

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
                        IsPicturePanelShown = true;
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
                        Console.WriteLine("Picture Deleted");
                    },
                    () => { return true; }
                    );
            }
        }

        public MainViewModel()
        {
            var ruleContent = LoadRuleContents();
            Report = new Report(ruleContent);
            //CurrentSelectedRule = Report.RuleList["4.1.1.1.2"];
            TreeViewItems = TreeViewItemsViewModel.GetTreeViewItems(Report);
            CurrentSelectedRule = TreeViewItems[0].Items[0].RuleResult;
            Console.WriteLine(CurrentSelectedRule.Content.Title);
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
            Console.WriteLine("extension : " + extension);
            if (extension == ".jpg" || extension == ".png")
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

        }

        public void PicturePanelDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("Drop");
            var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
            CurrentSelectedSubRule.AddPictures(files);
        }
    }
}

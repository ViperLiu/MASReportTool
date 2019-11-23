using Microsoft.Win32;
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
    class TabViewModel : INotifyPropertyChanged
    {
        private Report _Report;
        public Report Report
        {
            get { return _Report; }
            set
            {
                if (value != _Report)
                {
                    _Report = value;
                    TreeViewItems = TreeViewItemsViewModel.GetTreeViewItems(_Report);
                    SetCurrentSelectedRule(TreeViewItems[0].Items[0]);
                    SubRulesList = GetSubRuleList();
                    OnPropertyChanged("Report");
                }
            }
        }

        private List<SubRuleViewModel> _subRulesList;

        public List<SubRuleViewModel> SubRulesList
        {
            get { return _subRulesList; }
            set
            {
                if(value != _subRulesList)
                {
                    _subRulesList = value;
                    Console.WriteLine(_subRulesList.Count);
                    OnPropertyChanged("SubRulesList");
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

        private TreeViewItemsViewModel _currentedTreeviewItem;

        public bool IsClassChangedEnable = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SelectedItemChanged
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var item = obj as TreeViewItemsViewModel;
                        if (item.Level == 0)
                            return;
                        SetCurrentSelectedRule(item);
                        SubRulesList = GetSubRuleList();
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
                        SetCurrentSelectedRule(TreeViewItems[0].Items[0]);
                        SubRulesList = GetSubRuleList();
                        Console.WriteLine("Class Changed");
                    },
                    () => 
                    {
                        if (!IsClassChangedEnable)
                        {
                            this.EnableClassChangedCommand();
                            Console.WriteLine("ClassChangedEnable");
                            return false;
                        }
                        return true;
                    }
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

        public ICommand AddPicture
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {

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
                    (object obj) =>
                    {
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
                    (object obj) =>
                    {
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
                    (object obj) =>
                    {
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
                    (object obj) =>
                    {
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
                    (object obj) =>
                    {
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

        public TabViewModel()
        {
            Report = new Report();
            SubRulesList = GetSubRuleList();
            Report.MarkAsSaved();
        }

        public void DisableClassChangedCommand()
        {
            IsClassChangedEnable = false;
        }

        public void EnableClassChangedCommand()
        {
            IsClassChangedEnable = true;
        }

        private void SetCurrentSelectedRule(TreeViewItemsViewModel treeViewItem)
        {
            CurrentSelectedRule = treeViewItem.RuleResult;
            treeViewItem.IsBorderVisible = true;

            var originalSelectedTreeViewItem = _currentedTreeviewItem;
            _currentedTreeviewItem = treeViewItem;
            
            if (originalSelectedTreeViewItem == null)
                return;
            originalSelectedTreeViewItem.IsBorderVisible = false;
        }

        private List<SubRuleViewModel> GetSubRuleList()
        {
            var subRulesList = new List<SubRuleViewModel>();
            foreach(var subRule in CurrentSelectedRule.SubRuleList)
            {
                var subRuleVM = new SubRuleViewModel(subRule);
                subRuleVM.PictureButtonClicked += PictureButtonClicked;
                subRulesList.Add(subRuleVM);
            }
            return subRulesList;
        }

        private void PictureButtonClicked(object sender, EventArgs e)
        {
            var subRuleVM = sender as SubRuleViewModel;
            var subRule = subRuleVM.SubRule as SubRuleResult;
            CurrentSelectedSubRule = subRule;
            CurrentSelectedPic = subRule.Pictures.Count < 1 ? null : subRule.Pictures[0];
            IsPicturePanelShown = true;
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

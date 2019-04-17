using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MASReportTool
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static Dictionary<string, RuleContents> RuleContents { get; } = LoadRuleContents();

        private static Report Report = new Report(RuleContents);

        private Picture _CurrentSelectedPic;
        public Picture CurrentSelectedPic
        {
            get { return _CurrentSelectedPic; }
            set
            {
                if(value != _CurrentSelectedPic)
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
                if(value != _currentRuleNumber)
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

        public MainWindow()
        {
            InitializeComponent();
            List<TreeViewItems> items = TreeViewItems.GetTreeViewItems(Report);
            trvMenu.ItemsSource = items;
            PictureGrid.DataContext = this;
            NavBar.DataContext = Report;
            ClassSelector.DataContext = Report;

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button Clicked!");
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeview = sender as TreeView;
            var item = treeview.SelectedItem as TreeViewItems;
            if (item.Level != 1)
                return;
            this.CurrentRuleNumber = item.Title;
            this.CurrentSelectedRule = Report.RuleList[item.Title];
            SubRules.ItemsSource = this.CurrentSelectedRule.SubRuleList;
            SubRuleResults.ItemsSource = this.CurrentSelectedRule.SubRuleList;
            Condition.DataContext = this.CurrentSelectedRule;
            CurrentSelectedPic = null;
            
        }

        private void Btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedRule.Accept();
        }

        private void Btn_Fail_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedRule.Fail();
        }

        private void Btn_NotFit_Click(object sender, RoutedEventArgs e)
        {
            CurrentSelectedRule.NotFit();
        }

        private void Rb_SubRule_Accept_Click(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            var index = (int)rb.Tag - 1;
            CurrentSelectedSubRule = CurrentSelectedRule.SubRuleList[index];
            CurrentSelectedSubRule.Accept();
        }

        private void Rb_SubRule_Fail_Click(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            var index = (int)rb.Tag - 1;
            CurrentSelectedSubRule = CurrentSelectedRule.SubRuleList[index];
            CurrentSelectedSubRule.Fail();
        }

        private void Btn_Pictures_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var number = (int)btn.Tag;
            var index = number - 1;
            IsPicturePanelShown = true;
            CurrentSubRuleIndex = index;
            CurrentSelectedSubRule = CurrentSelectedRule.SubRuleList[index];
            PicturesPanel.ItemsSource = CurrentSelectedSubRule.Pictures;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void PicturesPanel_DragEnter(object sender, DragEventArgs e)
        {
            var file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            var extension = System.IO.Path.GetExtension(file).ToLower();
            if (extension == ".jpg" || extension == ".png")
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

        }

        private void PicturesPanel_Drop(object sender, DragEventArgs e)
        {
            var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
            CurrentSelectedSubRule.AddPictures(files);
            PicturesPanel.ItemsSource = CurrentSelectedSubRule.Pictures;
        }

        private void PictureClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsPicturePanelShown = false;
        }

        private void Thumbnail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var thumbnail = sender as Image;
            var index = (int)thumbnail.Tag;
            CurrentSelectedPic = CurrentSelectedSubRule.Pictures[index];
        }

        private void CM_DeletePic_Click(object sender, RoutedEventArgs e)
        {
            var cmItem = sender as MenuItem;
            var index = (int)cmItem.Tag;
            CurrentSelectedSubRule.DeletePicture(index);
        }

        private void AddPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog addPicDialog = new OpenFileDialog();
            addPicDialog.Filter = "圖片|*.png;*.jpg;*.jpeg";
            addPicDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            addPicDialog.Multiselect = true;
            var result = addPicDialog.ShowDialog();
            var files = addPicDialog.FileNames;
            if(result == true)
            {
                CurrentSelectedSubRule.AddPictures(files);
            }
        }

        private void CM_PicMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var cmItem = sender as MenuItem;
            var index = (int)cmItem.Tag;
            CurrentSelectedSubRule.SwapPictures(index, index - 1);
        }

        private void CM_PicMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var cmItem = sender as MenuItem;
            var index = (int)cmItem.Tag;
            CurrentSelectedSubRule.SwapPictures(index, index + 1);
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

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Report.CurrentOpenedFile);
            //如果檔案存在就直接存檔
            if (File.Exists(Report.CurrentOpenedFile))
            {
                SaveFile(Report.CurrentOpenedFile);
                return;
            }

            //檔案不存在就開啟存檔視窗
            SaveAsNewFile();
        }

        private void SaveFile(string file)
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
                SaveFile(file);
            }
        }

        private void Rb_Class_Checked(object sender, RoutedEventArgs e)
        {
            List<TreeViewItems> items = TreeViewItems.GetTreeViewItems(Report);
            trvMenu.ItemsSource = items;
        }
    }
}

﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASReportTool.ViewModels
{
    class TreeViewItemsViewModel : INotifyPropertyChanged
    {
        private bool _isBorderVisible = false;

        public bool IsBorderVisible
        {
            get { return _isBorderVisible; }
            set
            {
                if(value != _isBorderVisible)
                {
                    _isBorderVisible = value;
                    OnPropertyChanged("IsBorderVisible");
                }
            }
        }

        public int Level { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<TreeViewItemsViewModel> Items { get; private set; }
        public RuleResults RuleResult { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static List<TreeViewItemsViewModel> GetTreeViewItems(Report report)
        {
            List<TreeViewItemsViewModel> items = new List<TreeViewItemsViewModel>();
            List<string> parentList = new List<string>();
            foreach (var rule in report.RuleList)
            {
                var ruleResult = rule.Value;
                var ruleNumber = rule.Key;
                var parent = ruleNumber.Remove(ruleNumber.Length - 2);
                var ruleClass = ruleResult.Content.Class;
                if(ruleResult.FinalResult == Result.DontTest)
                    ruleResult.Reset();
                if (!Report.ClassFilter(report.Class, ruleClass))
                {
                    ruleResult.DontTest();
                    continue;
                }
                TreeViewItemsViewModel parentItem = new TreeViewItemsViewModel
                {
                    Title = parent,
                    Level = 0,
                    RuleResult = null,
                    Items = new ObservableCollection<TreeViewItemsViewModel>()
                };
                TreeViewItemsViewModel childItem = new TreeViewItemsViewModel
                {
                    Title = ruleNumber,
                    Level = 1,
                    RuleResult = ruleResult
                };
                if (!parentList.Contains(parent))
                {
                    parentItem.Items.Add(childItem);
                    items.Add(parentItem);
                    parentList.Add(parent);
                }
                else
                {
                    items.Last().Items.Add(childItem);
                }
            }
            return items;
        }

    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASReportTool
{
    class TreeViewItems
    {
        public int Level { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<TreeViewItems> Items { get; }
        public RuleResults RuleResult { get; private set; }

        public static List<TreeViewItems> GetTreeViewItems(Report report)
        {
            List<TreeViewItems> items = new List<TreeViewItems>();
            List<string> parentList = new List<string>();
            foreach (var rule in report.RuleList)
            {
                var ruleResult = rule.Value;
                var ruleNumber = rule.Key;
                var parent = ruleNumber.Remove(ruleNumber.Length - 2);
                var ruleClass = ruleResult.Content.Class;
                if(!Report.ClassFilter(report.Class, ruleClass))
                {
                    continue;
                }
                TreeViewItems parentItem = new TreeViewItems
                {
                    Title = parent,
                    Level = 0,
                    RuleResult = null
                };
                TreeViewItems childItem = new TreeViewItems
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

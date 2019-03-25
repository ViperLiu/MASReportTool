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

        public TreeViewItems()
        {
            this.Items = new ObservableCollection<TreeViewItems>();
        }

        public static List<TreeViewItems> GetTreeViewItems()
        {
            JObject o;

            using (StreamReader reader = File.OpenText("assets\\基準30.json"))
            {
                o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }

            List<TreeViewItems> items = new List<TreeViewItems>();
            List<string> parentList = new List<string>();
            foreach (var rule in o)
            {
                var str = rule.Key;
                var parent = str.Remove(str.Length - 2);
                Console.WriteLine(str);
                Console.WriteLine(parent);
                TreeViewItems parentItem = new TreeViewItems { Title = parent, Level = 0 };
                TreeViewItems childItem = new TreeViewItems { Title = str, Level = 1 };
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
            Console.WriteLine(items.Count);
            return items;
        }
    }
}

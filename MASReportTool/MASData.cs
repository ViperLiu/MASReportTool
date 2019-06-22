using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace MASReportTool
{
    public class MASData
    {
        public static Dictionary<string, RuleContents> RuleContents { get; } = LoadRuleContents();
        public static Report Report = new Report();
        public static List<string> RuleNumberList = new List<string>();
        public static string CurrentSelectRuleNumber;
        public static RuleContents CurrentSelectedRuleContent;
        public static RuleResults CurrentSelectedRuleResult;
        public static string Version = "";
        public static ProjectInfo ProjectInfo = new ProjectInfo();

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

        public static void Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TitleString" || e.PropertyName == "CurrentOpenedFile")
                return;

            //Report.MarkAsNotSaved(ruleNumber);
        }
    }
}

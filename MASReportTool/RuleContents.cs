using System.Collections.Generic;

namespace MASReportTool
{
    public class RuleContents
    {
        public string Title { get; private set; }
        public List<SubRuleContents> SubRuleContentsList { get; private set; }
        public string NotFitText { get; private set; }
        public string PassCondition { get; private set; }
        public int Class { get; private set; }

        public RuleContents(string title, string notFitText, string condition, int ruleClass)
        {
            this.Title = title;
            this.NotFitText = notFitText;
            this.Class = ruleClass;
            this.PassCondition = condition;
            this.SubRuleContentsList = new List<SubRuleContents>();
        }
    }
}

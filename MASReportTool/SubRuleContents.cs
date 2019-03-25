namespace MASReportTool
{
    public class SubRuleContents
    {
        public string Description { get; private set; }
        public string DefaultAcceptText { get; private set; }
        public string DefaultFailText { get; private set; }
        public int SubRuleNumber { get; private set; }
        
        public SubRuleContents(string description, string defaultAcceptText, string defaultFailText, int number)
        {
            this.Description = description;
            this.DefaultAcceptText = defaultAcceptText;
            this.DefaultFailText = defaultFailText;
            this.SubRuleNumber = number;
        }
    }
}

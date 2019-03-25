using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASReportTool
{
    class Rule
    {
        public RuleContents RuleContents { get; private set; }
        public RuleResults RuleResults { get; private set; }

        public Rule(RuleContents contents, RuleResults results)
        {
            this.RuleContents = contents;
            this.RuleResults = results;

        }
    }
}

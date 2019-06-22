using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace MASReportTool
{
    public class RuleResults : INotifyPropertyChanged
    {
        public List<SubRuleResult> SubRuleList { get; private set; }

        private string _finalResult;
        [JsonProperty]
        public string FinalResult
        {
            get
            { return this._finalResult; }
            set
            {
                if (_finalResult != value)

                {
                    this._finalResult = value;
                    OnPropertyChanged("FinalResult");
                }
            }
        }

        [JsonIgnore]
        public RuleContents Content { get; set; }

        public RuleResults(RuleContents content)
        {
            this.Content = content;
            this.FinalResult = "undetermin";
            this.SubRuleList = new List<SubRuleResult>();
        }

        public void Accept()
        {
            this.FinalResult = "accept";
        }

        public void Fail()
        {
            this.FinalResult = "fail";
        }

        public void NotFit()
        {
            this.FinalResult = "notfit";

            for (int i = 0; i < this.SubRuleList.Count; i++)
            {
                this.SubRuleList[i].NotFit();
            }

            this.SubRuleList[0].Text = this.Content.NotFitText;
        }

        public void DontTest()
        {
            foreach(var sub in SubRuleList)
            {
                sub.Reset();
            }
            this.FinalResult = "donttest";
            this.SubRuleList[0].Text = "此項不須檢測";
        }

        public void Reset()
        {
            this.FinalResult = "undetermin";
            foreach (var sub in SubRuleList)
            {
                sub.Reset();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

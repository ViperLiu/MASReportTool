using MASReportTool.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Xceed.Words.NET;

namespace MASReportTool
{
    public class Report : INotifyPropertyChanged
    {
        //讀取APP資訊
        private string _AppCommonName = "";
        public string AppCommonName
        {
            get { return this._AppCommonName; }
            set
            {
                if (_AppCommonName != value)
                {
                    _AppCommonName = value;
                    OnPropertyChanged("AppCommonName");
                }
            }
        }

        private string _AppVersion = "";
        public string AppVersion
        {
            get { return _AppVersion; }
            set
            {
                if (_AppVersion != value)
                {
                    _AppVersion = value;
                    OnPropertyChanged("AppVersion");
                }
            }
        }

        private string _AppOS = "";
        public string AppOS
        {
            get { return _AppOS; }
            set
            {
                if (_AppOS != value)
                {
                    _AppOS = value;
                    OnPropertyChanged("AppOS");
                }
            }
        }

        [JsonIgnore]
        public bool IsAndroid
        {
            get { return _AppOS == "android"; }
            set
            {
                if (value)
                    this.AppOS = "android";
                else
                    this.AppOS = "ios";
                OnPropertyChanged("IsAndroid");
            }
        }
        [JsonIgnore]
        public bool IsiOS
        {
            get { return _AppOS == "ios"; }
            set
            {
                if (value)
                    this.AppOS = "ios";
                else
                    this.AppOS = "android";
                OnPropertyChanged("IsiOS");
            }
        }

        private string _AppId = "";
        public string AppId
        {
            get { return _AppId; }
            set
            {
                if (_AppId != value)
                {
                    _AppId = value;
                    OnPropertyChanged("AppId");
                }
            }
        }

        private string _AppSHA1 = "";
        public string AppSHA1
        {
            get { return _AppSHA1; }
            set
            {
                if (_AppSHA1 != value)
                {
                    _AppSHA1 = value;
                    OnPropertyChanged("AppSHA1");
                }
            }
        }

        private int _Class = 1;
        public int Class
        {
            get { return _Class; }
            set
            {
                if (_Class != value)
                {
                    _Class = value;
                    OnPropertyChanged("Class");
                }
            }
        }

        //送測單位資訊
        private string _CompanyName = "";
        public string CompanyName
        {
            get { return _CompanyName; }
            set
            {
                if (_CompanyName != value)
                {
                    _CompanyName = value;
                    OnPropertyChanged("CompanyName");
                }
            }
        }

        private string _CompanyAddr = "";
        public string CompanyAddr
        {
            get { return _CompanyAddr; }
            set
            {
                if (_CompanyAddr != value)
                {
                    _CompanyAddr = value;
                    OnPropertyChanged("CompanyAddr");
                }
            }
        }

        private string _DeveloperName = "";
        public string DeveloperName
        {
            get { return _DeveloperName; }
            set
            {
                if (_DeveloperName != value)
                {
                    _DeveloperName = value;
                    OnPropertyChanged("DeveloperName");
                }
            }
        }

        //報告資訊
        private string _ReportNo = "";
        public string ReportNo
        {
            get { return _ReportNo; }
            set
            {
                if (_ReportNo != value)
                {
                    _ReportNo = value;
                    OnPropertyChanged("ReportNo");
                }
            }
        }

        private string _StartDate = "";
        public string StartDate
        {
            get { return _StartDate; }
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }

        private string _FinishDate = "";
        public string FinishDate
        {
            get { return _FinishDate; }
            set
            {
                if (_FinishDate != value)
                {
                    _FinishDate = value;
                    OnPropertyChanged("FinishDate");
                }
            }
        }

        private string _ReportDate = "";
        public string ReportDate
        {
            get { return _ReportDate; }
            set
            {
                if (_ReportDate != value)
                {
                    _ReportDate = value;
                    OnPropertyChanged("ReportDate");
                }
            }
        }

        private string _ReportVersion = "";
        public string ReportVersion
        {
            get { return _ReportVersion; }
            set
            {
                if (_ReportVersion != value)
                {
                    _ReportVersion = value;
                    OnPropertyChanged("ReportVersion");
                }
            }
        }

        private string _CurrentOpenedFile;
        [JsonIgnore]
        public string CurrentOpenedFile
        {
            get
            {
                return _CurrentOpenedFile;
            }
            set
            {
                _CurrentOpenedFile = value;
                OnPropertyChanged("CurrentOpenedFile");
            }
        }

        private bool _isSaved = true;
        [JsonIgnore]
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                if(value != _isSaved)
                {
                    _isSaved = value;
                    OnPropertyChanged("IsSaved");
                }
            }
        }

        public Dictionary<string, RuleResults> RuleList;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        [JsonConstructor]
        public Report()
        {
            var contents = MainViewModel.RuleContent;
            this.RuleList = new Dictionary<string, RuleResults>();

            foreach(var rule in contents)
            {
                RuleResults result = new RuleResults(rule.Value);
                result.PropertyChanged += this.Result_PropertyChanged;
                ushort subRuleNumber = 0;
                foreach(var sub in rule.Value.SubRuleContentsList)
                {
                    SubRuleResult subResult = new SubRuleResult(sub);
                    subResult.PropertyChanged += this.SubResult_PropertyChanged;
                    result.SubRuleList.Add(subResult);
                    subRuleNumber++;
                }
                this.RuleList.Add(rule.Key, result);
                
            }

            this.CurrentOpenedFile = "*未儲存的報告.jsonr";
            this.PropertyChanged += this.Report_PropertyChanged;
        }

        public void LoadRuleContents()
        {
            var contents = MainViewModel.RuleContent;
            //this.RuleList = new Dictionary<string, RuleResults>();

            foreach (var rule in contents)
            {
                this.RuleList[rule.Key].Content = rule.Value;
                ushort subRuleNumber = 0;
                foreach (var sub in rule.Value.SubRuleContentsList)
                {
                    this.RuleList[rule.Key].SubRuleList[subRuleNumber].Content = sub;
                    subRuleNumber++;
                }
            }
        }

        public void RegistPropertyChangedEvent()
        {
            this.PropertyChanged += this.Report_PropertyChanged;

            foreach(var rule in RuleList)
            {
                rule.Value.PropertyChanged += Result_PropertyChanged;
                foreach(var sub in rule.Value.SubRuleList)
                {
                    sub.PropertyChanged += SubResult_PropertyChanged;
                }
            }
        }

        public void Reset()
        {
            this.CurrentOpenedFile = "*未儲存的報告.jsonr";
            foreach (var rule in this.RuleList)
            {
                rule.Value.Reset();
            }
        }

        private void Report_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSaved")
                return;
            this.MarkAsNotSaved();
        }

        private void Result_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MarkAsNotSaved();
        }

        private void SubResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MarkAsNotSaved();
        }

        public void MarkAsNotSaved()
        {
            this.IsSaved = false;
        }

        public void MarkAsSaved()
        {
            this.IsSaved = true;
        }

        public void GetDates(string start, string finish, string report)
        {
            this.StartDate = start;
            this.FinishDate = finish;
            this.ReportDate = report;
        }


        /// <summary>
        /// 判定是否需要檢測
        /// </summary>
        /// <param name="targetClass">檢測級別 ( 甲、乙、丙 )</param>
        /// <param name="ruleClass">基準所屬的檢測級別</param>
        /// <returns>true則需要檢測，false則不用</returns>
        public static bool ClassFilter(int targetClass, int ruleClass)
        {
            if (ruleClass == 7)
                return true;

            if (targetClass == 1)
            {
                if (ruleClass == 1 || ruleClass == 3 || ruleClass == 5)
                {
                    return true;
                }
            }
            else if (targetClass == 2)
            {
                if (ruleClass == 2 || ruleClass == 3 || ruleClass == 6)
                {
                    return true;
                }
            }
            else if (targetClass == 3)
            {
                if (ruleClass == 4 || ruleClass == 5 || ruleClass == 6)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

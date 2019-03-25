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

        private int _Class = 0;
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

        private string DocxLocation;
        private DocX TemplateDocx;

        private string _TitleString;
        [JsonIgnore]
        public string TitleString
        {
            get { return _TitleString; }
            set
            {
                if(_TitleString != value)
                {
                    _TitleString = value;
                    OnPropertyChanged("TitleString");
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
                _TitleString = value + string.Format(" - v{0}", MASData.Version);
                _CurrentOpenedFile = value;
                OnPropertyChanged("CurrentOpenedFile");
            }
        }

        public Dictionary<string, RuleResults> RuleList;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Report(Dictionary<string, RuleContents> contents)
        {
            this.RuleList = new Dictionary<string, RuleResults>();

            foreach(var rule in contents)
            {
                RuleResults result = new RuleResults(rule.Value);
                ushort subRuleNumber = 0;
                foreach(var sub in rule.Value.SubRuleContentsList)
                {
                    SubRuleResult subResult = new SubRuleResult(sub);
                    result.SubRuleList.Add(subResult);
                    subRuleNumber++;
                }
                this.RuleList.Add(rule.Key, result);
            }

            this.CurrentOpenedFile = "*未儲存的報告.jsonr";
            this.PropertyChanged += MASData.Changed;
        }

        public void MarkAsNotSaved()
        {
            TitleString = TitleString.Replace("*", "");
            TitleString = "*" + TitleString;
        }

        public void MarkAsSaved()
        {
            TitleString = TitleString.Replace("*", "");
        }

        public void GetDates(string start, string finish, string report)
        {
            this.StartDate = start;
            this.FinishDate = finish;
            this.ReportDate = report;
        }

        public void BuildReport(string buildLocation)
        {
            this.DocxLocation = buildLocation;

            //載入報告樣板
            this.LoadTemplate();

            //將結果寫入docx
            //寫入header
            WriteHeaders();

            //寫入檢測結果總表
            WriteOverView();

            //寫入檢測結果統計
            WriteResultCount();

            //取出跟基準有關的table
            var resultTables = GetResultTables();

            //排除 不須檢測 的測項
            var RuleList = AvailableRuleList();

            for (int i = 0; i < resultTables.Count; i++)
            {
                //寫入佐證資料
                WriteTestData(resultTables[i], RuleList[i]);

                //寫入各基準的檢測結果
                WriteTestResult(resultTables[i], RuleList[i]);
            }

            //儲存成docx檔
            this.Save();
        }

        private void LoadTemplate()
        {
            string templateName = "";
            if (this.Class == 0)
                templateName = "template甲.docx";
            else if (this.Class == 1)
                templateName = "template乙.docx";
            else if (this.Class == 2)
                templateName = "template丙.docx";
            else
                throw new Exception("無法讀取檢測分類");
            try
            {
                this.TemplateDocx = DocX.Load("assets\\" + templateName);
            }
            catch
            {
                throw new Exception("[ERROR] 無法開啟 " + templateName + "，檔案可能已被其他程式開啟\r\n");
            }
        }

        private void Save()
        {
            try
            {
                this.TemplateDocx.SaveAs(this.DocxLocation);
            }
            catch
            {
                throw new Exception("[ERROR] 無法寫入檔案：" + this.DocxLocation + "\r\n");
            }
            finally
            {
                this.TemplateDocx.Dispose();
                this.TemplateDocx = null;
            }
        }

        private void WriteResultCount()
        {
            var failCount = 0;
            var notFitCount = 0;
            var isTestFinished = true;

            foreach (KeyValuePair<string, RuleResults> kvp in MASData.Report.RuleList)
            {
                var rule = kvp.Value;
                if (rule.FinalResult == "fail")
                    failCount++;
                else if (rule.FinalResult == "notfit")
                    notFitCount++;
                else if (rule.FinalResult == "undetermin")
                {
                    isTestFinished = false;
                    this.TemplateDocx.ReplaceText("{ResultCount}", "檢測未完成");
                    return;
                }
            }

            var resultStr = "不符合 " + failCount.ToString() + " 項\r\n不適用 " + notFitCount.ToString() + " 項";
            if (failCount == 0 && isTestFinished)
                resultStr = "符合";
            this.TemplateDocx.ReplaceText("{ResultCount}", resultStr);

        }

        private List<Table> GetResultTables()
        {
            var tbs = this.TemplateDocx.Tables;
            List<Table> targetTables = new List<Table>();
            for (int i = 4; i < tbs.Count; i++)
            {
                targetTables.Add(tbs[i]);
            }
            return targetTables;
        }

        private List<RuleResults> AvailableRuleList()
        {
            List<RuleResults> tmp = new List<RuleResults>();
            foreach (var rule in this.RuleList)
            {
                var r = rule.Value;
                if (r.FinalResult == "donttest")
                    continue;
                tmp.Add(r);
            }

            return tmp;
        }

        private void WriteTestData(Table resultTable, RuleResults currentRule)
        {
            //取得table行數
            int rowNumber = resultTable.RowCount;

            //取得佐證資料欄位的paragraph
            var resultTextPara = resultTable.Rows[rowNumber - 1].Cells[0].Paragraphs[0];

            //將結果文字與圖片寫入到佐證資料欄位的paragraph
            
            foreach(var subRule in currentRule.SubRuleList)
            {
                //輸出Text
                resultTextPara.InsertText(subRule.Text + "\r\n");
                resultTextPara.Alignment = Alignment.left;

                //輸出圖片
                if(subRule.Pictures.Count != 0)
                    InsertPicture(resultTextPara, subRule.Pictures);

                resultTextPara = resultTable.Rows[rowNumber - 1].Cells[0].InsertParagraph();
            }
        }

        private void InsertPicture(Paragraph paragraphToInsert, ObservableCollection<Picture> pictures)
        {
            Table picTable = paragraphToInsert.InsertTableAfterSelf(1, 2);
            picTable.AutoFit = AutoFit.Window;
            picTable.Alignment = Alignment.left;

            foreach(var p in pictures)
            {
                p.CreatePicture();
            }

            var i = 0;
            while(i < pictures.Count)
            {
                Xceed.Words.NET.Image img = this.TemplateDocx.AddImage(pictures[i].FullPath);
                Xceed.Words.NET.Picture p = img.CreatePicture();

                //非手機圖
                if(pictures[i].Ratio > 0.6)
                {
                    var row = picTable.InsertRow();
                    row.MergeCells(0, 1);

                    var row2 = picTable.InsertRow();
                    row2.MergeCells(0, 1);

                    //按照比例縮小圖片，如圖片不足600寬，則保留原大小
                    p.Width = pictures[i].Image.Width < 600 ? pictures[i].Image.Width : 600;
                    p.Height = (int)(p.Width / pictures[i].Ratio);

                    //新增table放圖片
                    row.Cells[0].Paragraphs.First().Append(pictures[i].Caption);
                    row2.Cells[0].Paragraphs.First().InsertPicture(p);

                    if (i == pictures.Count - 1)
                        break;

                    i++;
                }

                //第一張手機圖
                else
                {
                    //最後一張圖
                    if (i == pictures.Count - 1)
                    {
                        var row = picTable.InsertRow();
                        row.MergeCells(0, 1);

                        var row2 = picTable.InsertRow();
                        row2.MergeCells(0, 1);

                        //按照比例縮小圖片，如圖片不足200寬，則保留原大小
                        p.Width = pictures[i].Image.Width < 200 ? pictures[i].Image.Width : 200;
                        p.Height = (int)(p.Width / pictures[i].Ratio);

                        //新增table放圖片
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption);
                        row2.Cells[0].Paragraphs.First().InsertPicture(p);

                        break;
                    }
                        //下一張不是手機圖
                    if (pictures[i + 1].Ratio > 0.6)
                    {
                        var row = picTable.InsertRow();
                        row.MergeCells(0, 1);

                        var row2 = picTable.InsertRow();
                        row2.MergeCells(0, 1);

                        //按照比例縮小圖片，如圖片不足200寬，則保留原大小
                        p.Width = pictures[i].Image.Width < 200 ? pictures[i].Image.Width : 200;
                        p.Height = (int)(p.Width / pictures[i].Ratio);

                        //新增table放圖片
                        row2.Cells[0].Paragraphs.First().InsertPicture(p);
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption);

                        i++;
                    }

                    //下一張也是手機圖
                    else
                    {
                        Xceed.Words.NET.Image img2 = this.TemplateDocx.AddImage(pictures[i + 1].FullPath);
                        Xceed.Words.NET.Picture p2 = img2.CreatePicture();

                        var row = picTable.InsertRow();
                        var row2 = picTable.InsertRow();


                        //按照比例縮小圖片，如圖片不足200寬，則保留原大小
                        p.Width = pictures[i].Image.Width < 200 ? pictures[i].Image.Width : 200;
                        p.Height = (int)(p.Width / pictures[i].Ratio);

                        p2.Width = pictures[i + 1].Image.Width < 200 ? pictures[i + 1].Image.Width : 200;
                        p2.Height = (int)(p.Width / pictures[i + 1].Ratio);

                        //新增table放圖片
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption);
                        row2.Cells[0].Paragraphs.First().InsertPicture(p);

                        row.Cells[1].Paragraphs.First().Append(pictures[i + 1].Caption);
                        row2.Cells[1].Paragraphs.First().InsertPicture(p2);
                        
                        i += 2;
                    }
                }

            }
            picTable.RemoveRow(0);
            picTable.InsertParagraphAfterSelf("\r\n");
        }

        private void WriteTestResult(Table resultTable, RuleResults currentRule)
        {
            for (int j = 0; j < currentRule.SubRuleList.Count; j++)
            {

                //取得目前要編輯的cell
                Paragraph currentPara = resultTable.Rows[j + 1].Cells[0].Paragraphs[0];

                string result = currentRule.SubRuleList[j].Result;
                if (result == "accept")
                {
                    currentPara.Append("符合").Color(Color.Black);
                }
                else if (result == "fail")
                {
                    currentPara.Append("不符合").Color(Color.Red);
                }
                else if (result == "notfit")
                {
                    currentPara.Append("不適用").Color(Color.Blue);
                }
                else
                {
                    currentPara.Append("未檢測").Color(Color.Black);
                }
            }
        }

        private void WriteOverView()
        {
            var overviewTable = this.TemplateDocx.Tables[2];
            for (var i = 1; i < overviewTable.RowCount; i++)
            {
                var row = overviewTable.Rows[i];
                var text = row.Cells[2].Paragraphs[0].Text;
                if (text == "")
                    continue;

                //取得基準編號，及檢測結果
                var ruleNumber = text.Replace("{", "").Replace("}", "");
                var ruleResult = MASData.Report.RuleList[ruleNumber].FinalResult;

                if (ruleResult == "notfit")
                {
                    row.Cells[2].Paragraphs[0].Color(Color.Blue).ReplaceText(text, "不適用");
                }
                else if (ruleResult == "accept")
                {
                    row.Cells[2].Paragraphs[0].Color(Color.Black).ReplaceText(text, "符合");
                }
                else if (ruleResult == "fail")
                {
                    row.Cells[2].Paragraphs[0].Color(Color.Red).ReplaceText(text, "不符合");
                }
                else
                {
                    row.Cells[2].Paragraphs[0].Color(Color.Black).ReplaceText(text, "尚未檢測");
                }
            }
        }

        private void WriteHeaders()
        {
            //送測單位資訊
            this.TemplateDocx.ReplaceText("{CompanyName}", CompanyName);
            this.TemplateDocx.ReplaceText("{CompanyAddr}", CompanyAddr);
            this.TemplateDocx.ReplaceText("{DeveloperName}", DeveloperName);

            //APP資訊
            this.TemplateDocx.ReplaceText("{AppCommonName}", AppCommonName);
            this.TemplateDocx.ReplaceText("{AppId}", AppId);
            this.TemplateDocx.ReplaceText("{AppSHA1}", AppSHA1);
            this.TemplateDocx.ReplaceText("{OS}", AppOS);
            this.TemplateDocx.ReplaceText("{AppVersion}", AppVersion);

            //報告資訊
            this.TemplateDocx.ReplaceText("{ReportNo}", ReportNo);
            this.TemplateDocx.ReplaceText("{StartDate}", ConvertToROCYear(StartDate));
            this.TemplateDocx.ReplaceText("{FinishDate}", ConvertToROCYear(FinishDate));
            this.TemplateDocx.ReplaceText("{ReportDate}", ConvertToROCYear(ReportDate));
            this.TemplateDocx.ReplaceText("{ReportVersion}", ReportVersion);
        }
        
        private string ConvertToROCYear(string date)
        {
            var dateString = date.Split('年');
            var MonthDay = dateString[1];
            var year = int.Parse(dateString[0]);
            var ROCYear = year - 1911;

            StringBuilder newDateString = new StringBuilder();
            newDateString.Append("民國");
            newDateString.Append(ROCYear.ToString());
            newDateString.Append("年");
            newDateString.Append(MonthDay);

            return newDateString.ToString();
        }
    }
}

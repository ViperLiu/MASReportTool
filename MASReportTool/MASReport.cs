using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace MASReportTool
{
    class MASReport
    {
        

        private readonly static string _templateA = "***REMOVED***";
        private readonly static string _templateB = "***REMOVED***";
        private readonly static string _templateC = "***REMOVED***";
        private readonly static string _font = "標楷體";
        private readonly static string ResultTableTitle = "檢測基準";
        private readonly static string OverviewTableTitle = "技術要求";
        private readonly string _templateFile = "assets\\";
        private readonly Report _report;
        private readonly List<Table> _resultTables;
        private readonly Table _overviewTable;
        private readonly List<RuleResults> _ruleList;

        private readonly DocX _templateDocx;

        public MASReport(Report report)
        {
            if (report.Class == 1)
                _templateFile += _templateA;
            else if (report.Class == 2)
                _templateFile += _templateB;
            else if (report.Class == 3)
                _templateFile += _templateC;
            else
                throw new Exception("無法讀取檢測分類");

            try
            {
                _templateDocx = DocX.Load(_templateFile);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("[ERROR] 無法開啟 " + _templateFile + "，檔案可能已被其他程式開啟\r\n");
            }

            _report = report;
            _ruleList = GetAvailableRuleList();
            _resultTables = GetResultTables();
            _overviewTable = GetOverviewTable();
        }

        public void SaveFile(string outputLocation)
        {
            try
            {
                _templateDocx.SaveAs(outputLocation);
            }
            catch
            {
                throw new Exception("[ERROR] 無法寫入檔案：" + outputLocation + "\r\n");
            }
            finally
            {
                _templateDocx.Dispose();
            }
        }

        public void RunTest()
        {
            GetResultTables();
        }

        public void BuildReport(string buildLocation)
        {
            FillOverviewTable();
            FillResultTables();
            SaveFile(buildLocation);
        }

        private void FillOverviewTable()
        {
            for (var i = 1; i < _overviewTable.RowCount; i++)
            {
                var paragraph = _overviewTable.Rows[i].Cells[2].Paragraphs[0];
                var resultStr = _ruleList[i - 1].FinalResult.GetDisplayString();
                var color = _ruleList[i - 1].FinalResult.GetDisplayColor();
                paragraph.Append(resultStr).Font(_font).Color(color);
            }
        }

        private void FillResultTables()
        {
            for (var i = 0; i < _resultTables.Count; i++)
            {
                //寫入佐證資料
                WriteTestData(_resultTables[i], _ruleList[i]);

                //寫入各基準的檢測結果
                WriteTestResult(_resultTables[i], _ruleList[i]);
            }
        }

        private List<RuleResults> GetAvailableRuleList()
        {
            List<RuleResults> tmp = new List<RuleResults>();
            foreach (var rule in _report.RuleList)
            {
                var r = rule.Value;
                if (r.FinalResult == Result.DontTest)
                    continue;
                tmp.Add(r);
            }

            return tmp;
        }

        private List<Table> GetResultTables()
        {
            var tables = _templateDocx.Tables;
            List<Table> targetTables = new List<Table>();
            foreach (var table in tables)
            {
                if (table.Rows[0].Cells[0].Paragraphs.First().Text == ResultTableTitle)
                {
                    targetTables.Add(table);
                }
            }
            Console.WriteLine(targetTables.Count);
            return targetTables;
        }

        private Table GetOverviewTable()
        {
            var tables = _templateDocx.Tables;
            foreach(var table in tables)
            {
                if(table.Rows[0].Cells[1].Paragraphs.First().Text == OverviewTableTitle)
                {
                    return table;
                }
            }
            return null;
        }

        private void WriteTestData(Table resultTable, RuleResults currentRule)
        {
            //取得table行數
            int rowNumber = resultTable.RowCount;

            //取得佐證資料欄位的paragraph
            var resultTextPara = resultTable.Rows[rowNumber - 1].Cells[0].Paragraphs[0];

            //將結果文字與圖片寫入到佐證資料欄位的paragraph

            foreach (var subRule in currentRule.SubRuleList)
            {
                //輸出Text
                resultTextPara.Append(subRule.Text + "\r\n").Font(_font);
                resultTextPara.Alignment = Alignment.left;

                //輸出圖片
                if (subRule.Pictures.Count != 0)
                    InsertPicture(resultTextPara, subRule.Pictures);

                resultTextPara = resultTable.Rows[rowNumber - 1].Cells[0].InsertParagraph();
            }
        }

        private void InsertPicture(Paragraph paragraphToInsert, ObservableCollection<Picture> pictures)
        {
            Table picTable = paragraphToInsert.InsertTableAfterSelf(1, 2);
            picTable.AutoFit = AutoFit.Window;
            picTable.Alignment = Alignment.left;

            foreach (var p in pictures)
            {
                p.CreatePicture();
            }

            var i = 0;
            while (i < pictures.Count)
            {
                Xceed.Words.NET.Image img = _templateDocx.AddImage(pictures[i].FullPath);
                Xceed.Words.NET.Picture p = img.CreatePicture();

                //非手機圖
                if (pictures[i].Ratio > 0.6)
                {
                    var row = picTable.InsertRow();
                    row.MergeCells(0, 1);

                    var row2 = picTable.InsertRow();
                    row2.MergeCells(0, 1);

                    //按照比例縮小圖片，如圖片不足600寬，則保留原大小
                    p.Width = pictures[i].Image.Width < 600 ? pictures[i].Image.Width : 600;
                    p.Height = (int)(p.Width / pictures[i].Ratio);

                    //新增table放圖片
                    row.Cells[0].Paragraphs.First().Append(pictures[i].Caption).Font(_font);
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
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption).Font(_font);
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
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption).Font(_font);

                        i++;
                    }

                    //下一張也是手機圖
                    else
                    {
                        Xceed.Words.NET.Image img2 = _templateDocx.AddImage(pictures[i + 1].FullPath);
                        Xceed.Words.NET.Picture p2 = img2.CreatePicture();

                        var row = picTable.InsertRow();
                        var row2 = picTable.InsertRow();


                        //按照比例縮小圖片，如圖片不足200寬，則保留原大小
                        p.Width = pictures[i].Image.Width < 200 ? pictures[i].Image.Width : 200;
                        p.Height = (int)(p.Width / pictures[i].Ratio);

                        p2.Width = pictures[i + 1].Image.Width < 200 ? pictures[i + 1].Image.Width : 200;
                        p2.Height = (int)(p.Width / pictures[i + 1].Ratio);

                        //新增table放圖片
                        row.Cells[0].Paragraphs.First().Append(pictures[i].Caption).Font(_font);
                        row2.Cells[0].Paragraphs.First().InsertPicture(p);

                        row.Cells[1].Paragraphs.First().Append(pictures[i + 1].Caption).Font(_font);
                        row2.Cells[1].Paragraphs.First().InsertPicture(p2);

                        i += 2;
                    }
                }

            }
            picTable.RemoveRow(0);
            picTable.InsertParagraphAfterSelf("\r\n");

            foreach (var p in pictures)
            {
                p.ReleasePictureResource();
            }
        }

        private void WriteTestResult(Table resultTable, RuleResults currentRule)
        {
            for (int j = 0; j < currentRule.SubRuleList.Count; j++)
            {

                //取得目前要編輯的cell
                Paragraph currentPara = resultTable.Rows[j + 1].Cells[0].Paragraphs[0];

                var result = currentRule.SubRuleList[j].Result.GetDisplayString();
                var color = currentRule.SubRuleList[j].Result.GetDisplayColor();
                currentPara.Append(result).Color(color).Font(_font);
            }
        }
    }
}

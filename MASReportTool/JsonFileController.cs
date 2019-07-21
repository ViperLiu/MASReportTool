using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using MASReportTool.ViewModels;

namespace MASReportTool
{
    public class JsonFileController
    {
        public readonly string FilePath;

        public JsonFileController(string path)
        {
            this.FilePath = path;
        }

        public void SaveFile(object report)
        {
            string output = JsonConvert.SerializeObject(report, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            });
            File.WriteAllText(FilePath, output);
        }

        public Report LoadFile()
        {
            Report report;
            using (StreamReader reader = File.OpenText(this.FilePath))
            {
                var jsonStr = reader.ReadToEnd();
                try
                {
                    report = JsonConvert.DeserializeObject<Report>(jsonStr, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    });
                }
                catch
                {
                    Console.WriteLine("偵測到舊版本的 .jsonr 存檔");
                    report = LoadOldFile();
                }
                report.LoadRuleContents();
                report.CurrentOpenedFile = FilePath;
                return report;
            }
        }

        public Report LoadOldFile()
        {
            Report report = new Report();

            JObject jObject;
            using (StreamReader reader = File.OpenText(this.FilePath))
            {
                jObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }

            report.Class = (int)jObject["Class"];

            var ruleList = jObject["RuleList"].ToObject<JObject>();
            foreach (var rule in ruleList)
            {
                var ruleContent = MainViewModel.RuleContent[rule.Key];
                var subRuleContentList = ruleContent.SubRuleContentsList;

                RuleResults ruleResult = new RuleResults(ruleContent)
                {
                    FinalResult = ResultFactory.CreateResult(rule.Value["FinalResult"].ToString())
                };

                var SubRuleContentIndex = 0;
                foreach(JObject sub in rule.Value["SubRuleList"])
                {
                    JArray pictureList = (JArray)sub["Pictures"];

                    SubRuleResult subRule = new SubRuleResult(subRuleContentList[SubRuleContentIndex])
                    {
                        Result = ResultFactory.CreateResult(sub["Result"].ToString()),
                        Text = sub["Text"].ToString()
                    };

                    for (var i = 0; i < pictureList.Count; i++)
                    {
                        Picture pic = new Picture(
                            i, 
                            pictureList[i]["FullPath"].ToString(), 
                            pictureList[i]["Caption"].ToString()
                            );
                        subRule.Pictures.Add(pic);
                    }
                    ruleResult.SubRuleList.Add(subRule);
                    SubRuleContentIndex++;
                }
                report.RuleList[rule.Key] = ruleResult;
            }

            return report;
        }

        public static JObject LoadJsonFile(string path)
        {
            //load rule data from json file
            JObject o;

            using (StreamReader reader = File.OpenText(path))
            {
                o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }

            return o;
        }

        private class ResultFactory
        {
            public static ITestResult CreateResult(string str)
            {
                switch (str)
                {
                    case "accept":
                        return Result.Accept;
                    case "fail":
                        return Result.Fail;
                    case "notfit":
                        return Result.Notfit;
                    case "undetermin":
                        return Result.Default;
                    case "donttest":
                        return Result.DontTest;
                }
                return Result.Default;
            }
        }
    }
}

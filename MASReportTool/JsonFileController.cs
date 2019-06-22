using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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
            string output = JsonConvert.SerializeObject(report);
            File.WriteAllText(FilePath, output);
        }

        public Report LoadFile()
        {
            using (StreamReader reader = File.OpenText(this.FilePath))
            {
                var jsonStr = reader.ReadToEnd();
                var report = JsonConvert.DeserializeObject<Report>(jsonStr);
                report.LoadRuleContents();
                report.CurrentOpenedFile = FilePath;
                return report;
            }
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
    }
}

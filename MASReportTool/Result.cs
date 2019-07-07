using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASReportTool
{
    public interface ITestResult
    {
        string GetDisplayString();

        Color GetDisplayColor();

        string GetIconPath();
    }

    public static class Result
    {
        public static readonly ResultAccept Accept = new ResultAccept();

        public static readonly ResultFail Fail = new ResultFail();

        public static readonly ResultNotfit Notfit = new ResultNotfit();

        public static readonly ResultDefault Default = new ResultDefault();

        public static readonly ResultDontTest DontTest = new ResultDontTest();
    }

    public class ResultAccept : ITestResult
    {
        public Color GetDisplayColor()
        {
            return Color.Black;
        }

        public string GetDisplayString()
        {
            return "符合";
        }

        public string GetIconPath()
        {
            return "/assets/pics/icon_accept.png";
        }
    }

    public class ResultFail : ITestResult
    {
        public Color GetDisplayColor()
        {
            return Color.Red;
        }

        public string GetDisplayString()
        {
            return "不符合";
        }

        public string GetIconPath()
        {
            return "/assets/pics/icon_fail.png";
        }
    }

    public class ResultNotfit : ITestResult
    {
        public Color GetDisplayColor()
        {
            return Color.Blue;
        }

        public string GetDisplayString()
        {
            return "不適用";
        }

        public string GetIconPath()
        {
            return "/assets/pics/icon_notfit.png";
        }
    }

    public class ResultDefault : ITestResult
    {
        public Color GetDisplayColor()
        {
            return Color.Black;
        }

        public string GetDisplayString()
        {
            return "未檢測";
        }

        public string GetIconPath()
        {
            return "";
        }
    }

    public class ResultDontTest : ITestResult
    {
        public Color GetDisplayColor()
        {
            return Color.Black;
        }

        public string GetDisplayString()
        {
            return "不須檢測";
        }

        public string GetIconPath()
        {
            return "";
        }
    }
}

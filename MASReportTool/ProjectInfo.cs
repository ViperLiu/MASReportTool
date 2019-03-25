using System.Text;

namespace MASReportTool
{
    public enum ProjectPath
    {
        Application,
        PicturesFolder,
        Jsonr,
        Installer
    }
    public class ProjectInfo
    {
        public int Mode;
        public string OS;
        public int Class;
        public string ProjectFolder = "";
        public readonly string PicturesFolder = "pictures";
        public string ApplicationPath = "";
        public string InstallerPath = "";
        public string JsonrPath = "";
        public string InstallerSHA1 = "";
        public string GetFullPath(ProjectPath target)
        {
            if (this.PicturesFolder == null)
                return null;
            StringBuilder result = new StringBuilder(this.ProjectFolder + "\\");
            switch (target)
            {
                default:
                    return null;
                case ProjectPath.Application:
                    if (this.ApplicationPath == null)
                        return null;
                    result.Append(this.ApplicationPath);
                    break;
                case ProjectPath.PicturesFolder:
                    result.Append(this.PicturesFolder);
                    break;
                case ProjectPath.Installer:
                    if (this.InstallerPath == null)
                        return null;
                    result.Append(this.InstallerPath);
                    break;
                case ProjectPath.Jsonr:
                    if (this.JsonrPath == null)
                        return null;
                    result.Append(this.JsonrPath);
                    break;
            }

            return result.ToString();
        }
    }
}

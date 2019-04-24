using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MASReportTool.ViewModels
{
    class PicturePageViewModel
    {
        public SubRuleResult SubRule { get; }

        public Picture CurrentSelectedPic { get; }

        public List<PictureViewModel> Pictures { get; }

        public PicturePageViewModel(SubRuleResult subRule)
        {
            SubRule = subRule;
            Pictures = new List<PictureViewModel>();
            foreach(var pic in subRule.Pictures)
            {
                Pictures.Add(new PictureViewModel(pic));
            }
        }

        
    }
}

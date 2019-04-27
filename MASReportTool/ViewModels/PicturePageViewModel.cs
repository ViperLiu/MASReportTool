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

        public Picture CurrentSelectedPic { get; set; }

        public ICommand ThumbnailClicked
        {
            get { return new RelayCommand(ThumbnailClickedExecute, CanThumbnailClickedExecute); }
        }

        public PicturePageViewModel(SubRuleResult subRule)
        {
            SubRule = subRule;
        }

        private bool CanThumbnailClickedExecute()
        {
            return true;
        }

        private void ThumbnailClickedExecute(object parameter)
        {
            Console.WriteLine("fuck you");
        }

    }
}

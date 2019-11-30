using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MASReportTool.ViewModels
{
    class PictureViewModel
    {
        public Picture Picture { get; }

        public ICommand Clicked
        {
            get { return new RelayCommand(ClickedExecute, CanClickedExecute); }
        }

        public PictureViewModel(Picture picture)
        {
            Picture = picture;
        }

        private bool CanClickedExecute()
        {
            return true;
        }

        private void ClickedExecute(object parameter)
        {
            Console.WriteLine(parameter);
        }
    }
}

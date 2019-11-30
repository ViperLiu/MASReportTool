using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MASReportTool.ViewModels
{
    class SubRuleViewModel : INotifyPropertyChanged
    {
        private SubRuleResult _subRule;

        public SubRuleResult SubRule
        {
            get { return _subRule; }
            set
            {
                if(value != _subRule)
                {
                    _subRule = value;
                    OnPropertyChanged("SubRule");
                }
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if(value != _description)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        private int _subRuleNumber;

        public int SubRuleNumber
        {
            get { return _subRuleNumber; }
            set
            {
                if (value != _subRuleNumber)
                {
                    _subRuleNumber = value;
                    OnPropertyChanged("SubRuleNumber");
                }
            }
        }


        public SubRuleViewModel(SubRuleResult subRule)
        {
            SubRule = subRule;
            Description = subRule.Content.Description;
            SubRuleNumber = subRule.Content.SubRuleNumber;
        }

        public void SetSubRule(SubRuleResult subRule)
        {
            SubRule = subRule;
            Description = subRule.Content.Description;
            SubRuleNumber = subRule.Content.SubRuleNumber;
        }

        public ICommand SubRuleResultChanged
        {
            get
            {
                return new RelayCommand(
                    (object obj) => {
                        var rb = obj as RadioButton;
                        if (rb.IsChecked == false)
                            return;
                        var btnStr = rb.Content.ToString();
                        //var subRule = rb.Tag as SubRuleResult;
                        if (btnStr == "符合")
                            SubRule.Accept();
                        else if (btnStr == "不符合")
                            SubRule.Fail();
                    },
                    () => { return true; }
                    );
            }
        }

        public ICommand PictureButtonClick
        {
            get
            {
                return new RelayCommand(
                    (object obj) =>
                    {
                        OnPictureButtonClicked();
                    },
                    () => { return true; }
                    );
            }
        }

        public event EventHandler PictureButtonClicked;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPictureButtonClicked()
        {
            PictureButtonClicked?.Invoke(this, new EventArgs());
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

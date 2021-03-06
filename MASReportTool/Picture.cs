﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace MASReportTool
{
    public class Picture : INotifyPropertyChanged
    {
        [JsonIgnore]
        private string _FullPath;
        public string FullPath
        {
            get { return _FullPath; }
            set
            {
                if(value != _FullPath)
                {
                    _FullPath = value;
                    OnPropertyChanged("FullPath");
                }
            }
        }

        public readonly string Path;
        private string _Caption;
        public string Caption
        {
            get { return _Caption; }
            private set
            {
                if(_Caption != value)
                {
                    _Caption = value;
                    OnPropertyChanged("Caption");
                }
            }
        }

        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if(value != _Index)
                {
                    _Index = value;
                    OnPropertyChanged("Index");
                }
            }
        }
        [JsonIgnore]
        public Image Image { get; private set; }

        public double Ratio { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public ICommand Clicked
        {
            get { return new RelayCommand(ClickedExecute, CanClickedExecute); }
        }

        public Picture(int index, string path, string caption = "")
        {
            this.Index = index;
            this.FullPath = path;
            this.Caption = caption;
            this.Ratio = -1;
        }

        private bool CanClickedExecute()
        {
            return true;
        }

        private void ClickedExecute(object parameter)
        {
            Console.WriteLine(parameter);
        }

        public void AddCaption(string text)
        {
            this.Caption = text;
        }

        public void CreatePicture()
        {
            this.Image = System.Drawing.Image.FromFile(this.FullPath);
            this.Ratio = (double)this.Image.Width / (double)this.Image.Height;
        }

        public void ReleasePictureResource()
        {
            this.Image.Dispose();
            this.Ratio = -1;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

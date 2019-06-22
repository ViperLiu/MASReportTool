using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MASReportTool
{
    public class SubRuleResult : INotifyPropertyChanged
    {
        private string _Result;
        public string Result
        {
            get { return _Result; }
            set
            {
                if (_Result != value)
                {
                    _Result = value;
                    OnPropertyChanged("Result");
                }
            }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        [JsonIgnore]
        public SubRuleContents Content { get; set; }

        public ObservableCollection<Picture> Pictures {
            get;
            private set;
        }
        

        public SubRuleResult(SubRuleContents content)
        {
            this.Content = content;
            this.Result = "undetermin";
            this.Text = "";
            this.Pictures = new ObservableCollection<Picture>();
            this.Pictures.CollectionChanged += Pictures_CollectionChanged;
        }

        private void Pictures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Pictures");
        }

        public void Accept()
        {
            this.Result = "accept";
            this.Text = this.Content.DefaultAcceptText;
        }

        public void Fail()
        {
            this.Result = "fail";
            this.Text = this.Content.DefaultFailText;
        }

        public void NotFit()
        {
            this.Result = "notfit";
            this.Text = "";
        }

        public void Reset()
        {
            this.Result = "undetermin";
            this.Text = "";
            this.Pictures.Clear();
        }

        public void UpdatePicturesIndex()
        {
            for(var i = 0; i < this.Pictures.Count; i++)
            {
                this.Pictures[i].Index = i;
            }
            OnPropertyChanged("Pictures");
        }

        public void DeletePicture(int targetIndex)
        {
            this.Pictures.RemoveAt(targetIndex);
            this.UpdatePicturesIndex();
            OnPropertyChanged("Pictures");
        }

        public void SwapPictures(int indexA, int indexB)
        {
            try
            {
                var tmp = this.Pictures[indexA];
                this.Pictures[indexA] = this.Pictures[indexB];
                this.Pictures[indexB] = tmp;

                this.Pictures[indexA].Index = indexA;
                this.Pictures[indexB].Index = indexB;
            }
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("asdf");
            }
            OnPropertyChanged("Pictures");
        }

        public void AddPictures(string[] files)
        {
            for(int filesIndex = 0, startIndex = this.Pictures.Count; filesIndex < files.Length; filesIndex++, startIndex++)
            {
                var extension = System.IO.Path.GetExtension(files[filesIndex]).ToLower();
                var extensionNotAllowed = extension != ".jpg" && extension != ".jpeg" && extension != ".png";
                if (extensionNotAllowed)
                    continue;

                var picture = new Picture(startIndex, files[filesIndex]);
                picture.PropertyChanged += this.Picture_PropertyChanged;

                this.Pictures.Add(picture);
            }
        }

        private void Picture_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Pictures");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

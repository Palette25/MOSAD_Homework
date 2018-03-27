using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace App1.Models
{
    public class Item:INotifyPropertyChanged
      {
        private string p_title;
        private bool completed;
        private Visibility Image_visi;

        public string title {
            get { return p_title; }
            set
            {
                this.p_title = value;
                NotifyPropertyChanged("title");
            }
        }
        public string content {
            set;get;
        }
        public ImageSource source {
            set;get;
        }
        public DateTimeOffset date { set; get; }
        public int id { set; get; }
        public bool isdealed{
            get { return completed; }
            set
            {
                completed = value;
                NotifyPropertyChanged("isdealed");
            }
        }
        public Visibility ImageVisi
        {
            get { return Image_visi; }
            set
            {
                this.Image_visi = value;
                NotifyPropertyChanged("ImageVisi");
            }
        }

        public Item(string title, string content, ImageSource source, DateTimeOffset date, int id)
        {
            this.title = title;
            this.content = content;
            this.source = source;
            this.date = date;
            this.id = id;
            this.isdealed = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
 }
using SQLite.Net.Attributes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.ServiceModel;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace App1.Models
{
    [DataContract]
    public class Item: INotifyPropertyChanged
      {
        [DataMember]
        private string p_title;
        [DataMember]
        private bool completed;
        [DataMember]
        private Visibility Image_visi;
        [DataMember]
        public string title {
            get { return p_title; }
            set
            {
                this.p_title = value;
                NotifyPropertyChanged("title");
            }
        }
        [DataMember]
        public string content {
            set;get;
        }
        [DataMember]
        private string t_source;
        [DataMember]
        public string source {
            get { return t_source; }
            set
            {
                t_source = value;
                NotifyPropertyChanged("source");
            }
        }
        [DataMember]
        public string date { set; get; }

        [PrimaryKey]
        [DataMember]
        public int id { set; get; }
        [DataMember]
        public bool isdealed{
            get { return completed; }
            set
            {
                completed = value;
                NotifyPropertyChanged("isdealed");
            }
        }
        [DataMember]
        public Visibility ImageVisi
        {
            get { return Image_visi; }
            set
            {
                this.Image_visi = value;
                NotifyPropertyChanged("ImageVisi");
            }
        }

        public Item(string title, string content, string source, string date, int id)
        {
            this.title = title;
            this.content = content;
            this.source = source;
            this.date = date;
            this.id = id;
            this.isdealed = false;
        }

        public Item()
        {
            
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
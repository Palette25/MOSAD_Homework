using App1.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace App1.ViewModels
{
    public class AllItems
    {
        private ObservableCollection<Models.Item> all_items = new ObservableCollection<Models.Item>();
        private int count = 0;
        public ObservableCollection<Models.Item> ItemStore { get{ return this.all_items; } }

        private Models.Item SelectItem(int id)
        {
            Models.Item sel_item = null;
            for(int i = 0; i < all_items.Count; i++)
            {
                if (all_items[i].id == id)
                {
                    sel_item = all_items[i];
                }
            }
            return sel_item;
        }

        public void AddItem(string text, string content, ImageSource source, DateTimeOffset date)
        {
            all_items.Add(new Models.Item(text, content, source, date, count++));
        }

        public void DeleteItem(int id_)
        {
            Models.Item del_item = SelectItem(id_);
            all_items.Remove(del_item);
            
        }

        public void UpdateItem(string title_, string content_, ImageSource source_, DateTimeOffset date_, int id)
        {
            Models.Item up_item = SelectItem(id);
            up_item.title = title_;
            up_item.content = content_;
            up_item.source = source_;
            up_item.date = date_;
            Debug.WriteLine(title_);
        }

        public void CompleteItem(int id_)
        {
            Models.Item com_item = SelectItem(id_);
            com_item.isdealed = true;
        }

        public Item Select_with_title(string title)
        {
            Item sel_item = null;
            for (int i = 0; i < all_items.Count; i++)
            {
                if (all_items[i].title == title)
                {
                    sel_item = all_items[i];
                }
            }
            return sel_item;
        }

    }
    
    public class bool_to_null : IValueConverter
    {
        public object Convert(object value, Type targetType, object para, string culture)
        {
            return (bool)value;
        }

        public object ConvertBack(object value, Type targetType, object para, string culture)
        {
            return (bool)value;
        }
    }

    public class bool_to_visi : IValueConverter
    {
        public object Convert(object value, Type targetType, object para, string culture)
        {
            bool isdealed = (bool)value;
            var result = isdealed ? Visibility.Visible : Visibility.Collapsed;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object para, string culture)
        {
            Visibility visi = (Visibility)value;
            var result = visi == Visibility.Visible ? true : false;
            return result;
        }

    }

    public class MainViewModel
    {
        public static AllItems store = new AllItems();
        public static AllItems GetAllItems()
        {
            return store;
        }
    }

}
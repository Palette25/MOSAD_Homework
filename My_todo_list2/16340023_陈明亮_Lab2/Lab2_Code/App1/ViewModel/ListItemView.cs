using App1.Models;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App1.ViewModels
{
    public class AllItems
    {
        private static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, App.db_path);
        //Get SQLite Database connection
        internal SQLiteConnection GetConn()
        {
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), path);
            return conn;
        }

        private ObservableCollection<Models.Item> all_items = new ObservableCollection<Models.Item>();
        public static int count = 0;
        public ObservableCollection<Models.Item> ItemStore { get{ return this.all_items; } }

        public AllItems()
        {
            using(var conn = GetConn())
            {
                var exist = conn.GetTableInfo("Item");

                if (exist.Count == 0)
                {
                    conn.CreateTable<Item>();
                }
                else
                {
                    var item_list = conn.Table<Item>();
                    foreach (var item in item_list)
                    {
                        all_items.Add(item);
                        Debug.WriteLine(item.isdealed);
                        count++;
                    }
                }
                conn.Close();
            }
        }

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

        public void AddItem(string text, string content, string source, string date)
        {
            using(var conn = GetConn())
            {
                Item new_item = new Models.Item(text, content, source, date, count++);
                all_items.Add(new_item);
                conn.Insert(new_item);
                conn.Close();
            }
        }

        public void DeleteItem(int id_)
        {
            using(var conn = GetConn())
            {
                Models.Item del_item = SelectItem(id_);
                all_items.Remove(del_item);
                conn.Execute("delete from Item where id = ?", del_item.id);
                conn.Close();
                count--;
            }
        }

        public void UpdateItem(string title_, string content_, string source_, string date_, int id)
        {
            Models.Item up_item = SelectItem(id);
            up_item.title = title_;
            up_item.content = content_;
            up_item.source = source_;
            up_item.date = date_;
            using(var conn = GetConn())
            {
                conn.Update(up_item);
                conn.Close();
            }
        }

        public void CompleteItem(int id)
        {
            using(var conn = GetConn())
            {
                var sel_item = SelectItem(id);
                conn.Update(sel_item);
                conn.Close();
            }
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
            bool result = new bool();
            result = visi == Visibility.Visible ? true : false;
            return result;
        }

    }

    public class str_to_image : IValueConverter
    {
        public object Convert(object value, Type targetType, object para, string culture)
        {
            BitmapImage src = new BitmapImage();
            setSource(src, value);
            return src;
        }

        public object ConvertBack(object value, Type targetType, object para, string culture)
        {
            return null;
        }

        private async void setSource(BitmapImage src, object value)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(value.ToString());
            using(IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                await src.SetSourceAsync(stream);
            }
        }
    }

    public static class MainViewModel
    {
        public static AllItems store = new AllItems();

        public static AllItems GetAllItems()
        {
            return store;
        }
    }

}
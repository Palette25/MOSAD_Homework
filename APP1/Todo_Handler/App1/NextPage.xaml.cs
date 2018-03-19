using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Win32;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using static App1.MainPage;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NextPage : Page
    {
        public int num = 0;
        public NextPage()
        {
            this.InitializeComponent();
        }

        private async void on_create(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bool mode = false; ;
            if ((string)btn.Content=="Save")
            {
                mode = true;
            }
            String error_message = "";
            if (title.Text == "")
            {
                error_message += "Title cannot be empty!\n";
            }
            if (content.Text == "")
            {
                error_message += "Content cannot be empty!\n";
            }
            if(Smaller(date))
            {
                error_message += "The date has already passed, please change another date!\n";
            }
            if(error_message == "")
            {
                if (mode) error_message = "Save successfully!";
                else error_message = "Create successfully!";
            }
            var cd = new ContentDialog()
            {
                Content = error_message,
                PrimaryButtonText = "OK",
            };
            cd.PrimaryButtonClick += (s_, e_) => { };
            await cd.ShowAsync();
            if (error_message == "Create successfully!")
            {
                Item new_item = new Item(title.Text, content.Text, todo_img.Source, date.Date, ItemStore.num);
                ItemStore.num++;
                ItemStore.store.Add(new_item);
                Frame.GoBack();
            }else if(error_message == "Save successfully!")
            {
                for(int i = 0; i < ItemStore.store.Count; i++)
                {
                    Item item = (Item)ItemStore.store[i];
                    if (item.id == num)
                    {
                        item.text = title.Text;
                        item.content = content.Text;
                        item.source = todo_img.Source;
                        item.date = date.Date;
                    }
                }
                Frame.GoBack();
            }
        }

        private void on_cancel(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            content.Text = "";
            date.Date = DateTime.Now;
        }

        private bool Smaller(DatePicker date)
        {
            int res = date.Date.CompareTo(DateTime.Now);
            if (res == -1)
            {
                if (date.Date.Year==DateTime.Now.Year&&date.Date.Month==DateTime.Now.Month&&date.Date.Day==DateTime.Now.Day)
                    return false;
                else return true;
            }
            else return false;
        }

        private void btn2_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "");
        }

        private async void img_click(object sender, RoutedEventArgs e)
        {
            var srcImage = new BitmapImage();
            FileOpenPicker file = new FileOpenPicker();
            file.FileTypeFilter.Add(".jpg");
            file.FileTypeFilter.Add(".png");
            file.FileTypeFilter.Add(".jpeg");
            Windows.Storage.StorageFile result = await file.PickSingleFileAsync();
            if (result != null)
            {
                using(IRandomAccessStream stream = await result.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    todo_img.Source = srcImage;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sli = sender as Slider;
            if(sli != null)
            {
                todo_img.Height = 200 + sli.Value*0.3;
                todo_img.Width = 200 + sli.Value*0.3;
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != "")
            {
                Item old = (Item)e.Parameter;
                create.Content = "Save";
                title.Text = old.text;
                content.Text = old.content;
                date.Date = old.date;
                todo_img.Source = old.source;
                num = old.id;
            }
        }
    }
   
}

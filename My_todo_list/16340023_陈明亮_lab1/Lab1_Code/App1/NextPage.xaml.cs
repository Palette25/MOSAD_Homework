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
using App1.Models;
using App1.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NextPage : Page
    {
        private int edit_id = -1; //In editing mode, store the id of item
        public NextPage()
        {
            this.InitializeComponent();
        }

        private async void on_create(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bool mode = false;//Ture for edit mode, false for create mode
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
            await cd.ShowAsync();
            if (error_message == "Create successfully!")
            {
                MainViewModel.store.AddItem(title.Text, content.Text, todo_img.ImageSource, date.Date);
                Frame.GoBack();
            }else if(error_message == "Save successfully!")
            {
                MainViewModel.store.UpdateItem(title.Text, content.Text, todo_img.ImageSource, date.Date, edit_id);
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
                    todo_img.ImageSource = srcImage;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sli = sender as Slider;
            if(sli != null)
            {
                eimg.Height = 180 + sli.Value*0.3;
                eimg.Width = 180 + sli.Value*0.3;
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != "")
            {
                Item old = (Item)e.Parameter;
                delete.Visibility = Visibility.Visible;
                create.Content = "Save";
                title.Text = old.title;
                content.Text = old.content;
                date.Date = old.date;
                todo_img.ImageSource = old.source;
                edit_id = old.id;
            }
        }

        private async void delete_item(object sender, RoutedEventArgs e)
        {
            MainViewModel.store.DeleteItem(edit_id);
            var cd = new ContentDialog
            {
                Content = "Delete successfully!",
                PrimaryButtonText = "OK"
            };
            await cd.ShowAsync();
            Frame.GoBack();
        }
    }
   
}

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
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NextPage : Page
    {
        private int edit_id = -1; //In editing mode, store the id of item
        StorageFolder root = ApplicationData.Current.LocalFolder;
        private string img_name = "-1.jpg";
        private string source_img = "-1.jpg";

        public NextPage()
        {
            this.InitializeComponent();
            changeBackImg();
        }

        public void changeBackImg()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/green-blue.jpg", UriKind.Absolute));
            grid2.Background = imageBrush;
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
            if (img_name == "-1.jpg")
            {
                img_name = (mode ? edit_id : AllItems.count) + ".jpg";
                StorageFile newfile = await root.CreateFileAsync(img_name, CreationCollisionOption.ReplaceExisting);
                StorageFile default_file = await root.GetFileAsync(source_img);
                using (IRandomAccessStream stream = await default_file.OpenAsync(FileAccessMode.Read))
                {
                    DataReader reader = new DataReader(stream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] img_byte = new byte[stream.Size];
                    reader.ReadBytes(img_byte);
                    await FileIO.WriteBytesAsync(newfile, img_byte);
                }
            }
            if (error_message == "Create successfully!")
            {
                MainViewModel.store.AddItem(title.Text, content.Text, img_name, date.Date.ToString());
                Frame.GoBack();
            }else if(error_message == "Save successfully!")
            {
                MainViewModel.store.UpdateItem(title.Text, content.Text, img_name, date.Date.ToString(), edit_id);
                Frame.GoBack();
            }
            img_name = "-1.jpg";
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
                    StorageFolder root = ApplicationData.Current.LocalFolder;
                    Debug.WriteLine(root.Path);
                    bool edit_or_create = create.Content == "Save" ? true : false;
                    string file_name = (edit_or_create ? edit_id : AllItems.count + 1) + result.FileType;
                    img_name = file_name;
                    source_img = file_name;
                    StorageFile newfile = await root.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);
                    DataReader reader = new DataReader(stream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] img_byte = new byte[stream.Size];
                    reader.ReadBytes(img_byte);
                    await FileIO.WriteBytesAsync(newfile, img_byte);
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
                string json_pass = e.Parameter.ToString();
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json_pass)))
                {
                    DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(Item));
                    Item old = (Item)ds.ReadObject(ms);
                    delete.Visibility = Visibility.Visible;
                    create.Content = "Save";
                    title.Text = old.title;
                    content.Text = old.content;
                    date.Date = Convert.ToDateTime(old.date);
                    BitmapImage bit = new BitmapImage();
                    Convert_str_to_img(old.source, bit);
                    todo_img.ImageSource = bit;
                    img_name = old.source;
                    edit_id = old.id;
                }
                
            }
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("nextpage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("nextpage"))
                {
                    var temp = ApplicationData.Current.LocalSettings.Values["nextpage"] as ApplicationDataCompositeValue;
                    title.Text = (string)temp["title"];
                    content.Text = (string)temp["content"];
                    date.Date = Convert.ToDateTime(temp["date"]);
                    img_name = (string)temp["img_name"];
                    edit_id = (int)temp["edit_id"];
                    create.Content = (bool)temp["mode"] ? "Save" : "Create";
                    BitmapImage bit = new BitmapImage();
                    Convert_str_to_img(img_name, bit);
                    todo_img.ImageSource = bit;
                    ApplicationData.Current.LocalSettings.Values.Remove("nextpage");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                ApplicationDataCompositeValue temp = new ApplicationDataCompositeValue();
                temp["title"] = title.Text;
                temp["content"] = content.Text;
                temp["date"] = date.Date.ToString();
                temp["img_name"] = img_name;
                temp["edit_id"] = edit_id;
                temp["mode"] = (create.Content == "Save") ? true : false;
                ApplicationData.Current.LocalSettings.Values["nextpage"] = temp;
            }
        }

        private async void Convert_str_to_img(string source, BitmapImage src)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(source);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                await src.SetSourceAsync(stream);
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

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
   
}

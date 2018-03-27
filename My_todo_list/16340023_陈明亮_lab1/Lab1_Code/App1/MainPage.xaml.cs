using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using App1.Models;
using System.Collections.ObjectModel;
using static App1.ViewModels.bool_to_visi;
using App1.ViewModels;
using System.ComponentModel;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public AllItems ViewModel = ViewModels.MainViewModel.GetAllItems();
        private int img_size = 100;
        private int edit_id = -1; //In edit mode, the item id that focus
        private bool width_mode = false; //False for small window, Ture for big window
        private bool edit_or_create = false; //In big window, false for create, true for edit

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void changeBackImg()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/blue.jpg",UriKind.Absolute));
            grid.Background = imageBrush;
        }

        public MainPage()
        {
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Colors.AliceBlue;
            view.Title = "Mytodos";
            this.InitializeComponent();
            this.SizeChanged += (sender, e) =>
            {
                ObservableCollection<Models.Item> all = (ObservableCollection<Models.Item>)list.ItemsSource;
                if (Frame.ActualWidth < 600)
                {
                    foreach(var item in all)
                    {
                        item.ImageVisi = Visibility.Collapsed;
                    }
                }else
                {
                    foreach (var item in all)
                    {
                        item.ImageVisi = Visibility.Visible;
                    }
                }
            };
        }

        
        private void jump_next(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            width_mode = frame.ActualWidth >= 800 ? true : false;
            Debug.WriteLine(frame.ActualWidth);
            if(!width_mode)
            {
                Frame.Navigate(typeof(NextPage), "");
                add.Visibility = Visibility.Collapsed;
                delete.Visibility = Visibility.Collapsed;
            }else
            {
                on_initial();
            }
        }
        
        private async void delete_item(object sender, RoutedEventArgs e)
        {
            string content = null;
            if (edit_id == -1)
            {
                content = "Please pick an item before delete!";
            }
            else
            {
                ViewModel.DeleteItem(edit_id);
                content = "Delete successfully!";
            }
            var cd = new ContentDialog
            {
                Content = content,
                PrimaryButtonText = "OK"
            };
            await cd.ShowAsync();
            on_initial();
        }

        private async void on_create(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String error_message = "";
            if (title.Text == "")
            {
                error_message += "Title cannot be empty!\n";
            }
            if (content.Text == "")
            {
                error_message += "Content cannot be empty!\n";
            }
            if (Smaller(date))
            {
                error_message += "The date has already passed, please change another date!\n";
            }
            if (error_message == "")
            {
                if (edit_or_create) error_message = "Save successfully!";
                else error_message = "Create successfully!";
            }
            var cd = new ContentDialog()
            {
                Content = error_message,
                PrimaryButtonText = "OK",
            };
            if (error_message == "Create successfully!")
            {
                img_size = (int)eimg.Height;
                ViewModel.AddItem(title.Text, content.Text, todo_img.ImageSource, date.Date);
            }
            else if (error_message == "Save successfully!")
            {
                ViewModel.UpdateItem(title.Text, content.Text, todo_img.ImageSource, date.Date, edit_id);
            }
            await cd.ShowAsync();
        }

        private void on_cancel(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            content.Text = "";
            date.Date = DateTime.Now;
        }

        private void on_initial()
        {
            title.Text = "";
            content.Text = "";
            date.Date = DateTime.Now;
            create.Content = "Create";
            edit_or_create = false;
            edit_id = -1;
            list.SelectedItem = null;
            delete.Visibility = Visibility.Collapsed;
        }

        private bool Smaller(DatePicker date)
        {
            int res = date.Date.CompareTo(DateTime.Now);
            if (res == -1)
            {
                if (date.Date.Year == DateTime.Now.Year && date.Date.Month == DateTime.Now.Month && date.Date.Day == DateTime.Now.Day)
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
                using (IRandomAccessStream stream = await result.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    todo_img.ImageSource = srcImage;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sli = sender as Slider;
            if (sli != null)
            {
                eimg.Height = 180 + sli.Value * 0.3;
                eimg.Width = 180 + sli.Value * 0.3;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            width_mode = frame.ActualWidth >= 800 ? true:false;
            Item click_item = (Item)e.ClickedItem;
            edit_or_create = true; //Edit Mode
            edit_id = click_item.id;
            delete.Visibility = Visibility.Visible;
            if (width_mode)
            {
                create.Content = "Save";
                title.Text = click_item.title;
                content.Text = click_item.content;
                date.Date = click_item.date;
                todo_img.ImageSource = click_item.source;
            }else
            {
                Frame.Navigate(typeof(NextPage), click_item);
            }
        }

        private void list_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            Item find_item = MainViewModel.store.Select_with_title(text.Text);
            string time = find_item.date.Year.ToString() + '/' + find_item.date.Month.ToString() + '/' + find_item.date.Day.ToString();
            string content = "Description: " + find_item.content + "\nDate: " + time;
            ToolTip tip = new ToolTip();
            tip.Content = content;
            ToolTipService.SetToolTip(text, tip);
        }
    }
}

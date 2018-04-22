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
using Windows.Storage.Provider;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using System.Threading;
using Windows.Data.Xml.Dom;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using System.Runtime.Serialization.Json;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        //Main ViewModel List
        public AllItems ViewModel = ViewModels.MainViewModel.GetAllItems();
        //Pictures store main folder
        StorageFolder root = ApplicationData.Current.LocalFolder;
        DispatcherTimer timer = new DispatcherTimer();

        private int edit_id = -1; //In edit mode, the item id that focus
        private bool width_mode = false; //False for small window, Ture for big window
        private bool edit_or_create = false; //In big window, false for create, true for edit
        private int tick_id = 0;
        private string img_name = "-1.jpg";
        private string source_img = "-1.jpg";

        public event PropertyChangedEventHandler PropertyChanged;

        //PropertyChanged function
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        //Change the background image
        public void changeBackImg()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/green-blue.jpg",UriKind.Absolute));
            grid.Background = imageBrush;
        }

        //MainPage Contructer
        public MainPage()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Colors.AliceBlue;
            view.Title = "Mytodos";
            this.InitializeComponent();
            changeBackImg();
            //Control tile change time
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += ChangeTile;
            timer.Start();
            //Watch window size change function
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

        //Jump to nextpage with creating mode
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
        
        //Delete item
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
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            await cd.ShowAsync();
            on_initial();
        }

        //Create new item or save item changed value
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
            //If user do not update their own image, we use default image
            if(img_name == "-1.jpg")
            {
                img_name = (edit_or_create ? edit_id : AllItems.count) + ".jpg";
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
                ViewModel.AddItem(title.Text, content.Text, img_name, date.Date.ToString());
            }
            else if (error_message == "Save successfully!")
            {
                ViewModel.UpdateItem(title.Text, content.Text, img_name, date.Date.ToString(), edit_id);
            }
            img_name = "-1.jpg";
            await cd.ShowAsync();
        }

        //Clean all input
        private void on_cancel(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            content.Text = "";
            date.Date = DateTime.Now;
        }

        //Initial page data
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

        //Share item data with other apps
        private void on_share(object sender, RoutedEventArgs e)
        {
            //Get direct item with button clicked
            var context = (sender as FrameworkElement).DataContext;
            var item = list.ContainerFromItem(context) as ListViewItem;
            Item click_item = item.Content as Item;

            App.s_title = click_item.title;
            App.s_content = click_item.content;
            App.s_img = click_item.source;

            DataTransferManager.ShowShareUI();
        }

        //Deal date pass problem
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

        //Upload image function
        private async void img_click(object sender, RoutedEventArgs e)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(eimg);
            var srcImage = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            FileOpenPicker file = new FileOpenPicker();
            file.FileTypeFilter.Add(".jpg");
            file.FileTypeFilter.Add(".png");
            file.FileTypeFilter.Add(".jpeg");
            Windows.Storage.StorageFile result = await file.PickSingleFileAsync();
            if (result != null)
            {
                using (IRandomAccessStream stream = await result.OpenAsync(FileAccessMode.Read))
                {
                    string file_name = (edit_or_create ? edit_id : AllItems.count) + result.FileType;
                    img_name = file_name;
                    source_img = file_name;
                    StorageFile newfile = await root.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);
                    //Change filestream to byte[]
                    DataReader reader = new DataReader(stream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] img_byte = new byte[stream.Size];
                    reader.ReadBytes(img_byte);
                    //Write new image file to database folder
                    await FileIO.WriteBytesAsync(newfile, img_byte);
                    await srcImage.SetSourceAsync(stream);
                    todo_img.ImageSource = srcImage;
                }
            }
        }

        //Slider changed function
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sli = sender as Slider;
            if (sli != null)
            {
                eimg.Height = 180 + sli.Value * 0.3;
                eimg.Width = 180 + sli.Value * 0.3;
            }
        }

        //Item click function
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            width_mode = frame.ActualWidth >= 800 ? true:false;
            Item click_item = (Item)e.ClickedItem;
            Debug.WriteLine(click_item.isdealed);
            edit_or_create = true; //Edit Mode
            edit_id = click_item.id;
            delete.Visibility = Visibility.Visible;
            if (width_mode)
            {
                //Set item mesages
                create.Content = "Save";
                title.Text = click_item.title;
                content.Text = click_item.content;
                date.Date = Convert.ToDateTime(click_item.date);
                img_name = click_item.source;
                BitmapImage bit = new BitmapImage();
                Convert_str_to_img(click_item.source, bit);
                todo_img.ImageSource = bit;
            }else
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Item));
                MemoryStream ms = new MemoryStream();
                dcjs.WriteObject(ms, click_item);
                ms.Position = 0;
                StreamReader srm = new StreamReader(ms, Encoding.UTF8);
                string json_pass = srm.ReadToEnd();
                Frame.Navigate(typeof(NextPage), json_pass);
            }
        }

        //Get source image with path
        private async void Convert_str_to_img(string source, BitmapImage src)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(source);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                await src.SetSourceAsync(stream);
            }
        }

        //Bonus: point over then show informations
        private void list_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            Item find_item = MainViewModel.store.Select_with_title(text.Text);
            DateTimeOffset true_date = Convert.ToDateTime(find_item.date);
            string time = true_date.Year.ToString() + '/' + true_date.Month.ToString() + '/' + true_date.Day.ToString();
            string content = "Description: " + find_item.content + "\nDate: " + time;
            ToolTip tip = new ToolTip();
            tip.Content = content;
            ToolTipService.SetToolTip(text, tip);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("mainpage");
            }else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("mainpage"))
                {
                    var temp = ApplicationData.Current.LocalSettings.Values["mainpage"] as ApplicationDataCompositeValue;
                    //Composite Page settings reload
                    title.Text = (string)temp["title"];
                    content.Text = (string)temp["content"];
                    date.Date = Convert.ToDateTime(temp["date"]);
                    img_name = (string)temp["img_name"];
                    edit_id = (int)temp["edit_id"];
                    edit_or_create = (bool)temp["mode"];
                    if (edit_or_create) create.Content = "Save";
                    //Bonus: Image reload
                    BitmapImage bit = new BitmapImage();
                    Convert_str_to_img((string)temp["img_name"], bit);
                    todo_img.ImageSource = bit;

                    ApplicationData.Current.LocalSettings.Values.Remove("mainpage");
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
                temp["mode"] = edit_or_create;
                ApplicationData.Current.LocalSettings.Values["mainpage"] = temp;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as FrameworkElement).DataContext;
            var item = list.ContainerFromItem(context) as ListViewItem;
            Item click_item = item.Content as Item;
            ViewModel.CompleteItem(click_item.id);
        }

        private void ChangeTile(object sender, object e)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(File.ReadAllText("Tile.xml"));
            var textList = xmld.GetElementsByTagName("text");
            var imgList = xmld.GetElementsByTagName("image");
            if (AllItems.count == 0) return;
            else if(tick_id < AllItems.count)
            {
                //Text Change
                try
                {
                    textList[0].InnerText = ViewModel.ItemStore[tick_id].title;
                    textList[1].InnerText = ViewModel.ItemStore[tick_id].content;
                    textList[2].InnerText = ViewModel.ItemStore[tick_id].title;
                    textList[3].InnerText = ViewModel.ItemStore[tick_id].content;
                    textList[4].InnerText = ViewModel.ItemStore[tick_id].title;
                    textList[5].InnerText = ViewModel.ItemStore[tick_id].content;
                    textList[6].InnerText = ViewModel.ItemStore[tick_id].title;
                    textList[7].InnerText = ViewModel.ItemStore[tick_id].content;
                    //Image Source Change
                    string source = ViewModel.ItemStore[tick_id].source;
                    XmlElement img0 = imgList[0] as XmlElement, img1 = imgList[1] as XmlElement,
                                img2 = imgList[2] as XmlElement, img3 = imgList[3] as XmlElement;
                    img0.SetAttribute("src", "ms-appdata:///local/" + source);
                    img1.SetAttribute("src", "ms-appdata:///local/" + source);
                    img2.SetAttribute("src", "ms-appdata:///local/" + source);
                    img3.SetAttribute("src", "ms-appdata:///local/" + source);
                    //Send Tile notification
                    var update_notification = new TileNotification(xmld);
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(update_notification);
                    TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                    tick_id++;
                }catch(Exception ewq) { }
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string pre_key = key.Text;
            string reg = @"", result = "";
            int count = 0, size = pre_key.Length;
            //Regular expression build
            foreach (var c in pre_key)
            {
                count++;
                reg += c;
                reg += '+';
                reg += @"\S*";
            }
            foreach(Item item in ViewModel.ItemStore)
            {
                if (Regex.IsMatch(item.title, reg) || Regex.IsMatch(item.content, reg)
                    || Regex.IsMatch(item.date, reg))
                    result += item.title + ' ' + item.content + ' ' + item.date.Substring(0, 17) + '\n';
            }
            var cd = new ContentDialog
            {
                Content = result,
                PrimaryButtonText = "OK"
            };
            await cd.ShowAsync();
            Debug.WriteLine(reg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Colors.AliceBlue;
            view.Title = "Mytodos";
            this.InitializeComponent();
            on_Load_Check_Store();
            on_Load_Check_Deal();
        }
        public void box_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            string name = box.Name;
            DealItemCollection.name_collection.Add(name);
            char num = name[name.Length-1];
            Line target = (Line)this.FindName("line" + num);
            target.Visibility = Visibility.Visible;
        }

        public void box_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            string name = box.Name;
            DealItemCollection.name_collection.Remove(name);
            Debug.WriteLine(DealItemCollection.name_collection.Count);
            char num = name[name.Length - 1];
            Line target = (Line)this.FindName("line" + num);
            target.Visibility = Visibility.Collapsed;
        }

        private void btn1_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NextPage), "");
            app_btn.Visibility = Visibility.Collapsed;
        }
        
        private void on_Load_Check_Store()
        {
            if (ItemStore.store == null) return;
            for(int i=0;i<ItemStore.store.Count;i++)
            {
                Item item = (Item)ItemStore.store[i];
                item.AddToPage(this);
            }
        }

        private void on_Load_Check_Deal()
        {
            if (DealItemCollection.name_collection == null) return;
            foreach(string name in DealItemCollection.name_collection)
            {
                CheckBox deal_box = (CheckBox)this.FindName(name);
                deal_box.IsChecked = true;
                string line_name = "line" + name[name.Length - 1];
                Line deal_line = (Line)this.FindName(line_name);
                deal_line.Visibility = Visibility.Visible;
            }
        }


        private void delete_item(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem Mitem = (MenuFlyoutItem)sender;
            int id = int.Parse(Mitem.Name);
            for (int i = 0; i < ItemStore.store.Count; i++)
            {
                Item item = (Item)ItemStore.store[i];
                if (item.id == id)
                {
                    ItemStore.store.Remove(item);
                    ItemStore.num -= 1;
                }
            }
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MainPage));
        }

        private void edit_item(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem Mitem = (MenuFlyoutItem)sender;
            int id = int.Parse(Mitem.Name);
            Item mess = null;
            for(int i = 0; i < ItemStore.store.Count; i++)
            {
                Item item = (Item)ItemStore.store[i];
                if(item.id == id)
                {
                    mess = new Item(item.text, item.content, item.source, item.date, item.id);
                }
            }
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(NextPage), mess);
        }


        public class Item
        {
            public string text;
            public string content;
            public ImageSource source;
            public DateTimeOffset date;
            public int id;
            public Item(string text, string content, ImageSource source, DateTimeOffset date, int id)
            {
                this.text = text;
                this.content = content;
                this.source = source;
                this.date = date;
                this.id = id;
            }
            public void AddToPage(MainPage page)
            {
                //New CheckBox
                CheckBox new_box = new CheckBox()
                {
                    Name = "box" + id,
                    Margin = new Thickness(10, 20, 0, 0)
                };
                new_box.Checked += page.box_Checked;
                new_box.Unchecked += page.box_Unchecked;
                page.main.Children.Add(new_box);
                Grid.SetColumn(new_box, 0);
                Grid.SetRow(new_box, id);
                //New Image
                Image new_img = new Image
                {
                    Source = source,
                    Height = 100,
                    Width = 120
                };
                Grid.SetColumn(new_img, 1);
                Grid.SetRow(new_img, id);
                page.main.Children.Add(new_img);
                //New Text
                TextBlock new_text = new TextBlock()
                {
                    Text = text,
                    Margin = new Thickness(20, 30, 169, 28),
                    FontSize = 20
                };
                Grid.SetColumn(new_text, 2);
                Grid.SetRow(new_text, id);
                page.main.Children.Add(new_text);
                //New Line
                Line new_line = new Line()
                {
                    X1 = 1,
                    Stretch = Stretch.Fill,
                    Height = 2,
                    Width = 600,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2,
                    Name = "line" + id,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(20, 0, 0, 0)
                };
                Grid.SetColumn(new_line, 2);
                Grid.SetRow(new_line, id);
                page.main.Children.Add(new_line);
                //New btn
                AppBarButton new_btn = new AppBarButton()
                {
                    Margin = new Thickness(0, 0, 20, 0),
                    Icon = new SymbolIcon(Symbol.Setting),
                    IsCompact = true,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                MenuFlyout new_menu = new MenuFlyout();
                MenuFlyoutItem item1 = new MenuFlyoutItem()
                {
                    Text = "Edit",
                    Name = id.ToString()
                };
                item1.Click += page.edit_item;
                MenuFlyoutItem item2 = new MenuFlyoutItem
                {
                    Text = "Delete",
                    Name = id.ToString()
                };
                item2.Click += page.delete_item;
                new_menu.Items.Add(item1);
                new_menu.Items.Add(item2);
                new_btn.Flyout = new_menu;
                Grid.SetColumn(new_btn, 3);
                Grid.SetRow(new_btn, id);
                page.main.Children.Add(new_btn);
            }

        }

    }
}

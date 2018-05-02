using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace API_caller
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class Image : Page
    {
        const string subscriptionKey = "bc8ea83097cd4268b45d1e1e68e32006";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";
        public Image()
        {
            this.InitializeComponent();
            changeBackImg();
        }

        private void changeBackImg()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/green-blue.jpg", UriKind.Absolute));
            grid.Background = imageBrush;
        }

        private async void update_img(object sender, RoutedEventArgs e)
        {
            BitmapImage srcImage = new BitmapImage();
            FileOpenPicker file = new FileOpenPicker();
            file.FileTypeFilter.Add(".jpg");
            file.FileTypeFilter.Add(".png");
            file.FileTypeFilter.Add(".jpeg");
            Windows.Storage.StorageFile result = await file.PickSingleFileAsync();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";
            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;
            if (result != null)
            {
                using (IRandomAccessStream stream = await result.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    r_img.Source = srcImage;
                    DataReader reader = new DataReader(stream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)stream.Size);
                    byte[] bytes = new byte[stream.Size];
                    reader.ReadBytes(bytes);
                    info.Text = "Recognizing.......";
                    ByteArrayContent content = new ByteArrayContent(bytes);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                    string resultStr = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(resultStr);
                    ShowImageDetails(resultStr);
                }
            }
        }

        private void ShowImageDetails(string result)
        {
            JsonObject json = JsonValue.Parse(result).GetObject();
            JsonArray data = json.GetNamedArray("categories");
            JsonObject des = json.GetNamedObject("description");
            JsonObject color = json.GetNamedObject("color");
            string name = "Image Tip-Name: \n\t", tags = "\nImage Tags: \n\t", caption = "\nImage Captions: \n\t", mcolor = "\nImage Main Colors: \n\t";
            for(int i = 0; i < data.Count; i++)
            {
                if (i == data.Count - 1) name += data[i].GetObject().GetNamedString("name") + "\n";
                else name += data[i].GetObject().GetNamedString("name") + ",  ";
            }
            for(int i = 0; i < des.GetNamedArray("tags").Count && i<=8; i++)
            {
                if (i == des.GetNamedArray("tags").Count - 1) tags += des.GetNamedArray("tags")[i] + "\n";
                else tags += des.GetNamedArray("tags")[i] + ",  ";
            }
            for (int i = 0; i < des.GetNamedArray("captions").Count; i++)
            {
                if (i == des.GetNamedArray("captions").Count - 1) caption += des.GetNamedArray("captions")[i].GetObject().GetNamedString("text") + "\n";
                else caption += des.GetNamedArray("captions")[i].GetObject().GetNamedString("text") + ",  ";
            }
            mcolor += "ForegroundColor: "+color.GetNamedString("dominantColorForeground") + "\n\t";
            mcolor += "BackgroundColor: " + color.GetNamedString("dominantColorBackground") + "\n\t";
            mcolor += "Main Colors: ";
            JsonArray maincolor = color.GetNamedArray("dominantColors");
            for(int i = 0; i < maincolor.Count; i++)
            {
                if (i == maincolor.Count - 1) mcolor += maincolor[i] + "\n";
                else mcolor += maincolor[i] + ",  ";
            }
            info.Text = name + tags + caption + mcolor;
        }

        
    }
}

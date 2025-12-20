using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.Views
{
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
        }

        public void SetImage(BitmapImage bitmapImage)
        {
            DisplayedImage.Source = bitmapImage;
        }
    }
}

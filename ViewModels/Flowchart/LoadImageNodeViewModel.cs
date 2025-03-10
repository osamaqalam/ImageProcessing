using ImageProcessing.App.Models.Flowchart;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Services.Imaging;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.IO;
using ImageProcessing.App.Models.Imaging;
using ImageProcessing.App.Views;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class LoadImageNodeViewModel : FlowchartNodeViewModel
    {
        private readonly IImageService _imageService;
        private LoadImageNode _model = new();

        public ICommand BrowseCommand { get; }

        private string? _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                // 1. Update ViewModel's backing field
                if (SetProperty(ref _imagePath, value))
                {
                    // 2. Mirror to Model
                    _model.ImagePath = value;
                }
            }
        }

        private BitmapImage? _loadedBitmap;
        public BitmapImage? LoadedBitmap
        {
            get => _loadedBitmap;
            private set
            {
                if (SetProperty(ref _loadedBitmap, value))
                {
                    OpenImageWindow(value);
                }
            }
        }
        public LoadImageNodeViewModel(IImageService imageService)
        {
            _imageService = imageService;
            BrowseCommand = new RelayCommand(_ => BrowseImage());
        }

        private void BrowseImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
                var image = _imageService.LoadImage(ImagePath);
                LoadedBitmap = ConvertToBitmapImage(image);
            }
        }

        private BitmapImage ConvertToBitmapImage(ImageData imageData)
        {
            if (imageData?.PixelData == null) return null;

            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream(imageData.PixelData))
            {
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            bitmap.Freeze(); // For thread safety
            return bitmap;
        }

        private void OpenImageWindow(BitmapImage bitmapImage)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var imageWindow = new ImageWindow();
                imageWindow.SetImage(bitmapImage);
                imageWindow.Show();
            });
        }
    }
}

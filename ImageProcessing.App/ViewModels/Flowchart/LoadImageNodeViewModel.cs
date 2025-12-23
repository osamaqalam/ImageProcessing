using ImageProcessing.App.Models.Flowchart;
using Microsoft.Win32;
using System.Windows.Input;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Services.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using ImageProcessing.App.Models.Imaging;
using ImageProcessing.App.Views;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class LoadImageNodeViewModel : ExecutableNodeViewModel, IImageOutputNode
    {
        private readonly IImageService _imageService;
        private LoadImageNode _model = new();
        
        public event Action<BitmapImage>? ImageOutputted;

        private static int _counter = 0;

        public ICommand BrowseCommand { get; }

        private string? _imagePath;
        public string? ImagePath
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

        private BitmapImage? _outputImage;
        public BitmapImage? OutputImage
        {
            get => _outputImage;
            private set
            {
                if (SetProperty(ref _outputImage, value))
                {
                    OpenImageWindow(value);
                    ImageOutputted?.Invoke(value);
                }
            }
        }
        public LoadImageNodeViewModel(IImageService imageService)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 70;

            Id = ++_counter;
            Label = $"LoadImage{(Id > 1 ? $" {Id}" : "")}";

            _imageService = imageService;
            BrowseCommand = new RelayCommand(_ => BrowseImage());
        }

        public override bool CanExecute()
        {
            return ImagePath != null;
        }

        public override void Execute()
        {
            var image = _imageService.LoadImage(ImagePath);
            OutputImage = ConvertToBitmapImage(image); 
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

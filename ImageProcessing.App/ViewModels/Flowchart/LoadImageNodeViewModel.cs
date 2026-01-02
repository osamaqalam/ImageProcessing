using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Models.Imaging;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class LoadImageNodeViewModel : ImageOutputNodeViewModel
    {
        private LoadImageNode _model = new();

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

        public LoadImageNodeViewModel(IImageService imageService)
            : base(imageService)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 70;

            Id = ++_counter;
            Label = $"LoadImage{(Id > 1 ? $" {Id}" : "")}";

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
    }
}

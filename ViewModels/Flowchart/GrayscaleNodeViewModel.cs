using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Views;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    class GrayscaleNodeViewModel : FlowchartNodeViewModel, IImageOutputNode
    {
        private readonly IImageService _imageService;

        // Reference to MainVM's OutputImages
        public ObservableDictionary<string, ImageNodeData> OutputImages { get; }

        public event Action<BitmapImage>? ImageOutputted;

        private string? _selectedNodeId;
        public string? SelectedNodeId
        {
            get => _selectedNodeId;
            set
            {
                if (SetProperty(ref _selectedNodeId, value))
                {
                    OutputImages.TryGetValue(value, out ImageNodeData imageNodeData);
                    BitmapImage grayscaleImage = _imageService.ConvertToGrayscale(imageNodeData.Image); ;
                    ImageOutputted?.Invoke(grayscaleImage);
                    OpenImageWindow(grayscaleImage);
                }
            }
        }

        public GrayscaleNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 60;

            _imageService = imageService;
            OutputImages = outputImages;
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

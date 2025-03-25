using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Views;
using System.Windows.Media.Imaging;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

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
                    ;
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

        public GrayscaleNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 60;

            _imageService = imageService;
            OutputImages = outputImages;
        }

        public bool CanExecute()
        {
            return SelectedNodeId != null;
        }

        public void Execute()
        {
            OutputImages.TryGetValue(SelectedNodeId, out ImageNodeData imageNodeData);
            OutputImage = _imageService.ConvertToGrayscale(imageNodeData.Image); ;
            ImageOutputted?.Invoke(OutputImage);
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

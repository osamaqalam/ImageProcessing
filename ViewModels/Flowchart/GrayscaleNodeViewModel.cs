using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Views;
using System.Windows.Media.Imaging;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using System.ComponentModel;
using System.Windows.Data;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    class GrayscaleNodeViewModel : FlowchartNodeViewModel, IImageOutputNode
    {
        private readonly IImageService _imageService;

        // Reference to MainVM's OutputImages
        public ObservableDictionary<string, ImageNodeData> OutputImages { get; }

        /// <summary>
        /// Collection view of OutputImages excluding entries from this node
        /// </summary>
        private ICollectionView? _inImgOptions;
        public ICollectionView InImgOptions
        {
            get
            {
                if (_inImgOptions == null)
                {
                    _inImgOptions = CollectionViewSource.GetDefaultView(OutputImages);
                    _inImgOptions.Filter = item =>
                    {
                        var kvp = (MutableKeyValuePair<string, ImageNodeData>)item;
                        return kvp.Value.SourceNode != this;
                    };
                }
                return _inImgOptions;
            }
        }

        public event Action<BitmapImage>? ImageOutputted;

        private static int _counter = 0;

        private string? _selectedInImgLabel;
        public string? SelectedInImgLabel
        {
            get => _selectedInImgLabel;
            set
            {
                if (SetProperty(ref _selectedInImgLabel, value))
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

            Id = ++_counter;
            Label = $"Grayscale{(Id > 1 ? $" {Id}" : "")}";

            _imageService = imageService;
            OutputImages = outputImages;
        }

        public bool CanExecute()
        {
            return SelectedInImgLabel != null;
        }

        public void Execute()
        {
            OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData);
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

        public void RefreshFilter()
        {
            // Recreate the CollectionView to ensure the filter uses the latest state
            _inImgOptions = null;
            OnPropertyChanged(nameof(InImgOptions));
        }
    }
}

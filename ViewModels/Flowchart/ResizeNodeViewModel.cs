using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using ImageProcessing.App.Views;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class ResizeNodeViewModel : FlowchartNodeViewModel, IImageOutputNode
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

        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
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

        public ResizeNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 80;

            Id = ++_counter;
            Label = $"Resize{(Id > 1 ? $" {Id}" : "")}";

            _imageService = imageService;
            OutputImages = outputImages;
        }

        public bool CanExecute()
        {
            return SelectedInImgLabel != null && Scale != 0;
        }

        public void Execute()
        {
            OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData);
            OutputImage = _imageService.Resize(imageNodeData.Image, Scale); ;
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

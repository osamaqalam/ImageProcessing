using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.Views;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart.Abstractions
{
    /// <summary>
    /// Abstract base class for nodes that output images
    /// Contains common functionality for image processing nodes
    /// </summary>
    public abstract class ImageOutputNodeViewModel : ExecutableNodeViewModel
    {
        protected readonly IImageService _imageService;

        // Reference to MainVM's OutputImages (optional for nodes that don't consume other images)
        public ObservableDictionary<string, ImageNodeData>? OutputImages { get; }

        /// <summary>
        /// Collection view of OutputImages excluding entries from this node
        /// Only available if OutputImages is provided
        /// </summary>
        private ICollectionView? _inImgOptions;
        public ICollectionView? InImgOptions
        {
            get
            {
                if (OutputImages == null) return null;
                
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

        private string? _selectedInImgLabel;
        public string? SelectedInImgLabel
        {
            get => _selectedInImgLabel;
            set => SetProperty(ref _selectedInImgLabel, value);
        }

        private BitmapImage? _outputImage;
        public BitmapImage OutputImage
        {
            get => _outputImage!;
            protected set
            {
                if (SetProperty(ref _outputImage, value))
                {
                    if (value != null)
                    {
                        OpenImageWindow(value);
                        ImageOutputted?.Invoke(value);
                    }
                }
            }
        }

        // Constructor for nodes that consume other images (like Binarize, Grayscale, Resize)
        protected ImageOutputNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            _imageService = imageService;
            OutputImages = outputImages;
        }

        // Constructor for nodes that don't consume other images (like LoadImage)
        protected ImageOutputNodeViewModel(IImageService imageService)
        {
            _imageService = imageService;
            OutputImages = null;
        }

        /// <summary>
        /// Opens the image in a new window
        /// </summary>
        protected virtual void OpenImageWindow(BitmapImage bitmapImage)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var imageWindow = new ImageWindow();
                imageWindow.SetImage(bitmapImage);
                imageWindow.Show();
            });
        }

        /// <summary>
        /// Refreshes the InImgOptions filter to reflect current state
        /// </summary>
        public void RefreshFilter()
        {
            // Recreate the CollectionView to ensure the filter uses the latest state
            _inImgOptions = null;
            OnPropertyChanged(nameof(InImgOptions));
        }
    }
}

using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using ImageProcessing.App.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    class BinarizeNodeViewModel : ExecutableNodeViewModel, IImageOutputNode
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

        /// <summary>
        /// Collection view for thresholding type options
        /// </summary>
        private ICollectionView? _thresholdingTypeOptions;
        public ICollectionView ThresholdingTypeOptions
        {
            get
            {
                if (_thresholdingTypeOptions == null)
                {
                    var options = new List<string> { "InRange", "OutRange" };
                    _thresholdingTypeOptions = CollectionViewSource.GetDefaultView(options);
                }
                return _thresholdingTypeOptions;
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

        private string? _selectedThresholdingType;
        public string? SelectedThresholdingType
        {
            get => _selectedThresholdingType;
            set
            {
                if (SetProperty(ref _selectedThresholdingType, value))
                    ;
            }
        }

        private int _rangeStart;
        public int RangeStart
        {
            get => _rangeStart;
            set => SetProperty(ref _rangeStart, value);
        }

        private int _rangeEnd;
        public int RangeEnd
        {
            get => _rangeEnd;
            set => SetProperty(ref _rangeEnd, value);
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
        public BinarizeNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            // Set custom size for image nodes
            Width = 200;
            Height = 180;

            Id = ++_counter;
            Label = $"Binarize{(Id > 1 ? $" {Id}" : "")}";

            _imageService = imageService;
            OutputImages = outputImages;
        }

        public override bool CanExecute()
        {
            return SelectedInImgLabel != null;
        }

        public override void Execute()
        {
            OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData);
            OutputImage = _imageService.ConvertToBinary(imageNodeData.Image, _selectedThresholdingType, _rangeStart, _rangeEnd);
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

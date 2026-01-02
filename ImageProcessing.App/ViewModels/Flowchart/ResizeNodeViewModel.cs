using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class ResizeNodeViewModel : ImageOutputNodeViewModel
    {
        /// <summary>
        /// Collection view for interpolation mode options
        /// </summary>
        private ICollectionView? _interpolationModeOptions;
        public ICollectionView InterpolationModeOptions
        {
            get
            {
                if (_interpolationModeOptions == null)
                {
                    var options = new List<string> { "NearestNeighbor", "Bilinear" };
                    _interpolationModeOptions = CollectionViewSource.GetDefaultView(options);
                }
                return _interpolationModeOptions;
            }
        }

        private static int _counter = 0;

        private string? _selectedInterpolationMode;
        public string? SelectedInterpolationMode
        {
            get => _selectedInterpolationMode;
            set => SetProperty(ref _selectedInterpolationMode, value);
        }

        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        public ResizeNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
            : base(imageService, outputImages)
        {
            // Set custom size for image nodes
            Width = 200;
            Height = 130;

            Id = ++_counter;
            Label = $"Resize{(Id > 1 ? $" {Id}" : "")}";
        }

        public override bool CanExecute()
        {
            return SelectedInImgLabel != null && Scale != 0;
        }

        public override void Execute()
        {
            if (SelectedInImgLabel != null && OutputImages != null && OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData) && imageNodeData.Image != null)
            {
                OutputImage = _imageService.Resize(imageNodeData.Image, Scale, SelectedInterpolationMode);
            }
        }
    }
}

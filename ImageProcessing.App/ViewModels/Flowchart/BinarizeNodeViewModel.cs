using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    class BinarizeNodeViewModel : ImageOutputNodeViewModel
    {
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

        private static int _counter = 0;

        private string? _selectedThresholdingType;
        public string? SelectedThresholdingType
        {
            get => _selectedThresholdingType;
            set => SetProperty(ref _selectedThresholdingType, value);
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

        public BinarizeNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
            : base(imageService, outputImages)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 40;

            Id = ++_counter;
            Label = $"Binarize{(Id > 1 ? $" {Id}" : "")}";
        }

        public override bool CanExecute()
        {
            return SelectedInImgLabel != null;
        }

        public override void Execute()
        {
            if (SelectedInImgLabel != null && OutputImages != null && OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData) && imageNodeData.Image != null)
            {
                OutputImage = _imageService.ConvertToBinary(imageNodeData.Image, _selectedThresholdingType!, _rangeStart, _rangeEnd);
            }
        }
    }
}

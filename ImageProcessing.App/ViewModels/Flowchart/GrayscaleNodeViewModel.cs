using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    class GrayscaleNodeViewModel : ImageOutputNodeViewModel
    {
        private static int _counter = 0;

        public GrayscaleNodeViewModel(IImageService imageService, ObservableDictionary<string, ImageNodeData> outputImages)
            : base(imageService, outputImages)
        {
            // Set custom size for image nodes
            Width = 100;
            Height = 70;

            Id = ++_counter;
            Label = $"Grayscale{(Id > 1 ? $" {Id}" : "")}";
        }

        public override bool CanExecute()
        {
            return SelectedInImgLabel != null;
        }

        public override void Execute()
        {
            if (SelectedInImgLabel != null && OutputImages != null && OutputImages.TryGetValue(SelectedInImgLabel, out ImageNodeData imageNodeData) && imageNodeData.Image != null)
            {
                OutputImage = _imageService.ConvertToGrayscale(imageNodeData.Image);
            }
        }
    }
}

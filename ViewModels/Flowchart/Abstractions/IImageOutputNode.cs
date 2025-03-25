using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart.Abstractions
{
    // Interfaces/IImageOutputNode.cs
    public interface IImageOutputNode : IExecutableNode
    {
        BitmapImage OutputImage { get; }
        event Action<BitmapImage> ImageOutputted;
    }
}

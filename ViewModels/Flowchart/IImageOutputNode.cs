using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    // Interfaces/IImageOutputNode.cs
    public interface IImageOutputNode
    {
        event Action<BitmapImage> ImageOutputted;
    }
}

using ImageProcessing.App.ViewModels;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.Models.Flowchart
{
    // Models/Flowchart/ImageNodeData.cs
    public class ImageNodeData
    {
        public BitmapImage Image { get; }
        public FlowchartNodeViewModel SourceNode { get; }

        public ImageNodeData(BitmapImage image, FlowchartNodeViewModel node)
        {
            Image = image;
            SourceNode = node;
        }
    }
}

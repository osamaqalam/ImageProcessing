using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App.ViewModels
{
    public class FlowchartNodeViewModel : ViewModelBase, IFlowchartNode
    {
        private string _nodeId = Guid.NewGuid().ToString();
        public string NodeId { get => _nodeId; set => SetProperty(ref _nodeId, value); }

        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }

        private double _y;
        public double Y { get => _y; set => SetProperty(ref _y, value); }

        private double _width = 100;
        public double Width { get => _width; set => SetProperty(ref _width, value); }

        private double _height = 100;
        public double Height { get => _height; set => SetProperty(ref _height, value); }

    }
}

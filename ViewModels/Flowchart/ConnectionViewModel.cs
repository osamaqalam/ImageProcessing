// ViewModels/Flowchart/ConnectionViewModel.cs
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class ConnectionViewModel : ViewModelBase
    {
        public IFlowchartNode Source { get; }
        public IFlowchartNode Target { get; }

        public ConnectionViewModel(IFlowchartNode source, IFlowchartNode target)
        {
            Source = source;
            Target = target;
        }
    }
}
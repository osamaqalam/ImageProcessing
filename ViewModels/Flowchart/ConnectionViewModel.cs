// ViewModels/Flowchart/ConnectionViewModel.cs
using ImageProcessing.App.Utilities;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    public class ConnectionViewModel : ViewModelBase
    {
        public FlowchartNodeViewModel Source { get; }
        public FlowchartNodeViewModel Target { get; }

        public ConnectionViewModel(FlowchartNodeViewModel source, FlowchartNodeViewModel target)
        {
            Source = source;
            Target = target;
        }
    }
}
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;

namespace ImageProcessing.App.ViewModels.Flowchart
{
    /// <summary>
    /// Base class for executable flowchart nodes
    /// </summary>
    public abstract class ExecutableNodeViewModel : FlowchartNodeViewModel, IExecutableNode
    {
        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set => SetProperty(ref _isExecuting, value);
        }

        public abstract void Execute();
        public abstract bool CanExecute();
    }
}

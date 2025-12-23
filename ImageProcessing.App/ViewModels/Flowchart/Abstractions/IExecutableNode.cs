using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.ViewModels.Flowchart.Abstractions
{
    // For nodes that perform processing
    public interface IExecutableNode : IFlowchartNode
    {
        bool IsExecuting { get; set; }
        void Execute();
        bool CanExecute();
    }
}

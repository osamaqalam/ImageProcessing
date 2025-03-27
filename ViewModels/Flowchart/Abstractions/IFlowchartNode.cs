using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.App.ViewModels.Flowchart.Abstractions
{
    public interface IFlowchartNode
    {
        int Id { get; }
        string Label { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }
}

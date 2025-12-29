using System.Collections.Generic;

namespace ImageProcessing.App.Models.Flowchart
{
    /// <summary>
    /// Data Transfer Object for the entire flowchart
    /// </summary>
    public class FlowchartDTO
    {
        public string Version { get; set; } = "1.0";
        public List<NodeDTO> Nodes { get; set; } = new();
        public List<ConnectionDTO> Connections { get; set; } = new();
    }
}

using System.Collections.Generic;

namespace ImageProcessing.App.Models.Flowchart
{
    /// <summary>
    /// DTO for connections between nodes
    /// </summary>
    public class ConnectionDTO
    {
        public int SourceNodeId { get; set; }
        public int TargetNodeId { get; set; }
    }
}

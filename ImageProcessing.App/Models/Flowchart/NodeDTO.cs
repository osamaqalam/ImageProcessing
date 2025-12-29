using System.Collections.Generic;

namespace ImageProcessing.App.Models.Flowchart
{
    /// <summary>
    /// Base DTO for all node types
    /// </summary>
    public class NodeDTO
    {
        public int Id { get; set; }
        public string NodeType { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        // Node-specific properties stored as dictionary for flexibility
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}

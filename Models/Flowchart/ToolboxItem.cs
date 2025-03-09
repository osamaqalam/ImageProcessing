namespace ImageProcessing.App.Models.Flowchart;

public class ToolboxItem
{
    public string Title { get; set; }    // Display name (e.g., "Load Image")
    public Type NodeType { get; set; }   // Associated ViewModel type (e.g., LoadImageNodeViewModel)

    public ToolboxItem(string title, Type nodeType)
    {
        Title = title;
        NodeType = nodeType;
    }
}
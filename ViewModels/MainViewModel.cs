using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Windows.Input;
using System.Xml.Linq;

namespace ImageProcessing.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IImageService _imageService;

    // Collection of flowchart nodes
    public ObservableCollection<FlowchartNodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

    // Currently selected node (for context menus/properties)
    private FlowchartNodeViewModel? _selectedNode;
    public FlowchartNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }

    // Commands
    public ICommand AddNodeCommand { get; }
    public ICommand DeleteNodeCommand { get; }

    public MainViewModel(IImageService imageService)
    {
        _imageService = imageService;

        // Initialize start and end nodes
        var startNode = new StartNodeViewModel { X = 300, Y = 100 };
        var endNode = new EndNodeViewModel { X = 300, Y = 300 };

        Nodes.Add(startNode);
        Nodes.Add(endNode);

        var connection = new ConnectionViewModel(startNode, endNode);
        Connections.Add(connection);

        // Initialize commands
        AddNodeCommand = new RelayCommand(execute => AddNode());
        DeleteNodeCommand = new RelayCommand(execute => DeleteNode(), canExecute => CanDeleteNode());
    }

    /// <summary>Adds a new node to the flowchart canvas</summary>
    private void AddNode()
    {
        var newNode = new LoadImageNodeViewModel(_imageService)
        {
            X = Nodes.Last().X, // Default position
            Y = Nodes.Any() ? Nodes[Nodes.Count - 2].Y + 50 : 100
        };
        Nodes.Insert(Nodes.Count - 1, newNode); // Node before the endnode
        //Nodes.Add(newNode);

    }

    /// <summary>Removes the currently selected node</summary>
    private void DeleteNode()
    {
        if (SelectedNode != null)
        {
            Nodes.Remove(SelectedNode);
            SelectedNode = null;
        }
    }

    /// <summary>Determines if the Delete command can execute</summary>
    private bool CanDeleteNode() => SelectedNode != null;
}
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace ImageProcessing.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IImageService _imageService;

    // Collection of flowchart nodes
    public ObservableCollection<FlowchartNodeViewModel> Nodes { get; } = new();

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

        // Add a default node
        Nodes.Add(new LoadImageNodeViewModel(_imageService)
        {
            X = 100,
            Y = 100
        });

        // Initialize commands
        AddNodeCommand = new RelayCommand(_ => AddNode());
        DeleteNodeCommand = new RelayCommand(_ => DeleteNode(), _ => CanDeleteNode());
    }

    /// <summary>Adds a new node to the flowchart canvas</summary>
    private void AddNode()
    {
        var newNode = new LoadImageNodeViewModel(_imageService)
        {
            X = 100, // Default position
            Y = 100
        };

        Nodes.Add(newNode);
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
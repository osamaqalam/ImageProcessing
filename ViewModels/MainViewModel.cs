using ImageProcessing.App.Services;
using ImageProcessing.App.Services.Imaging;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ImageProcessing.App.Models.Flowchart;
using System.Windows.Media.Imaging;
using ImageProcessing.App.Models.Imaging;

namespace ImageProcessing.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IImageService _imageService;

    // Collection of flowchart nodes
    public ObservableCollection<FlowchartNodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
    public ObservableDictionary<string, ImageNodeData> OutputImages { get; } = new();

    // Commands
    public ICommand AddNodeCommand { get; }
    public ICommand DeleteNodeCommand { get; }
    public ICommand ConnectionClickedCommand { get; }

    private const double FLOWCHART_CENTER_X = 300;
    private const double FLOWCHART_START_Y = 100;
    private const double FLOWCHART_END_Y = 300;
    private const double FLOWCHART_START_END_WIDTH = 10;
    private const double FLOWCHART_START_END_HEIGHT = 10;


    // Currently selected node (for context menus/properties)
    private FlowchartNodeViewModel? _selectedNode;
    public FlowchartNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }

    public MainViewModel(IImageService imageService)
    {
        _imageService = imageService;

        // Initialize start and end nodes
        var startNode = new StartNodeViewModel { X = FLOWCHART_CENTER_X,
            Y = FLOWCHART_START_Y, 
            Width = FLOWCHART_START_END_WIDTH, 
            Height = FLOWCHART_START_END_HEIGHT };

        var endNode = new EndNodeViewModel { X = FLOWCHART_CENTER_X, 
            Y = FLOWCHART_END_Y, 
            Width = FLOWCHART_START_END_WIDTH, 
            Height = FLOWCHART_START_END_HEIGHT };

        Nodes.Add(startNode);
        Nodes.Add(endNode);

        var connection = new ConnectionViewModel(startNode, endNode);
        Connections.Add(connection);

        // Initialize commands
        DeleteNodeCommand = new RelayCommand(execute => DeleteNode(), canExecute => CanDeleteNode());
        ConnectionClickedCommand = new RelayCommand(OnConnectionClicked);
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

    private void OnConnectionClicked(object param)
    {
        if (param is ConnectionViewModel connection)
        {
            ShowInsertNodeDialog(connection);
        }
    }

    private void ShowInsertNodeDialog(ConnectionViewModel connection)
    {
        var dialogService = ((App)Application.Current).Services.GetRequiredService<IDialogService>();
        var nodeType = dialogService.ShowInsertNodeDialog();

        if (nodeType != null)
        {
            InsertNodeIntoConnection(connection, nodeType);
        }
    }

    private void InsertNodeIntoConnection(ConnectionViewModel connection, Type nodeType)
    {
        // Create new node
        var newNode = (FlowchartNodeViewModel)Activator.CreateInstance(nodeType, _imageService);
        newNode.X = (connection.Source.X + connection.Target.X) / 2;
        newNode.Y = (connection.Source.Y + connection.Target.Y) / 2;

        // Handle image output if supported
        if (newNode is IImageOutputNode imageNode)
        {
            imageNode.ImageOutputted += (image) =>
                Application.Current.Dispatcher.Invoke(() =>
                    RegisterOutputImage(newNode, image));
        }

        // Update connections
        Connections.Remove(connection);
        Connections.Add(new ConnectionViewModel(connection.Source, newNode));
        Connections.Add(new ConnectionViewModel(newNode, connection.Target));

        // Add node to collection
        Nodes.Insert(Nodes.IndexOf(connection.Target), newNode);
    }

    private void RegisterOutputImage(FlowchartNodeViewModel node, BitmapImage image)
    {
        var imageData = new ImageNodeData(image, node);
        OutputImages.AddOrUpdate(node.NodeId, imageData);
    }

}
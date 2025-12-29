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
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;

namespace ImageProcessing.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IImageService _imageService;
    private readonly IFlowchartSerializationService _serializationService;
    private readonly IServiceProvider _serviceProvider;

    // Collection of flowchart nodes
    public ObservableCollection<IFlowchartNode> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
    public ObservableDictionary<string, ImageNodeData> OutputImages { get; } = new();

    // Commands
    public ICommand DeleteNodeCommand { get; }
    public ICommand ConnectionClickedCommand { get; }
    public ICommand SelectNodeCommand { get; }
    public ICommand PlayCommand { get; }
    public ICommand PlayNextCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SaveFlowchartCommand { get; }
    public ICommand LoadFlowchartCommand { get; }

    // Execution state
    private bool _isExecuting;
    public bool IsExecuting
    {
        get => _isExecuting;
        private set => SetProperty(ref _isExecuting, value);
    }

    private int _currentNodeIndex;

    private const double FLOWCHART_CENTER_X = 300;
    private const double FLOWCHART_START_Y = 100;
    private const double FLOWCHART_START_END_WIDTH = 10;
    private const double FLOWCHART_START_END_HEIGHT = 10;
    private const double FLOWCHART_NODES_GAP = 50;

    // Currently selected node (for context menus/properties)
    private FlowchartNodeViewModel? _selectedNode;
    public FlowchartNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set
        {
            // Deselect previous node
            if (_selectedNode != null)
                _selectedNode.IsSelected = false;

            // Select new node
            SetProperty(ref _selectedNode, value);
            if (_selectedNode != null)
                _selectedNode.IsSelected = true;
        }
    }

    public MainViewModel(IImageService imageService, IFlowchartSerializationService serializationService, IServiceProvider serviceProvider)
    {
        _imageService = imageService;
        _serializationService = serializationService;
        _serviceProvider = serviceProvider;
        _currentNodeIndex = 1;

        // Initialize start and end nodes
        var startNode = new StartNodeViewModel { X = FLOWCHART_CENTER_X,
            Y = FLOWCHART_START_Y,
            Width = FLOWCHART_START_END_WIDTH,
            Height = FLOWCHART_START_END_HEIGHT };

        var endNode = new EndNodeViewModel { X = FLOWCHART_CENTER_X, 
            Y = FLOWCHART_START_Y + (FLOWCHART_START_END_HEIGHT + FLOWCHART_START_END_HEIGHT) / 2 + FLOWCHART_NODES_GAP,
            Width = FLOWCHART_START_END_WIDTH,
            Height = FLOWCHART_START_END_HEIGHT };

        Nodes.Add(startNode);
        Nodes.Add(endNode);

        var connection = new ConnectionViewModel(startNode, endNode);
        Connections.Add(connection);

        // Initialize commands
        DeleteNodeCommand = new RelayCommand(execute => DeleteNode(), canExecute => CanDeleteNode());
        ConnectionClickedCommand = new RelayCommand(OnConnectionClicked);
        SelectNodeCommand = new RelayCommand(node =>
        {
            SelectedNode = node as FlowchartNodeViewModel;

            // Display the output image if the selected node is an ImageOutputNode and the OutputImage was generated
            if (node is IImageOutputNode imageNode && imageNode.OutputImage != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var imageWindow = new ImageProcessing.App.Views.ImageWindow();
                    imageWindow.SetImage(imageNode.OutputImage);
                    imageWindow.Owner = Application.Current.MainWindow;
                    imageWindow.Show();
                });
            }
        });
        PlayCommand = new RelayCommand(
            async _ => await ExecuteAll(),
            _ => !_isExecuting && Nodes.Any(n => n is IExecutableNode)
        );

        PlayNextCommand = new RelayCommand(
            async _ => await ExecuteNextNode(),
            _ => !_isExecuting && Nodes.Any(n => n is IExecutableNode) &&
            _currentNodeIndex < Nodes.Count
        );

        PauseCommand = new RelayCommand(
            _ => PauseExecution(),
            _ => _isExecuting
        );

        StopCommand = new RelayCommand(
            _ => StopExecution(),
            _ => _isExecuting
        );

        ResetCommand = new RelayCommand(
            _ => ResetExecution(),
            _ => !_isExecuting
        );

        SaveFlowchartCommand = new RelayCommand(
            _ => SaveFlowchart(),
            _ => !_isExecuting
        );

        LoadFlowchartCommand = new RelayCommand(
            _ => LoadFlowchart(),
            _ => !_isExecuting
        );
    }

    private async Task ExecuteAll()
    {
        IsExecuting = true;

        try
        {
            foreach (var node in Nodes
                                .Skip(_currentNodeIndex)
                                .OfType<IExecutableNode>())
            {
                if (!_isExecuting) break;
                await ExecuteNode(node);
                _currentNodeIndex++;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Execution failed: {ex.Message}");
        }
        finally
        {
            IsExecuting = false;
        }
    }

    private async Task ExecuteNextNode()
    {
        IsExecuting = true;
        try
        {
            var node = Nodes
                .Skip(_currentNodeIndex)
                .OfType<IExecutableNode>()
                .FirstOrDefault();

            if (node != null)
            {
                await ExecuteNode(node);
                _currentNodeIndex++;
            }
        }
        finally
        {
            IsExecuting = false;
        }
    }

    private async Task ExecuteNode(IExecutableNode node)
    {
        if (node.CanExecute())
        {
            node.IsExecuting = true;
            await Task.Run(() => node.Execute());
            node.IsExecuting = false;
        }
    }

    private void PauseExecution()
    {
        IsExecuting = false;
    }

    private void StopExecution()
    {
        IsExecuting = false;
        _currentNodeIndex = 1;
    }

    private void ResetExecution()
    {
        _currentNodeIndex = 1;
    }

    /// <summary>Removes the currently selected node</summary>
    private void DeleteNode()
    {
        if (SelectedNode == null) return;
        int selNodeIndex = indexOfNode(SelectedNode);
        Nodes.Remove(SelectedNode);
        OutputImages.Remove(SelectedNode.Label + ".OutputImage");

        // Shift the nodes after the deleted node upwards
        if (selNodeIndex > 0)
        {
            for (int i = selNodeIndex; i < Nodes.Count; i++)
            {
                var prevNode = Nodes[i - 1];
                var currentNode = Nodes[i];
                currentNode.Y = prevNode.Y + (prevNode.Height + currentNode.Height) / 2 + FLOWCHART_NODES_GAP;
            }
        }
        RedrawConnections();
        SelectedNode = null;
    }

    /// <summary>Determines if the Delete command can execute</summary>
    private bool CanDeleteNode() => SelectedNode != null;

    private void OnConnectionClicked(object? param)
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
        var newNode = (FlowchartNodeViewModel)((App)Application.Current).Services.GetRequiredService(nodeType);
        newNode.X = connection.Source.X; // Keep X of prev node
        newNode.Y = connection.Source.Y + (connection.Source.Height + newNode.Height) / 2 + FLOWCHART_NODES_GAP;

        // Add node to collection in place of the old connection's target
        Nodes.Insert(Nodes.IndexOf(connection.Target), newNode);

        // Shift all nodes after inserted one
        int nextNodeIndex = indexOfNode(connection.Target);
        if (nextNodeIndex > 0)
        {
            for (int i = nextNodeIndex; i < Nodes.Count; i++)
            {
                var prevNode = Nodes[i - 1];
                var currentNode = Nodes[i];
                currentNode.Y = prevNode.Y + (prevNode.Height + currentNode.Height) / 2 + FLOWCHART_NODES_GAP;
            }
        }

        // Handle image output if supported
        if (newNode is IImageOutputNode imageNode)
        {
            // Register output image when ImageOutputted event is raised
            imageNode.ImageOutputted += (image) =>
                Application.Current.Dispatcher.Invoke(() =>
                    RegisterOutputImage(newNode, image));

            // Register placeholder output image so it can be used in other nodes
            RegisterOutputImage(newNode, null);
        }

        RedrawConnections();
    }

    private void RegisterOutputImage(FlowchartNodeViewModel node, BitmapImage? image)
    {
        var imageData = new ImageNodeData(image, node);
        OutputImages.AddOrUpdate(node.Label+".OutputImage", imageData);
    }

    private int indexOfNode(IFlowchartNode node)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].Label == node.Label)
                return i;
        }
        return -1;
    }

    private void RedrawConnections()
    {
        Connections.Clear();
        for (int i = 0; i < Nodes.Count - 1; i++)
        {
            var conn = new ConnectionViewModel(Nodes[i], Nodes[i + 1]);
            Connections.Add(conn);
        }
    }

    private void SaveFlowchart()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Flowchart files (*.flowchart)|*.flowchart|JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = ".flowchart",
            Title = "Save Flowchart"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                _serializationService.SaveFlowchart(dialog.FileName, Nodes, Connections);
                //MessageBox.Show("Flowchart saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save flowchart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void LoadFlowchart()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Flowchart files (*.flowchart)|*.flowchart|JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Load Flowchart"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var flowchartDto = _serializationService.LoadFlowchart(dialog.FileName);
                ReconstructFlowchart(flowchartDto);
                // MessageBox.Show("Flowchart loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load flowchart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ReconstructFlowchart(FlowchartDTO flowchartDto)
    {
        // Clear existing flowchart
        Nodes.Clear();
        Connections.Clear();
        OutputImages.Clear();
        _currentNodeIndex = 1;

        // Reset static counters for all node types
        ResetNodeCounters();

        // Create factory
        var factory = new NodeFactory(_serviceProvider, OutputImages);

        // Dictionary to map IDs to reconstructed nodes
        var nodeMap = new Dictionary<int, IFlowchartNode>();

        // Reconstruct nodes
        foreach (var nodeDto in flowchartDto.Nodes)
        {
            var node = factory.CreateNode(nodeDto);
            if (node != null)
            {
                Nodes.Add(node);
                nodeMap[nodeDto.Id] = node;

                // Register output images for image output nodes
                if (node is IImageOutputNode imageNode)
                {
                    imageNode.ImageOutputted += (image) =>
                        Application.Current.Dispatcher.Invoke(() =>
                            RegisterOutputImage((FlowchartNodeViewModel)node, image));

                    RegisterOutputImage((FlowchartNodeViewModel)node, null);
                }
            }
        }

        // Reconstruct connections
        foreach (var connectionDto in flowchartDto.Connections)
        {
            if (nodeMap.TryGetValue(connectionDto.SourceNodeId, out var source) &&
                nodeMap.TryGetValue(connectionDto.TargetNodeId, out var target))
            {
                Connections.Add(new ConnectionViewModel(source, target));
            }
        }

        RedrawConnections();

        // Update counters based on loaded nodes
        UpdateNodeCounters();
    }

    private void ResetNodeCounters()
    {
        // Use reflection to reset static counters in node ViewModels
        ResetCounter(typeof(LoadImageNodeViewModel));
        ResetCounter(typeof(GrayscaleNodeViewModel));
        ResetCounter(typeof(ResizeNodeViewModel));
        ResetCounter(typeof(BinarizeNodeViewModel));
    }

    private void ResetCounter(Type nodeType)
    {
        var counterField = nodeType.GetField("_counter", BindingFlags.NonPublic | BindingFlags.Static);
        if (counterField != null)
        {
            counterField.SetValue(null, 0);
        }
    }

    private void UpdateNodeCounters()
    {
        // Update counters to match the highest ID loaded
        var maxId = new Dictionary<Type, int>();

        foreach (var node in Nodes)
        {
            var nodeType = node.GetType();
            if (node is FlowchartNodeViewModel vm)
            {
                if (!maxId.ContainsKey(nodeType) || vm.Id > maxId[nodeType])
                {
                    maxId[nodeType] = vm.Id;
                }
            }
        }

        foreach (var kvp in maxId)
        {
            var counterField = kvp.Key.GetField("_counter", BindingFlags.NonPublic | BindingFlags.Static);
            if (counterField != null)
            {
                counterField.SetValue(null, kvp.Value);
            }
        }
    }
}
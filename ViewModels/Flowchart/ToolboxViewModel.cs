using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Utilities;
using System.Collections.ObjectModel;

namespace ImageProcessing.App.ViewModels.Flowchart;

public class ToolboxViewModel : ViewModelBase
{
    public ObservableCollection<ToolboxItem> ToolboxItems { get; } = new();

    public ToolboxViewModel()
    {
        InitializeToolboxItems();
    }

    private void InitializeToolboxItems()
    {
        ToolboxItems.Add(new ToolboxItem(
            "Load Image",
            typeof(LoadImageNodeViewModel) 
        ));
    }
}
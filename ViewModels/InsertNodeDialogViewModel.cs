using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels.Flowchart;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ImageProcessing.App.ViewModels
{
    public class InsertNodeDialogViewModel : ViewModelBase
    {
        public event Action? CloseDialogRequested;
        public ObservableCollection<ToolboxItem> AvailableNodeTypes { get; } = new();
        public ToolboxItem SelectedNodeType { get; set; }
        public ICommand OkCommand { get; }

        public InsertNodeDialogViewModel()
        {
            AvailableNodeTypes.Add(new ToolboxItem("Load Image", typeof(LoadImageNodeViewModel)));
            // Add other node types

            OkCommand = new RelayCommand(_ =>
            {
                CloseDialogRequested?.Invoke();
            });

        }
    }
}

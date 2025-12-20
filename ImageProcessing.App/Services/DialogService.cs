using ImageProcessing.App.Views;

namespace ImageProcessing.App.Services
{
    public class DialogService : IDialogService
    {
        public Type? ShowInsertNodeDialog()
        {
            var dialog = new InsertNodeDialog();
            return dialog.ShowDialog() == true ? dialog.SelectedNodeType : null;
        }
    }
}

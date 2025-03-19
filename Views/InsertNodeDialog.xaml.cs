using ImageProcessing.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageProcessing.App.Views
{
    /// <summary>
    /// Interaction logic for InsertNodeDialog.xaml
    /// </summary>
    public partial class InsertNodeDialog : Window
    {
        public Type? SelectedNodeType => ((InsertNodeDialogViewModel)DataContext).SelectedNodeType?.NodeType;

        public InsertNodeDialog()
        {
            InitializeComponent();
            DataContext = ((App)Application.Current).Services.GetRequiredService<InsertNodeDialogViewModel>();
            if (DataContext is InsertNodeDialogViewModel vm)
            {
                vm.CloseDialogRequested += OnCloseDialogRequested;
            }
        }

        private void OnCloseDialogRequested()
        {
            // Ensure the window is loaded and shown before setting DialogResult
            if (!IsLoaded)
            {
                // If window isn't loaded yet, defer the close operation
                Loaded += (s, e) => CloseWithResult(true);
            }
            else
            {
                CloseWithResult(true);
            }
        }

        private void CloseWithResult(bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}

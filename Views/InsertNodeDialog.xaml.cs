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
                vm.CloseDialogRequested += () =>
                {
                    this.DialogResult = true; // Set on THIS dialog window
                    this.Close();
                };
            }
        }
    }
}

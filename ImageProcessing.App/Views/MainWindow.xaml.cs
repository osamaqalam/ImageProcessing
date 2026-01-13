using ImageProcessing.App.ViewModels;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessing.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Enable data binding in MainWindow.xaml to attributes defined in MainViewModel.cs
            // This implements concept of Dependency Injection which promotes seperation of concerns
            DataContext = ((App)Application.Current).Services.GetRequiredService<MainViewModel>();
        }

        private void ImageComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is ImageOutputNodeViewModel vm)
            {
                vm.RefreshFilter();
            }
        }
    }
}
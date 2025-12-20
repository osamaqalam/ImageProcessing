using ImageProcessing.App.ViewModels.Flowchart;
using System.Windows.Controls;


namespace ImageProcessing.App.Views.Flowchart.Nodes
{
    /// <summary>
    /// Interaction logic for GrayscaleNodeView.xaml
    /// </summary>
    public partial class GrayscaleNodeView : UserControl
    {
        public GrayscaleNodeView()
        {
            InitializeComponent();
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (DataContext is GrayscaleNodeViewModel vm)
            {
                vm.RefreshFilter();
            }
        }
    }
}

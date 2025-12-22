using ImageProcessing.App.ViewModels.Flowchart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessing.App.Views.Flowchart.Nodes
{
    /// <summary>
    /// Interaction logic for BinarizeNodeView.xaml
    /// </summary>
    public partial class BinarizeNodeView : UserControl
    {
        public BinarizeNodeView()
        {
            InitializeComponent();
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (DataContext is BinarizeNodeViewModel vm)
            {
                vm.RefreshFilter();
            }
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                // Only allow digits
                if (!Regex.IsMatch(pastedText, @"^\d+$"))
                {
                    e.CancelCommand();
                    return;
                }
                // Check numeric range
                if (int.TryParse(pastedText, out int value))
                {
                    if (value < 0 || value > 255)
                    {
                        e.CancelCommand();
                    }
                }
                else
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

    }
}

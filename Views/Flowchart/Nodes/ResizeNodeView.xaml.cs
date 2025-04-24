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
    /// Interaction logic for ResizeNodeView.xaml
    /// </summary>
    public partial class ResizeNodeView : UserControl
    {
        public ResizeNodeView()
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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // Allow digits, negative sign, and a single decimal point
            bool isValid = Regex.IsMatch(newText, @"^-?\d*\.?\d*$");
            e.Handled = !isValid;
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(pastedText, @"^-?\d*\.?\d*$"))
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

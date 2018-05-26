using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ExcelToWordAutoFill.Views
{
    /// <summary>
    /// Interaction logic for AddFormView.xaml
    /// </summary>
    public partial class AddFormView : UserControl
    {
        public AddFormView()
        {
            InitializeComponent();
        }


        private void Button1_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                sourceExcelTextBox.Text = dialog.FileName;
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                formModelTextBox.Text = dialog.FileName;
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputFolderTextBox.Text = dialog.SelectedPath;
            }
        }

    }
}

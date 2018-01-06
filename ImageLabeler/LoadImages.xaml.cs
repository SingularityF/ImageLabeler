using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ImageLabeler
{
    /// <summary>
    /// Interaction logic for LoadImages.xaml
    /// </summary>

    public enum PathForms { Full, Name };
    public partial class LoadImages : Window
    {
        public List<DefItem> defs;
        public string[] files;
        public string output;

        public LoadImages()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image files|*.*";
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == true)
            {
                files = openFile.FileNames;
                FilePathDisp.Text = files[0] + " ...";
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilePathDisp.Text == "")
            {
                MessageBox.Show("No images selected.", "Error");
                return;
            }
            if (OutputPathDisp.Text == "")
            {
                MessageBox.Show("No output selected.", "Error");
                return;
            }
            MainWindow mainWnd = new MainWindow();
            mainWnd.defs = defs;
            mainWnd.files = files;
            mainWnd.output = output;
            if (FullSelect.IsChecked == true)
            {
                mainWnd.pathForm = PathForms.Full;
            }
            else
            {
                mainWnd.pathForm = PathForms.Name;
            }
            mainWnd.Left = Left;
            mainWnd.Top = Top;
            mainWnd.Show();
            Close();
        }

        private void SelectOutputBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "csv files (*.csv)|*.csv";
            saveFile.DefaultExt = "csv";
            saveFile.AddExtension = true;
            if (saveFile.ShowDialog() == true)
            {
                OutputPathDisp.Text = saveFile.FileName;
                output = saveFile.FileName;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Definition defWnd = new Definition();
            defWnd.Left = Left;
            defWnd.Top = Top;
            defWnd.Show();
            Close();
        }
    }
}

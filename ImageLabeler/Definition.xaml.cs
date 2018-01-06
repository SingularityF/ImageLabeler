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
    /// Interaction logic for Definition.xaml
    /// </summary>
    public partial class Definition : Window
    {


        public Definition()
        {
            InitializeComponent();
        }

        private void AddDefBtn_Click(object sender, RoutedEventArgs e)
        {
            AddRow();
        }

        private void AddRow()
        {
            StackPanel defrow = FindResource("DefRow") as StackPanel;
            DefTab.Children.Add(defrow);
        }

        private void DiscardBtn_Click(object sender, RoutedEventArgs e)
        {
            DiscardChanges();
        }

        private void DiscardChanges()
        {
            DefTab.Children.Clear();
        }

        private void AssignKey(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            KeyReader reader = new KeyReader();
            reader.ShowDialog();
            lab.Content = reader.retKey.ToString();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            List<DefItem> defs = new List<DefItem>();
            foreach (var panel in DefTab.Children)
            {
                var defrow = panel as StackPanel;
                DefItem item = new DefItem();
                var def1 = defrow.Children[0] as Label;
                var def2 = defrow.Children[1] as TextBox;
                var def3 = defrow.Children[2] as TextBox;
                if (def1.Content == null || def1.Content.ToString() == "None" || def1.Content.ToString() == "Press to assign"
                    || def2.Text == "" || def3.Text == "")
                {
                    MessageBox.Show("Missing values.", "Error");
                    return;
                }
                item.KeyName = def1.Content.ToString();
                item.DispName = def2.Text;
                item.ClassName = def3.Text;
                defs.Add(item);
            }
            if (defs.Count == 0)
            {
                MessageBox.Show("Nothing to export.", "Error");
                return;
            }
            if (defs.Count != defs.Distinct().Count())
            {
                MessageBox.Show("Conflicting values.", "Error");
                return;
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Definition files (*.def)|*.def";
            saveFile.DefaultExt = "def";
            saveFile.AddExtension = true;
            if (saveFile.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    StreamWriter writer = new StreamWriter(fs);
                    foreach (var item in defs)
                    {
                        writer.WriteLine(item.KeyName);
                        writer.WriteLine(item.DispName);
                        writer.WriteLine(item.ClassName);
                        writer.Flush();
                    }
                }
            }
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Definition files (*.def)|*.def";
            if (openFile.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open))
                {
                    List<DefItem> defs = new List<DefItem>();
                    StreamReader reader = new StreamReader(fs);
                    while (!reader.EndOfStream)
                    {
                        var keyname = reader.ReadLine();
                        var dispname = reader.ReadLine();
                        var classname = reader.ReadLine();
                        DefItem item = new DefItem();
                        item.KeyName = keyname;
                        item.DispName = dispname;
                        item.ClassName = classname;
                        defs.Add(item);
                    }
                    DiscardChanges();
                    for (int i = 0; i < defs.Count; i++)
                    {
                        AddRow();
                        var defrow = DefTab.Children[DefTab.Children.Count - 1] as StackPanel;
                        var def1 = defrow.Children[0] as Label;
                        var def2 = defrow.Children[1] as TextBox;
                        var def3 = defrow.Children[2] as TextBox;
                        def1.Content = defs[i].KeyName;
                        def2.Text = defs[i].DispName;
                        def3.Text = defs[i].ClassName;
                    }
                }
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            List<DefItem> defs = new List<DefItem>();
            foreach (var panel in DefTab.Children)
            {
                var defrow = panel as StackPanel;
                DefItem item = new DefItem();
                var def1 = defrow.Children[0] as Label;
                var def2 = defrow.Children[1] as TextBox;
                var def3 = defrow.Children[2] as TextBox;
                if (def1.Content == null || def1.Content.ToString() == "None" || def1.Content.ToString() == "Press to assign"
                    || def2.Text == "" || def3.Text == "")
                {
                    MessageBox.Show("Missing values.", "Error");
                    return;
                }
                item.KeyName = def1.Content.ToString();
                item.DispName = def2.Text;
                item.ClassName = def3.Text;
                defs.Add(item);
            }
            if (defs.Count == 0)
            {
                MessageBox.Show("No definitions given.", "Error");
                return;
            }

            if (defs.Count != defs.Distinct().Count())
            {
                MessageBox.Show("Conflicting values.", "Error");
                return;
            }
            LoadImages loadimgWnd = new LoadImages();
            loadimgWnd.defs = defs;
            loadimgWnd.Left = Left;
            loadimgWnd.Top = Top;
            loadimgWnd.Show();
            Close();
        }
    }
}

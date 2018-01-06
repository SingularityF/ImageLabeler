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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CsvHelper;

namespace ImageLabeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<DefItem> defs;
        public PathForms pathForm;
        public string[] files;
        public string output;
        public int[] labs;
        int curPos = 0;
        ImageSource imgLeft;
        ImageSource imgMid;
        ImageSource imgRight;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddReference();
            if (File.Exists(output) && Resume())
            {
                ;
            }
            else
            {
                labs = new int[files.Length];
                for (int i = 0; i < labs.Length; i++)
                {
                    labs[i] = -1;
                }
                RefreshDisplay();
            }
        }

        private void AddReference()
        {
            foreach (DefItem item in defs)
            {
                Grid refrow = FindResource("RefRow") as Grid;
                RefTab.Children.Add(refrow);
                Grid added = RefTab.Children[RefTab.Children.Count - 1] as Grid;
                var keyborder = added.Children[0] as Border;
                var key = keyborder.Child as Label;
                key.Content = item.KeyName;
                var name = added.Children[1] as Label;
                name.Content = item.DispName;
            }
        }

        private void RefreshDisplay()
        {
            UpdateImage();
            UpdateProgress();
            ShowLabel();
        }

        private bool Resume()
        {
            try
            {
                using (FileStream fs = new FileStream(output, FileMode.Open))
                {
                    var reader = new StreamReader(fs);
                    var csv = new CsvReader(reader);
                    int rowcnt = 0;
                    int firstunlab = -1;
                    List<int> labsCache = new List<int>();
                    while (csv.Read())
                    {
                        if (pathForm == PathForms.Full)
                        {
                            if (files[rowcnt] != csv.GetField(0))
                            {
                                MessageBox.Show("Inconsistent output file, will overwrite.\nGo back to review configurations if this is not what you want.", "Error");
                                return false;
                            }
                        }
                        else
                        {
                            if (System.IO.Path.GetFileName(files[rowcnt]) != csv.GetField(0))
                            {
                                MessageBox.Show("Inconsistent output file, will overwrite.\nGo back to review configurations if this is not what you want.", "Error");
                                return false;
                            }
                        }
                        if (csv.GetField(1) == "")
                        {
                            labsCache.Add(-1);
                        }
                        else
                        {
                            for(int i = 0; i < defs.Count; i++)
                            {
                                if (defs[i].ClassName == csv.GetField(1))
                                {
                                    labsCache.Add(i);
                                    break;
                                }
                            }
                        }
                        rowcnt++;
                    }
                    if (rowcnt != files.Length)
                    {
                        MessageBox.Show("Inconsistent output file, will overwrite.\nGo back to review configurations if this is not what you want.", "Error");
                        return false;
                    }
                    labs = new int[labsCache.Count];
                    for (int i = 0; i < labsCache.Count; i++)
                    {
                        labs[i] = labsCache[i];
                        if (labsCache[i] == -1)
                        {
                            firstunlab = i;
                        }
                    }
                    if (firstunlab == -1)
                    {
                        firstunlab = labsCache.Count - 1;
                    }
                    curPos = firstunlab;

                    RefreshDisplay();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot open output file.", "Error");
                GoBack();
            }
            return true;
        }

        private async Task SaveOutput()
        {
            int[] curLabs = labs;
            try
            {
                using (FileStream fs = new FileStream(output, FileMode.OpenOrCreate))
                {
                    var writer = new StreamWriter(fs);
                    var csv = new CsvWriter(writer);
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (pathForm == PathForms.Full)
                        {
                            csv.WriteField(files[i]);
                        }
                        else
                        {
                            csv.WriteField(System.IO.Path.GetFileName(files[i]));
                        }
                        if (curLabs[i] != -1)
                        {
                            csv.WriteField(defs[curLabs[i]].ClassName);
                        }
                        else
                        {
                            csv.WriteField("");
                        }
                        csv.NextRecord();
                        await csv.FlushAsync();
                    }
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot open output file.", "Error");
                return;
            }
            MessageBox.Show("Successfully saved.", "Notice");
        }
        private void UpdateImage()
        {
            try
            {
                imgMid = new BitmapImage(new Uri(files[curPos]));
                DispMid.Source = imgMid;
                if ((curPos + 1) <= (files.Length - 1))
                {
                    imgRight = new BitmapImage(new Uri(files[curPos + 1]));
                    DispRight.Source = imgRight;
                }
                else
                {
                    DispRight.Source = null;
                }
                if ((curPos - 1) >= 0)
                {
                    imgLeft = new BitmapImage(new Uri(files[curPos - 1]));
                    DispLeft.Source = imgLeft;
                }
                else
                {
                    DispLeft.Source = null;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load image.\nAborting.", "Error");
                GoBack();
            }
        }
        private void NextImage()
        {
            if ((curPos + 1) <= (files.Length - 1))
            {
                curPos++;
                RefreshDisplay();
            }

        }
        private void PrevImage()
        {
            if ((curPos - 1) >= 0)
            {
                curPos--;
                RefreshDisplay();
            }
        }

        private void UpdateProgress()
        {
            string progress = (curPos + 1).ToString() + " / " + files.Length.ToString();
            ProgressDisp.Content = progress;
        }
        private void ShowLabel()
        {
            if (labs[curPos] == -1)
            {
                LabelDisp.Content = "N/A";
            }
            else
            {
                LabelDisp.Content = defs[labs[curPos]].DispName;
            }
        }

        private void DispLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PrevImage();
        }

        private void DispRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NextImage();
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            await SaveOutput();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            for (int i = 0; i < defs.Count; i++)
            {
                if (e.Key == (Key)Enum.Parse(typeof(Key), defs[i].KeyName))
                {
                    labs[curPos] = i;
                    NextImage();
                    ShowLabel();
                    return;
                }
            }
            if (e.Key == Key.Right)
            {
                NextImage();
            }
            else if (e.Key == Key.Left)
            {
                PrevImage();
            }
        }

        private void ToNextBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (labs[i] == -1)
                {
                    curPos = i;
                    RefreshDisplay();
                    return;
                }
            }
        }

        private void GoBackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
        private void GoBack()
        {
            LoadImages loadimgWnd = new LoadImages();
            loadimgWnd.defs = defs;
            loadimgWnd.Left = Left;
            loadimgWnd.Top = Top;
            loadimgWnd.Show();
            Close();
        }

        private void ToFirstBtn_Click(object sender, RoutedEventArgs e)
        {
            curPos = 0;
            RefreshDisplay();
        }

        private void ToLastBtn_Click(object sender, RoutedEventArgs e)
        {
            curPos = files.Length - 1;
            RefreshDisplay();
        }
    }
}

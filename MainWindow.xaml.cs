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
using System;
using System.IO;
using System;
using Microsoft.Win32;
using Path = System.IO.Path;


namespace MergeImagesLogFast
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("dasdsa");
        }

        private void btn_SearchFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFileDialog
            {
                Title = "Select Folder",
                //Filter = "Folders|*.none",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(folderDialog.FileName);
                PopulateListView(selectedPath);
            }
        }
        private void PopulateListView(string folderPath)
        {
            ListViewImagens.Items.Clear();

            try
            {
                string[] files = Directory.GetFiles(folderPath);
                string[] directories = Directory.GetDirectories(folderPath);

                foreach (string file in files)
                {
                    ListViewImagens.Items.Add(new ListViewItem { Content = Path.GetFileName(file) });
                }

                foreach (string directory in directories)
                {
                    ListViewImagens.Items.Add(new ListViewItem { Content = Path.GetFileName(directory) });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading folder content: {ex.Message}");
            }
        }
    }
}
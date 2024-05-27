using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;

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

                GlobalVariables.pathIni = selectedPath;
            }
        }
        private void PopulateListView(string folderPath)
        {
            LVImagens.Items.Clear();
            GlobalVariables.FilesImagens = new List<string>();

            try
            {
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                     .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png"));

                GlobalVariables.FilesImagens = files;

                foreach (string file in files)
                {
                    LarguraAlturaImagem(file);
                    LVImagens.Items.Add(new ListViewItem { Content = Path.GetFileName(file) });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading folder content: {ex.Message}");
            }
        }

        private void LarguraAlturaImagem(string file)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(file));

            long AlturaImagem = Convert.ToInt64(bitmap.Height);
            long LarguraImagem = Convert.ToInt64(bitmap.Width);

            if (GlobalVariables.ImgMaxWidth < AlturaImagem)
                GlobalVariables.ImgMaxWidth = AlturaImagem;

            if (GlobalVariables.ImgMaxHeight < LarguraImagem)
                GlobalVariables.ImgMaxHeight = LarguraImagem;

            GlobalVariables.CombineImgMaxHeight += AlturaImagem;
            GlobalVariables.CombineImgMaxWidth += LarguraImagem;


            TBAltura.Text = GlobalVariables.ImgMaxWidth.ToString();
            TBLargura.Text = GlobalVariables.ImgMaxHeight.ToString();

            LAlturaComb.Content = GlobalVariables.CombineImgMaxHeight.ToString();
            LLarguraComb.Content = GlobalVariables.CombineImgMaxWidth.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PreviewTextInputForInt(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void ListViewImagens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BMesclar_Click(object sender, RoutedEventArgs e)
        {
            var path = GlobalVariables.pathIni;
            if (path == null)
                return;

            string[] imageFiles = GlobalVariables.FilesImagens.ToArray();
            if (imageFiles.Length < 2)
            {
                MessageBox.Show("Imagens insuficientes!");
                return;
            }

            string NomePadraoImagem = "Combined_";

            bool vertical = true;

            if (RB_Vertical.IsChecked == true)
                vertical = true;
            else
                vertical = false;

            // Agrupa as imagens em lotes
            List<string[]> groupedImages = new List<string[]>();
            int groupSize = Convert.ToInt32(TBLotes.Text);
            int numGroups = (int)Math.Ceiling((double)imageFiles.Length / groupSize);
            for (int i = 0; i < numGroups; i++)
            {
                int groupStartIndex = i * groupSize;
                int groupEndIndex = Math.Min(groupStartIndex + groupSize, imageFiles.Length);
                string[] group = new string[groupEndIndex - groupStartIndex];
                Array.Copy(imageFiles, groupStartIndex, group, 0, group.Length);
                groupedImages.Add(group);
            }


            int grupoImagem = 0;
            foreach (var imagePaths in groupedImages)
            {
                List<Bitmap> images = LoadImages(imagePaths);

                Bitmap combinedImage = CombineImages(images, vertical);

                combinedImage.Save($"D:\\Teste\\{NomePadraoImagem}{grupoImagem}.jpg");

                grupoImagem++;
            }

        }

        private Bitmap CombineImages(List<Bitmap> images, bool vertical)
        {
            int totalWidth = 0;
            int totalHeight = 0;

            // Calcula a largura e altura totais
            foreach (Bitmap image in images)
            {
                totalWidth = vertical ? Math.Max(totalWidth, image.Width) : totalWidth + image.Width;
                totalHeight = vertical ? totalHeight + image.Height : Math.Max(totalHeight, image.Height);
            }

            // Cria a imagem combinada
            Bitmap combinedImage = new Bitmap(totalWidth, totalHeight);

            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                int currentX = 0;
                int currentY = 0;

                foreach (Bitmap image in images)
                {
                    if (vertical)
                    {
                        g.DrawImage(image, new Point(0, currentY));
                        currentY += image.Height;
                    }
                    else
                    {
                        g.DrawImage(image, new Point(currentX, 0));
                        currentX += image.Width;
                    }
                }
            }

            return combinedImage;
        }

        private List<Bitmap> LoadImages(string[] imagePaths)
        {
            List<Bitmap> images = new List<Bitmap>();

            foreach (string path in imagePaths)
            {
                images.Add(new Bitmap(path));
            }

            return images;
        }

        private void RB_Vertical_Checked(object sender, RoutedEventArgs e)
        {
            DesabilitaTextBoxAlturaLargura(0);
        }

        private void RB_Horizontal_Checked(object sender, RoutedEventArgs e)
        {
            DesabilitaTextBoxAlturaLargura(1);
        }

        private void DesabilitaTextBoxAlturaLargura(int v)
        {
            if (v == 0)
            {
                TBAltura.IsEnabled = false;
                TBLargura.IsEnabled = true;
            }
            else
            {
                TBAltura.IsEnabled = true;
                TBLargura.IsEnabled = false;
            }
        }
    }
}
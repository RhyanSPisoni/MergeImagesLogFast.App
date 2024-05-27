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
using static System.Net.WebRequestMethods;

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

                if (GlobalVariables.FilesImagens.Count() > 2)
                {
                    BMesclar.IsEnabled = true;
                    BCalcularLote.IsEnabled = true;
                }
                else
                {
                    BMesclar.IsEnabled = true;
                    BCalcularLote.IsEnabled = false;
                }
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

                Array.Sort(files.ToArray(), CompareFileNamesNumerically);

                GlobalVariables.FilesImagens = files;

                foreach (string file in files)
                {
                    LVImagens.Items.Add(new ListViewItem { Content = $"{Path.GetFileName(file)} {LarguraAlturaImagem(file)}" });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading folder content: {ex.Message}");
            }
        }

        private string LarguraAlturaImagem(string file)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(file));

            long AlturaImagem = Convert.ToInt64(bitmap.Height);
            long LarguraImagem = Convert.ToInt64(bitmap.Width);

            GlobalVariables.CombineImgMaxHeight += AlturaImagem;
            GlobalVariables.CombineImgMaxWidth += LarguraImagem;

            LAlturaComb.Content = GlobalVariables.CombineImgMaxHeight.ToString();
            LLarguraComb.Content = GlobalVariables.CombineImgMaxWidth.ToString();

            return $"| H: {AlturaImagem} W: {LarguraImagem}";
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

            // Abrir dialog pasta
            var folderDialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder"
            };

            if (folderDialog.ShowDialog() == false)
                return;

            GlobalVariables.pathOutput = folderDialog.FileName.Replace("Folder", "");
            string NomePadraoImagem = TBNomePadrao.Text;

            bool vertical;

            if (RB_Vertical.IsChecked == true)
                vertical = true;
            else
                vertical = false;

            // Agrupa as imagens em lotes
            List<string[]> groupedImages = AgrupaImagensLotes(imageFiles);

            int grupoImagem = 0;
            foreach (var imagePaths in groupedImages)
            {
                List<Bitmap> images = LoadImages(imagePaths);
                Bitmap combinedImage = CombineImages(images, vertical);
                combinedImage.Save($"{folderDialog.FileName.Replace("Folder", "")}\\{NomePadraoImagem}{grupoImagem}.jpg");
                grupoImagem++;
            }

            BMesclados.IsEnabled = true;
        }

        private List<string[]> AgrupaImagensLotes(string[] imageFiles)
        {
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

            return groupedImages;
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

        private void BCalcularLote_Click(object sender, RoutedEventArgs e)
        {
            string[] imageFiles = GlobalVariables.FilesImagens.ToArray();

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


            int v = 0;
            string NomePadrao = TBNomePadrao.Text;
            foreach (var file in groupedImages)
            {
                int altura = 0;
                int largura = 0;
                foreach (var item in file)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(item));

                    altura += Convert.ToInt32(bitmap.Height);
                    largura += Convert.ToInt32(bitmap.Width);
                }

                LVModeloImagem.Items.Add(new ListViewItem { Content = $"{NomePadrao}{v} | H: {altura} W: {largura}" });

                v++;
            }
        }

        public int CompareFileNamesNumerically(string a, string b)
        {
            // Extrai os números dos nomes de arquivo
            int numberA = ExtractNumber(Path.GetFileNameWithoutExtension(a));
            int numberB = ExtractNumber(Path.GetFileNameWithoutExtension(b));
            // Compara os números extraídos
            return numberA.CompareTo(numberB);
        }

        private int ExtractNumber(string s)
        {
            // Extrai os dígitos da string
            string number = "";
            foreach (char c in s)
            {
                if (char.IsDigit(c))
                    number += c;
            }
            // Converte a string de dígitos em um número inteiro
            return int.Parse(number);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", GlobalVariables.pathOutput);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var donate = new Donate();
            donate.Owner = this;
            donate.Show();
        }
    }
}
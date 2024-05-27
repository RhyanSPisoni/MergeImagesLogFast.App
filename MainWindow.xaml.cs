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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

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

            try
            {
                string[] files = Directory.GetFiles(folderPath);
                string[] directories = Directory.GetDirectories(folderPath);

                foreach (string file in files)
                {
                    LVImagens.Items.Add(new ListViewItem { Content = Path.GetFileName(file) });
                }

                foreach (string directory in directories)
                {
                    LVImagens.Items.Add(new ListViewItem { Content = Path.GetFileName(directory) });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading folder content: {ex.Message}");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        static void MesclaImagem()
        {
            // Diretório onde estão as imagens
            string inputDir = "./imagens/";
            // Diretório onde será salva a imagem combinada
            string outputDir = "./output/";

            // Largura desejada para as imagens
            int desiredWidth = 900;

            // Lista apenas os arquivos JPEG no diretório e ordena-os numericamente
            string[] imageFiles = Directory.GetFiles(inputDir, "*.jpg");
            Array.Sort(imageFiles, CompareFileNamesNumerically);

            // Agrupa as imagens em lotes de 5
            List<string[]> groupedImages = new List<string[]>();
            int groupSize = 5;
            int numGroups = (int)Math.Ceiling((double)imageFiles.Length / groupSize);
            for (int i = 0; i < numGroups; i++)
            {
                int groupStartIndex = i * groupSize;
                int groupEndIndex = Math.Min(groupStartIndex + groupSize, imageFiles.Length);
                string[] group = new string[groupEndIndex - groupStartIndex];
                Array.Copy(imageFiles, groupStartIndex, group, 0, group.Length);
                groupedImages.Add(group);
            }

            // Combina as imagens verticalmente
            int imageIndex = 1;
            foreach (var group in groupedImages)
            {
                int totalHeight = 0;
                int maxWidth = 0;

                // Carrega as imagens e calcula a altura total e a largura máxima
                foreach (string imagePath in group)
                {
                    using (var image = Image.Load(imagePath))
                    {
                        maxWidth = Math.Max(maxWidth, image.Width);
                        totalHeight += image.Height;
                    }
                }

                // Redimensiona as imagens e combina verticalmente
                using (var outputImage = new Image<Rgba32>(maxWidth, totalHeight))
                {
                    int y = 0;
                    foreach (string imagePath in group)
                    {
                        using (var image = Image.Load(imagePath))
                        {
                            // Redimensiona a imagem para a largura desejada
                            image.Mutate(x => x.Resize(desiredWidth, (int)((double)desiredWidth / image.Width * image.Height)));

                            // Combina a imagem na imagem de saída
                            outputImage.Mutate(x => x.DrawImage(image, new Point(0, y), 1f));

                            // Atualiza a posição y para a próxima imagem
                            y += image.Height;
                        }
                    }

                    // Salva a imagem combinada
                    string outputPath = Path.Combine(outputDir, $"combined_{imageIndex}.jpg");
                    outputImage.Save(outputPath);
                    Console.WriteLine($"Imagem combinada {imageIndex} salva em: {outputPath}");
                }

                imageIndex++;
            }
        }

        // Função de comparação para ordenar os nomes de arquivo numericamente
        static int CompareFileNamesNumerically(string a, string b)
        {
            // Extrai os números dos nomes de arquivo
            int numberA = ExtractNumber(Path.GetFileNameWithoutExtension(a));
            int numberB = ExtractNumber(Path.GetFileNameWithoutExtension(b));
            // Compara os números extraídos
            return numberA.CompareTo(numberB);
        }

        // Função auxiliar para extrair números de strings
        static int ExtractNumber(string s)
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

            long valueAltura = Convert.ToInt64(TBAltura.Text);
            long valueLargura = Convert.ToInt64(TBLargura.Text);

            if (RB_Vertical.IsChecked == true) //Vertical
            {

            }
            else //Horizontal
            {

            }
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
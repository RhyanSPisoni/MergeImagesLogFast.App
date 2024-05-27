using System;
using System.Collections.Generic;
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

namespace MergeImagesLogFast
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class Donate : Window
    {
        public Donate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("ad37b8e1-73fa-414c-aee3-7e96e32e93f7");
        }
    }
}

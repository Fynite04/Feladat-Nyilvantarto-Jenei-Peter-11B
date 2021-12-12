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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace FeladatNyilvantarto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CheckBox> feladatokLista = new List<CheckBox>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // A feladatSzovegInput-ba beírt szöveg hozzáadása a
        // feladatokListaLBx ListBox-hoz CheckBox-ként
        private void ujHozzadasaBtn_Click(object sender, RoutedEventArgs e)
        {
            string ujFeladatSzoveg = feladatSzovegInput.Text;

            feladatokListaLBx.Items.Clear();
            feladatokLista.Add(new CheckBox() { Content = ujFeladatSzoveg});
            
            for (int i = 0; i < feladatokLista.Count; i++)
            {
                feladatokLista[i].Checked += CheckBox_EventHandler;
                feladatokLista[i].Unchecked += CheckBox_EventHandler;
                feladatokListaLBx.Items.Add(feladatokLista[i]);
            }

            feladatSzovegInput.Clear();
        }

        // Checkboxok szövegének kezelése a checkbox állapotától függően
        private void CheckBox_EventHandler(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in feladatokListaLBx.Items)
            {
                if (cb.IsChecked == true)
                {
                    cb.FontStyle = FontStyles.Italic;
                    cb.Foreground = Brushes.Gray;
                }
                else
                {
                    cb.FontStyle = FontStyles.Normal;
                    cb.Foreground = Brushes.Black;
                }
            }
        }
    }
}

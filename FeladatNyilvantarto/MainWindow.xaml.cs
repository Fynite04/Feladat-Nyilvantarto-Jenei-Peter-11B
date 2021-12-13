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
        List<CheckBox> toroltFelLista = new List<CheckBox>();

        string kijeloltFeladat = "";
        string kijeloltToroltFel = "";

        public MainWindow()
        {
            InitializeComponent();
            AdatfajlKezeles();
        }

        private void AdatfajlKezeles()
        {
            if (File.Exists("feladatok.dat"))
            {
                CbListaFeltoltes(this.feladatokLista, "feladatok.dat");
                ListBoxokFeltoltese();
            }
            else
            {
                File.Create("feladatok.dat");
            }

            if (File.Exists("toroltFeladatok.dat"))
            {
                CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");
                ListBoxokFeltoltese();
            }
            else
            {
                File.Create("toroltFeladatok.dat");
            }
        }

        private void CbListaFeltoltes(List<CheckBox> lista, string adatFajl)
        {
            List<string> felSorok = File.ReadAllLines(adatFajl).ToList();
            lista.Clear();

            foreach (string f in felSorok)
            {
                if (f != "")
                {
                    string[] fArr = f.Split('|');
                    bool isChecked = fArr[1] == "1" ? true : false;
                    lista.Add(new CheckBox { Content = fArr[0], IsChecked = isChecked });
                }
            }
        }

        private void ListBoxokFeltoltese()
        {
            // Feladatok ListBox feltöltése

            feladatokListaLBx.Items.Clear();
            for (int i = 0; i < feladatokLista.Count; i++)
            {
                feladatokLista[i].Checked += CheckBox_EventHandler;
                feladatokLista[i].Unchecked += CheckBox_EventHandler;
                feladatokListaLBx.Items.Add(feladatokLista[i]);
            }

            // Törölt feladatok ListBox feltöltése
            toroltElemekLBx.Items.Clear();
            for (int i = 0; i < toroltFelLista.Count; i++)
            {
                toroltFelLista[i].Checked += CheckBox_EventHandler;
                toroltFelLista[i].Unchecked += CheckBox_EventHandler;
                toroltElemekLBx.Items.Add(toroltFelLista[i]);
            }
        }

        // A feladatSzovegInput-ba beírt szöveg hozzáadása a
        // feladatokListaLBx ListBox-hoz CheckBox-ként
        private void ujHozzadasaBtn_Click(object sender, RoutedEventArgs e)
        {
            string ujFeladatSzoveg = feladatSzovegInput.Text;

            feladatokListaLBx.Items.Clear();
            feladatokLista.Add(new CheckBox() { Content = ujFeladatSzoveg });

            string adandoSzoveg = File.ReadAllText("feladatok.dat") == "" ? $"{ujFeladatSzoveg}|0" : $"\n{ujFeladatSzoveg}|0";
            File.AppendAllText("feladatok.dat", adandoSzoveg);

            ListBoxokFeltoltese();

            feladatSzovegInput.Clear();
        }

        // Checkboxok szövegének kezelése a checkbox állapotától függően
        private void CheckBox_EventHandler(object sender, RoutedEventArgs e)
        {
            int index = 0;
            foreach (CheckBox cb in feladatokListaLBx.Items)
            {
                if (cb.IsChecked == true)
                {
                    cb.FontStyle = FontStyles.Italic;
                    cb.Foreground = Brushes.Gray;
                    FajlSzerkesztesAdottSorban("feladatok.dat", $"{feladatokLista[index].Content.ToString()}|1", index);
                }
                else
                {
                    cb.FontStyle = FontStyles.Normal;
                    cb.Foreground = Brushes.Black;
                    FajlSzerkesztesAdottSorban("feladatok.dat", $"{feladatokLista[index].Content.ToString()}|0", index);
                }
                index++;
            }
        }

        private void FajlSzerkesztesAdottSorban(string fajlNev, string ujSzoveg, int sor)
        {
            List<string> sorokList = File.ReadAllLines(fajlNev).ToList();
            sorokList.RemoveAll(s => s == "");
            sorokList[sor] = ujSzoveg;
            File.WriteAllLines(fajlNev, sorokList);
        }

        private void feladatTorles_Click(object sender, RoutedEventArgs e)
        {
            //if (feladatokListaLBx.SelectedItem == null)
            //    return;

            string felNev = "";
            int isChecked = 0;

            string kijeloltELem = feladatokListaLBx.SelectedItem.ToString();
            felNev = kijeloltELem.Remove(0, 41);
            felNev = felNev.Remove(felNev.IndexOf(" IsChecked:"));
            isChecked = kijeloltELem.EndsWith("True") ? 1 : 0;

            List<string> sorok = File.ReadAllLines("feladatok.dat").ToList();
            sorok.RemoveAll(s => s.StartsWith(felNev + "|"));
            File.WriteAllLines("feladatok.dat", sorok);

            var toroltFelLista = File.ReadAllLines("toroltFeladatok.dat").ToList();
            toroltFelLista.Add($"{felNev}|{isChecked}");
            toroltFelLista.RemoveAll(s => s == "");
            File.WriteAllLines("toroltFeladatok.dat", toroltFelLista);

            CbListaFeltoltes(this.feladatokLista, "feladatok.dat");             // Feladatok lista elemeinek frissítése
            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");       // Törölt feladatok lista elemeinek frissítése
            ListBoxokFeltoltese();                                              // Feladatok ListBox elemeinek frissítése
        }

        private void feladatokListaLBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (feladatokListaLBx.SelectedItem != null)
            {
                string kijeloltELem = feladatokListaLBx.SelectedItem.ToString();
                kijeloltELem = kijeloltELem.Remove(0, 41);
                kijeloltELem = kijeloltELem.Remove(kijeloltELem.IndexOf(" IsChecked:"));

                feladatSzovegInput.Text = kijeloltELem;
                kijeloltFeladat = kijeloltELem;
            }
        }

        private void feladatModositas_Click(object sender, RoutedEventArgs e)
        {
            List<string> felList = File.ReadAllLines("feladatok.dat").ToList();

            for (int f = 0; f < felList.Count; f++)
            {
                if (felList[f].StartsWith(kijeloltFeladat + "|"))
                {
                    felList[f] = feladatSzovegInput.Text + "|" + felList[f].Substring(kijeloltFeladat.Length + 1);
                }
            }

            File.WriteAllLines("feladatok.dat", felList);

            CbListaFeltoltes(feladatokLista, "feladatok.dat");
            ListBoxokFeltoltese();
        }

        private void toroltElemekLBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (toroltElemekLBx.SelectedItem != null)
            {
                string kijeloltELem = toroltElemekLBx.SelectedItem.ToString();
                kijeloltELem = kijeloltELem.Remove(0, 41);
                kijeloltELem = kijeloltELem.Remove(kijeloltELem.IndexOf(" IsChecked:"));

                kijeloltToroltFel = kijeloltELem;
            }
        }

        private void kijeloltFelVisszaBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (toroltElemekLBx.SelectedItem == null)
            //    return;

            string felNev = "";
            int isChecked = 0;

            string kijeloltELem = toroltElemekLBx.SelectedItem.ToString();
            felNev = kijeloltELem.Remove(0, 41);
            felNev = felNev.Remove(felNev.IndexOf(" IsChecked:"));
            isChecked = kijeloltELem.EndsWith("True") ? 1 : 0;

            List<string> toroltFelLista = File.ReadAllLines("toroltFeladatok.dat").ToList();
            toroltFelLista.RemoveAll(s => s.StartsWith(felNev + "|"));
            File.WriteAllLines("toroltFeladatok.dat", toroltFelLista);

            var sorok = File.ReadAllLines("feladatok.dat").ToList();
            sorok.Add($"{felNev}|{isChecked}");
            sorok.RemoveAll(s => s == "");
            File.WriteAllLines("feladatok.dat", sorok);

            CbListaFeltoltes(this.feladatokLista, "feladatok.dat");             // Feladatok lista elemeinek frissítése
            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");       // Törölt feladatok lista elemeinek frissítése
            ListBoxokFeltoltese();
        }

        private void kijeloltFelVeglegTorleseBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> toroltFelLista = File.ReadAllLines("toroltFeladatok.dat").ToList();
            toroltFelLista.RemoveAll(f => f.StartsWith(kijeloltToroltFel + "|"));
            File.WriteAllLines("toroltFeladatok.dat", toroltFelLista);

            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat"); 
        }
    }
}

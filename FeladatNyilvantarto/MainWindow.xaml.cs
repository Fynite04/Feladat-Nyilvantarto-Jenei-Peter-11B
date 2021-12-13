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
            CheckboxKezelo();
        }

        // Adatfájlok létrehozása amennyiben nem léteznek
        // Ha igen, akkor azok alapján a ListBoxok feltöltése
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

        // Checkbox lista feltöltése / frissítése az adatfájl használatával
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

        // ListBoxok (feladatokListaLBx, toroltElemekLBx) feltöltése / frissítése,
        // a bennük lévő checkboxokhoz események hozzárendelése
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

        // A feladatSzovegInput-ba beírt szöveg hozzáadása a feladatok
        // ListBox-hoz Checkbox-ként, adatfájl szerkesztése ez alapján
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

        // Checkboxok állapotának (ki van e pipálva) változásakor jön működésbe
        private void CheckBox_EventHandler(object sender, RoutedEventArgs e)
        {
            CheckboxKezelo();
        }

        // Checkboxok szövegének kezelése az állapotuktól függően, az adatfájlok
        // szerkesztése ezek alapján
        private void CheckboxKezelo()
        {
            // Feladatok ListBox
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

            // Törölt feladatok ListBox
            index = 0;
            foreach (CheckBox cb in toroltElemekLBx.Items)
            {
                if (cb.IsChecked == true)
                {
                    cb.FontStyle = FontStyles.Italic;
                    cb.Foreground = Brushes.Gray;
                    FajlSzerkesztesAdottSorban("toroltFeladatok.dat", $"{toroltFelLista[index].Content.ToString()}|1", index);
                }
                else
                {
                    cb.FontStyle = FontStyles.Normal;
                    cb.Foreground = Brushes.Black;
                    FajlSzerkesztesAdottSorban("toroltFeladatok.dat", $"{toroltFelLista[index].Content.ToString()}|0", index);
                }
                index++;
            }
        }

        // Szövegfájlok szerkesztése egy megadott sorban; az adatfájlok szerkesztése
        // a checkboxok állapota alapján (be: 1, ki: 0)
        private void FajlSzerkesztesAdottSorban(string fajlNev, string ujSzoveg, int sor)
        {
            List<string> sorokList = File.ReadAllLines(fajlNev).ToList();
            sorokList.RemoveAll(s => s == "");              // A File.WriteAllLines által létrehozott üres sorok törlése
            sorokList[sor] = ujSzoveg;
            File.WriteAllLines(fajlNev, sorokList);
        }

        // A kijelölt feladat törlése (áthelyezés a feladat ListBoxból a törölt
        // feladatok ListBoxba) a gomb megnyomásakor
        private void feladatTorles_Click(object sender, RoutedEventArgs e)
        {
            if (feladatokListaLBx.SelectedItem == null)
                return;

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

            CbListaFeltoltes(this.feladatokLista, "feladatok.dat");
            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");
            ListBoxokFeltoltese();
            CheckboxKezelo();
        }

        // A feladatok ListBoxban kijelölt elemek figyelése
        private void feladatokListaLBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (feladatokListaLBx.SelectedItem != null)
            {
                string kijeloltELem = feladatokListaLBx.SelectedItem.ToString();
                kijeloltELem = kijeloltELem.Remove(0, 41);
                kijeloltELem = kijeloltELem.Remove(kijeloltELem.IndexOf(" IsChecked:"));

                feladatSzovegInput.Text = kijeloltELem;         // A kijelölt elemek megjelenítése a beviteli mezőben
                kijeloltFeladat = kijeloltELem;
            }
        }

        // A kijelölt feladat tartalmának módosítása a beviteli mezőn beírt szöveg alapján
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
            CheckboxKezelo();
        }

        // A törölt feladatok ListBoxban kijelölt elemek figyelése
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

        // A kijelölt törölt feladat visszaállítása a feladatok közé a gomb megnyomásakor
        private void kijeloltFelVisszaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (toroltElemekLBx.SelectedItem == null)
                return;

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

            CbListaFeltoltes(this.feladatokLista, "feladatok.dat");
            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");
            ListBoxokFeltoltese();
            CheckboxKezelo();
        }

        // A kijelölt törölt feladatok végleges törlése a gomb megnyomásakor
        private void kijeloltFelVeglegTorleseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (toroltElemekLBx.SelectedItem == null)
                return;

            List<string> toroltFelLista = File.ReadAllLines("toroltFeladatok.dat").ToList();
            toroltFelLista.RemoveAll(f => f.StartsWith(kijeloltToroltFel + "|"));
            File.WriteAllLines("toroltFeladatok.dat", toroltFelLista);

            CbListaFeltoltes(this.toroltFelLista, "toroltFeladatok.dat");
            ListBoxokFeltoltese();
            CheckboxKezelo();
        }

        
    }
}

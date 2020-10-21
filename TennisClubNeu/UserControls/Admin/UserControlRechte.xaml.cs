using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TennisClubNeu.Classes;

namespace TennisClubNeu.UserControls.Admin
{
    /// <summary>
    /// Interaktionslogik für UserControlSpieler.xaml
    /// </summary>
    public partial class UserControlRechte : UserControl
    {
        LogRechte Logger;
        public UserControlRechte(LogRechte logrechte)
        {
            InitializeComponent();
            Logger = logrechte;
            ZeichneGrid();
        }

        private void ZeichneGrid() {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                List<Spieler> spieler = (from Spieler s in db.Spieler orderby s.Nachname, s.Vorname select s).ToList();
                dataGrid.ItemsSource = spieler;
            }
        }
        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;
            if (e.Column.Header.ToString() == "IstGebucht")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "ChipId")
                e.Column.Visibility = System.Windows.Visibility.Hidden; 
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            Spieler s = (Spieler)grid.SelectedItem;
            chkBuchungen.IsChecked = s.IstAdminBuchungen;
            chkFesteBuchungen.IsChecked = s.IstAdminFesteBuchungen;
            chkTurnierspiele.IsChecked = s.IstAdminTurniere;
            chkPlatzsperre.IsChecked = s.IstAdminPlatzsperre;
            chkSpieler.IsChecked = s.IstAdminSpieler;
            chkRechte.IsChecked = s.IstAdminRechte;
            LblVollerName.Content = s.Nachname + ", " + s.Vorname;
            tbId.Text = s.Id.ToString();
            BtnSpeichern.Visibility = Visibility.Visible;
        }

        private void Clear() {
            chkBuchungen.IsChecked = false;
            chkFesteBuchungen.IsChecked = false;
            chkTurnierspiele.IsChecked = false;
            chkPlatzsperre.IsChecked = false;
            chkSpieler.IsChecked = false;
            chkRechte.IsChecked = false;
            LblVollerName.Content = "";
            tbId.Text = "0";
            ZeichneGrid();
            BtnSpeichern.Visibility = Visibility.Hidden;
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                int id = Int32.Parse(tbId.Text);
                Spieler spieler = (from Spieler s in db.Spieler where s.Id == id select s).FirstOrDefault();

                spieler.IstAdminBuchungen = (bool)chkBuchungen.IsChecked;
                spieler.IstAdminFesteBuchungen = (bool)chkFesteBuchungen.IsChecked;
                spieler.IstAdminSpieler = (bool)chkSpieler.IsChecked;
                spieler.IstAdminTurniere = (bool)chkTurnierspiele.IsChecked;
                spieler.IstAdminRechte = (bool)chkRechte.IsChecked;
                spieler.IstAdminPlatzsperre = (bool)chkPlatzsperre.IsChecked;

                db.SaveChanges();
                Logger.Logger.Info(Logger.Rechte.Name + " aendert Rechte von " + spieler.Vorname + " " + spieler.Nachname);
                Clear();
            }
        }
    }
}

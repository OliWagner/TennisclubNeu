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
    public partial class UserControlSpieler : UserControl
    {
        LogRechte Logger;
        public UserControlSpieler(LogRechte logrechte)
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

            if (e.Column.Header.ToString() == "IstAdminTurniere")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "IstAdminPlatzsperre")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "IstAdminBuchungen")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "IstAdminFesteBuchungen")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "IstAdminSpieler")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "IstAdminRechte")
                e.Column.Visibility = System.Windows.Visibility.Hidden;



        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            Spieler s = (Spieler)grid.SelectedItem;
            tbId.Text = s.Id.ToString();
            tbNachname.Text = s.Nachname;
            tbVorname.Text = s.Vorname;
            tbChipId.Text = s.ChipId;
            chkAktiv.IsChecked = (bool)s.IstAktiv;
        }

        private void Clear() {
            tbChipId.Text = "";
            tbNachname.Text = "";
            tbId.Text = "0";
            tbVorname.Text = "";
            chkAktiv.IsChecked = false;
            ZeichneGrid();
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                string neuspielername = "";

                if (tbId.Text.Equals("0"))
                {
                    Spieler s = new Spieler();
                    s.Nachname = tbNachname.Text;
                    s.Vorname = tbVorname.Text;
                    s.ChipId = tbChipId.Text;
                    s.IstAdminBuchungen = false;
                    s.IstAdminFesteBuchungen = false;
                    s.IstAdminPlatzsperre = false;
                    s.IstAdminRechte = false;
                    s.IstAdminTurniere = false;
                    s.IstAdminSpieler = false;
                    s.IstAktiv = true;
                    db.Spieler.Add(s);
                    neuspielername = " fuegt Spieler " + s.Vorname + " " + s.Nachname + " hinzu.";
                }
                else
                {
                    int i = Int32.Parse(tbId.Text);
                    Spieler s = (from Spieler sp in db.Spieler where sp.Id == i select sp).FirstOrDefault();
                    s.Nachname = tbNachname.Text;
                    s.Vorname = tbVorname.Text;
                    s.ChipId = tbChipId.Text;
                    s.IstAktiv = (bool)chkAktiv.IsChecked;
                    neuspielername = " aendert Spieler " + s.Vorname + " " + s.Nachname + ".";
                }
                db.SaveChanges();
                Logger.Logger.Info(Logger.Rechte.Name + neuspielername);
                Clear();
            }
        }

        private void BtnSpeichern_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                int Id = Int32.Parse(tbId.Text);

                List<string> lstChipIds = (from Spieler spieler in db.Spieler where spieler.Id != Id select spieler.ChipId).ToList();

                if (tbNachname.Text.Length < 2 || tbVorname.Text.Length < 2 || tbChipId.Text.Length == 0 || !CheckChipId(lstChipIds))
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = true;
            }
            
        }

        private bool CheckChipId(List<string> lstChipIds) {


                if (lstChipIds.Contains(tbChipId.Text)) {
                    return false;
                }
            
            return true;
        }

        private void BtnSpeichern_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}

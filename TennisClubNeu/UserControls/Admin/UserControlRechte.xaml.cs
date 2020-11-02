using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls.Admin
{
    /// <summary>
    /// Interaktionslogik für UserControlSpieler.xaml
    /// </summary>
    public partial class UserControlRechte : UserControl
    {
        public UserControlRechte()
        {
            InitializeComponent();
            ZeichneGrid();
        }

        private void ZeichneGrid() {

            dataGrid.ItemsSource = RechteRepository.GetInstance().GetSpieler();
            
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
            RechteRepository.GetInstance().Save(Int32.Parse(tbId.Text), (bool)chkBuchungen.IsChecked, (bool)chkFesteBuchungen.IsChecked, (bool)chkSpieler.IsChecked, (bool)chkTurnierspiele.IsChecked, (bool)chkRechte.IsChecked, (bool)chkPlatzsperre.IsChecked);
            Clear();
        }
    }
}

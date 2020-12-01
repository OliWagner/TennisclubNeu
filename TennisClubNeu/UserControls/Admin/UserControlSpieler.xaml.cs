using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls.Admin
{
    /// <summary>
    /// Interaktionslogik für UserControlSpieler.xaml
    /// </summary>
    public partial class UserControlSpieler : UserControl
    {
        public UserControlSpieler()
        {
            InitializeComponent();
            ZeichneGrid();
        }

        private void ZeichneGrid() {

            dataGrid.ItemsSource = SpielerRepository.GetInstance().GetSpieler();
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
            SpielerRepository.GetInstance().Save(tbId.Text, tbNachname.Text, tbVorname.Text, tbChipId.Text, (bool)chkAktiv.IsChecked);            
            Clear();
        }

        private void BtnSpeichern_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
                List<string> lstChipIds = SpielerRepository.GetInstance().GetListChipIds(Int32.Parse(tbId.Text));

                if (tbNachname.Text.Length < 2 || tbVorname.Text.Length < 2 || tbChipId.Text.Length == 0 || !CheckChipId(lstChipIds))
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = true;
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

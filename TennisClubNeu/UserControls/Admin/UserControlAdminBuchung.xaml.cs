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
    /// Interaktionslogik für UserControlAdminBuchung.xaml
    /// </summary>
    public partial class UserControlAdminBuchung : UserControl
    {
        public UserControlAdminBuchung()
        {
            InitializeComponent();
            ZeichneGrid();
        }

        private void ZeichneGrid()
        {
                dataGrid.ItemsSource = AdminBuchungRepository.GetInstance().GetBuchungen(); ;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            Buchungen s = (Buchungen)grid.SelectedItem;
            string buchung = "Platz " + s.PlatzId + " " + s.Titel + ", " + s.Startzeit.ToShortDateString() + " " + s.Startzeit.ToShortTimeString() + " - " + s.Endzeit.ToShortDateString() + " " + s.Endzeit.ToShortTimeString();
            tbId.Text = s.Id.ToString();
            lblBuchung.Content = buchung;
            btnLoeschen.Visibility = Visibility.Visible;
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Zeile1")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Zeile2")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Zeile3")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Zeile4")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Zeile5")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Spieler1Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Spieler2Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Spieler3Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Spieler4Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "Spieler5Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "FesteBuchungGuid")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.Column.Header.ToString() == "TurnierspielGuid")
                e.Column.Visibility = System.Windows.Visibility.Hidden;

            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy HH:mm";
        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            Buchungen b = (Buchungen)dataGrid.SelectedItem;
            int id = b.Id;
            AdminBuchungRepository.GetInstance().RemoveBuchung(id);
            ZeichneGrid();

        }
    }
}

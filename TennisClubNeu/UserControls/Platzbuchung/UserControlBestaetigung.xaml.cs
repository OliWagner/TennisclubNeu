using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls.Platzbuchung
{
    /// <summary>
    /// Interaktionslogik für UserControlBestaetigung.xaml
    /// </summary>
    public partial class UserControlBestaetigung : UserControl
    {
        private Buchungen buchung;

        public Buchungen Buchung { get => buchung; set => buchung = value; }

        public UserControlBestaetigung()
        {
            InitializeComponent();
        }

        public void ZeichneGrid(Buchungen buchung) {
            Buchung = buchung;
            lblPlatzUhrzeit.Content = "Platz " + buchung.PlatzId + " um " + buchung.Startzeit.ToShortTimeString();
            lblSpieler1.Content = Helpers.GetSpielerNameById(buchung.Spieler1Id);
            lblSpieler2.Content = Helpers.GetSpielerNameById(buchung.Spieler2Id);
            lblSpieler3.Content = Helpers.GetSpielerNameById(buchung.Spieler3Id);
            lblSpieler4.Content = Helpers.GetSpielerNameById(buchung.Spieler4Id);

        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
                Buchungen _buchung = BuchungenRepository.GetInstance().GetBuchungById(Buchung.Id);

                if (_buchung != null) {
                    Helpers.SetzeSpielerIstGebucht((int)_buchung.Spieler1Id, false);
                    Helpers.SetzeSpielerIstGebucht((int)_buchung.Spieler2Id, false);
                    if (_buchung.Spieler3Id != 0) {
                        Helpers.SetzeSpielerIstGebucht((int)_buchung.Spieler3Id, false);
                    }
                    if (_buchung.Spieler4Id != 0)
                    {
                        Helpers.SetzeSpielerIstGebucht((int)_buchung.Spieler4Id, false);
                    }
                    BuchungenRepository.GetInstance().RemoveBuchung(_buchung.Id);
                }
        }

        private void BtnBestaetigen_Click(object sender, RoutedEventArgs e)
        {
            //Hier passiert garnix
        }
    }
}

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TennisClubNeu.Classes;

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
            lblSpieler1.Content = Helpers.GetSpielerNameById(new TennisclubNeuEntities(), buchung.Spieler1Id);
            lblSpieler2.Content = Helpers.GetSpielerNameById(new TennisclubNeuEntities(), buchung.Spieler2Id);
            lblSpieler3.Content = Helpers.GetSpielerNameById(new TennisclubNeuEntities(), buchung.Spieler3Id);
            lblSpieler4.Content = Helpers.GetSpielerNameById(new TennisclubNeuEntities(), buchung.Spieler4Id);

        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                Buchungen _buchung = (from Buchungen bu in db.Buchungen where bu.Id == Buchung.Id select bu).FirstOrDefault();
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


                    db.Buchungen.Remove(_buchung);
                    db.SaveChanges();
                }
            }
        }

        private void BtnBestaetigen_Click(object sender, RoutedEventArgs e)
        {
            //Hier passiert garnix
        }
    }
}

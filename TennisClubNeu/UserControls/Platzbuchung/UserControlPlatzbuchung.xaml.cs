using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TennisclubNeu;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls
{
    /// <summary>
    /// Interaktionslogik für UserControlPlatzbuchung.xaml
    /// </summary>
    public partial class UserControlPlatzbuchung : UserControl
    {
        public Spieler Spieler1 { get; set; } = new Spieler();
        public Spieler Spieler2 { get; set; } = new Spieler();        
        public Spieler Spieler3 { get; set; } = new Spieler();
        public Spieler Spieler4 { get; set; } = new Spieler();
        public int PlatzId { get => platzId; set => platzId = value; }
        public DateTime Startzeit { get => startzeit; set => startzeit = value; }
        public DateTime Endezeit { get => endezeit; set => endezeit = value; }

        private int platzId;
        private DateTime startzeit;
        private DateTime endezeit;
        private AnzeigePlatz _ap;
        private bool _istRestBuchung = false;

        public UserControlPlatzbuchung()
        {
            InitializeComponent();
            GetEventsTastatur();
        }

        public void InitializeData(AnzeigePlatz ap, int spielerId)
        {

            Spieler sp = SpielerRepository.GetInstance().GetSpielerById(spielerId);
            SetzeSpieler(sp.Id);
            
            _ap = ap;
            string strUhrzeit = _ap.Uhrzeit.Content.ToString();
            if (strUhrzeit.Equals("frei"))
            {
                PlatzId = ap.PlatzId;
                Startzeit = ErmittleStartzeit("");
                Endezeit = Startzeit.AddHours(1);
                tbAnzeige.Text = "";
                tbUhrzeit.Text = "Platz " + ap.PlatzId + " " + Startzeit.ToShortTimeString() + " - " + Endezeit.ToShortTimeString();
            }


            else if (strUhrzeit.Contains("-"))
            {
                string strM = strUhrzeit.Split(':')[1];
                int minuten = Int32.Parse(strM);
                _istRestBuchung = true;
                PlatzId = ap.PlatzId;
                Startzeit = DateTime.Now;
                Endezeit = Startzeit.AddMinutes(minuten);
                tbAnzeige.Text = "";
                tbUhrzeit.Text = "Platz " + ap.PlatzId + " " + Startzeit.ToShortTimeString() + " - " + Endezeit.ToShortTimeString();
            }
            else {
                PlatzId = ap.PlatzId;
                Startzeit = ErmittleStartzeit(strUhrzeit);
                Endezeit = Startzeit.AddHours(1);
                tbAnzeige.Text = "";
                tbUhrzeit.Text = "Platz " + ap.PlatzId + " " + Startzeit.ToShortTimeString() + " - " + Endezeit.ToShortTimeString();
            }
        }

        private DateTime ErmittleStartzeit(string startZeit) {
            if (startZeit.Equals(""))
            {
                DateTime zeit = DateTime.Now;
                int minute = zeit.Minute;
                int stunde = zeit.Hour;
                int year = zeit.Year;
                int month = zeit.Month;
                int day = zeit.Day;
                int minuteneu = 0;

                if (minute >= 0 && minute < 15)
                {
                    minuteneu = 30;
                }
                if (minute >= 15 && minute < 30)
                {
                    //Muss auf 45 enden
                    minuteneu = 45;
                }
                if (minute >= 30 && minute < 45)
                {
                    //Muss auf 00 + 1 Stunde enden enden
                    stunde++;
                    minuteneu = 00;
                }
                if (minute >= 45 && minute <= 59)
                {
                    //Muss auf 15 + 1 Stunde enden enden
                    stunde++;
                    minuteneu = 15;
                }
                return new DateTime(year, month, day, stunde, minuteneu, 0);
            }
            else {
                DateTime zeit = DateTime.Now;
                string[] zeiten = startZeit.Split(':');
                int minute = Int32.Parse(zeiten[1]);
                int stunde = Int32.Parse(zeiten[0]);
                int year = zeit.Year;
                int month = zeit.Month;
                int day = zeit.Day;
                return new DateTime(year, month, day, stunde, minute, 0);
            }
            
        }


        /// <summary>
        /// Methode für Tastatur erforderlich, Hängt delegate UcTastaturBtnClick an die Button.Click-Ereignisse der Tastatur
        /// </summary>
        private void GetEventsTastatur()
        {
            foreach (var obj in UcTastatur.grdMain.Children)
            {
                if (obj.GetType() == typeof(Button)) {
                    Button b = (Button)obj;
                    b.Click += UcTastaturBtnClick;
                }
            }
        }

        /// <summary>
        /// Verarbeitet die Eingaben der Tastatur
        /// </summary>
        /// <param name="sender">Der Wert des Buttons ist im Tag-Objekt gespeichert</param>
        /// <param name="e"></param>
        private void UcTastaturBtnClick(object sender, RoutedEventArgs e)
        {
            Button _sender = (Button)sender;
            if (_sender.Tag.ToString().Equals("BACK") || _sender.Tag.ToString().Equals("SPACE") || _sender.Tag.ToString().Equals("DEL"))
            {
                if(_sender.Tag.ToString().Equals("BACK") && tbAnzeige.Text.Length > 0) {
                    tbAnzeige.Text = tbAnzeige.Text.Substring(0, tbAnzeige.Text.Length - 1);
                }
                if (_sender.Tag.ToString().Equals("SPACE")) {
                    tbAnzeige.Text = tbAnzeige.Text += " ";
                }
                if (_sender.Tag.ToString().Equals("DEL"))
                {
                    tbAnzeige.Text = "";
                }
            }
            else {
                tbAnzeige.Text = tbAnzeige.Text += _sender.Tag.ToString();
            }
            
            ZeichneAuswahl();
        }

        private void ZeichneAuswahl() {
            
            dgSpielerwahl.ItemsSource = SpielerRepository.GetInstance().GetFuerBuchungVerfuegbareSpieler(tbAnzeige.Text, Spieler1.Id, Spieler2.Id, Spieler3.Id, Spieler4.Id);

            dgSpielerwahl.Columns.RemoveAt(0);
            for (int i = dgSpielerwahl.Columns.Count - 1; i > 1; i--)
            {
                dgSpielerwahl.Columns.RemoveAt(i);
            }
            dgSpielerwahl.HeadersVisibility = DataGridHeadersVisibility.None;  
        }

        int _selectedSpielerId;

        private void DgSpielerwahl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            Spieler s = (Spieler)dg.SelectedItem;
            if (s != null)
            {
                tbAnzeige.Text = s.Vorname + " " +s.Nachname;
                _selectedSpielerId = s.Id;
                btnBestaetigen.Visibility = Visibility.Visible;
            }
            else
            {
                btnBestaetigen.Visibility = Visibility.Hidden;
            }
        }


        public void SetzeSpieler(int spielerId) {
                Spieler sp = SpielerRepository.GetInstance().GetSpielerById(spielerId);

                if (Spieler1.Nachname == null) { Spieler1 = sp; tbAnzeige.Text = ""; tbSpieler1.Text = Helpers.GetSpielerNameById(sp.Id);
                    Helpers.SetzeSpielerInBearbeitung(Spieler1.Id, false);
                    return; }               
                if (Spieler2.Nachname == null) { Spieler2 = sp; tbAnzeige.Text = ""; tbSpieler2.Text = Helpers.GetSpielerNameById(sp.Id);
                    btnBuchungSpeichern.Visibility = Visibility.Visible;
                    Helpers.SetzeSpielerInBearbeitung(Spieler2.Id, false);
                    return; }
                if (Spieler3.Nachname == null) { Spieler3 = sp; tbAnzeige.Text = ""; tbSpieler3.Text = Helpers.GetSpielerNameById(sp.Id);
                    Helpers.SetzeSpielerInBearbeitung(Spieler3.Id, false);
                    return; }
                if (Spieler4.Nachname == null) { Spieler4 = sp; tbAnzeige.Text = ""; tbSpieler4.Text = Helpers.GetSpielerNameById(sp.Id);
                    if (!_istRestBuchung) {
                        //REstbuchung ist die verbleibende Zeit bis zu einem vorher feststehenden Termin bis zu 59 Minuten
                        Endezeit = Endezeit.AddHours(0.5);
                    }
                    Helpers.SetzeSpielerInBearbeitung(Spieler4.Id, false);
                    tbUhrzeit.Text = "Platz " + PlatzId + " " + Startzeit.ToShortTimeString() + " - " + Endezeit.ToShortTimeString();
                    return;
            }
        }

        private void BtnBestaetigen_Click(object sender, RoutedEventArgs e)
        {
            SetzeSpieler(_selectedSpielerId);
            _selectedSpielerId = 0;
            btnBestaetigen.Visibility = Visibility.Hidden;
            tbAnzeige.Text = "";
            dgSpielerwahl.ItemsSource = null;
        }

        private void BtnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            SetzeSpielerInBearbeitungFrei();
        }

        private void SetzeSpielerInBearbeitungFrei() {
            if (Spieler1.Id != 0)
            {
                Helpers.SetzeSpielerInBearbeitung(Spieler1.Id, true);
            }

            if (Spieler2.Id != 0)
            {
                Helpers.SetzeSpielerInBearbeitung(Spieler2.Id, true);
            }

            if (Spieler3.Id != 0)
            {
                Helpers.SetzeSpielerInBearbeitung(Spieler3.Id, true);
            }

            if (Spieler4.Id != 0)
            {
                Helpers.SetzeSpielerInBearbeitung(Spieler4.Id, true);
            }


        }

        private void BtnBuchungSpeichern_Click(object sender, RoutedEventArgs e)
        {
                Helpers.SetzeSpielerIstGebucht(Spieler1.Id, true);
                Helpers.SetzeSpielerIstGebucht(Spieler2.Id, true);

                if (Spieler3.Id != 0) {
                    Helpers.SetzeSpielerIstGebucht(Spieler3.Id, true);
                }

                if (Spieler4.Id != 0)
                {
                    Helpers.SetzeSpielerIstGebucht(Spieler4.Id, true);
                }

                Buchungen buchung = new Buchungen();
                buchung.PlatzId = PlatzId;
                buchung.Startzeit = Startzeit;
                buchung.Endzeit = Endezeit;
                buchung.Titel = "Platzbuchung";
                buchung.Spieler1Id = Spieler1.Id;
                buchung.Spieler2Id = Spieler2.Id;
                buchung.Spieler3Id = Spieler3.Id;
                buchung.Spieler4Id = Spieler4.Id;
            BuchungenRepository.GetInstance().BuchungEintragen(buchung);
            SetzeSpielerInBearbeitungFrei();
        }
    }
}

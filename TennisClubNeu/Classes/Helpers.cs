using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TennisclubNeu;

namespace TennisClubNeu.Classes
{
    public static class Helpers
    {
        public static Grid GetMainGrid(GridInfo info) {
            Grid Returngrid = new Grid();
                
                //GridColums
                for (int i = 1; i <= info.Columns; i++)
                {
                    //Abwechselnd Space und Content
                    if (i % 2 == 0)
                    {
                        //Content
                        ColumnDefinition cd = new ColumnDefinition();
                        GridLength len = new GridLength(info.ContentColumnWidth);
                        cd.Width = len;
                        Returngrid.ColumnDefinitions.Add(cd);
                    }
                    else
                    {
                        //Space
                        ColumnDefinition cd = new ColumnDefinition();
                        GridLength len = new GridLength(info.SpaceColumnWidth);
                        cd.Width = len;
                        Returngrid.ColumnDefinitions.Add(cd);
                    }
                }
                //GridRows
                for (int i = 1; i <= info.Rows; i++)
                {
                    //Abwechselnd Space und Content
                    if (i % 2 == 0)
                    {
                        //Content
                        RowDefinition rd = new RowDefinition();
                        GridLength len = new GridLength(info.ContentRowHeight);
                        rd.Height = len;
                        Returngrid.RowDefinitions.Add(rd);
                    }
                    else
                    {
                        //Space
                        RowDefinition rd = new RowDefinition();
                        GridLength len = new GridLength(info.SpaceRowHeight);
                        rd.Height = len;
                        Returngrid.RowDefinitions.Add(rd);
                    }
                }
                return Returngrid;
            }
            
        public static AnzeigePlatz GetAnzeigePlatz(AnzeigePlatzViewModel model) {
            return new AnzeigePlatz(model);
        }

        public static Rechte GetRechteFuerAnmeldung(string anmeldeId) {
            Rechte rechte = new Rechte();
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {

                Spieler spieler = (from Spieler sp in db.Spieler where sp.ChipId.Equals(anmeldeId) select sp).FirstOrDefault();
                if (spieler == null)
                {
                    return null;
                }
                rechte.Name = spieler.Vorname + " " + spieler.Nachname;
                rechte.Id = spieler.Id;
                rechte.IsAdminBuchungen = spieler.IstAdminBuchungen;
                rechte.IsAdminFesteBuchungen = spieler.IstAdminFesteBuchungen;
                rechte.IsAdminTurnierspiele = spieler.IstAdminTurniere;
                rechte.IsAdminPlatzsperre = spieler.IstAdminPlatzsperre;
                rechte.IsAdminRechte = spieler.IstAdminRechte;
                rechte.IsAdminSpieler = spieler.IstAdminSpieler;
                return rechte;
            }
        }

        public static string GetSpielerNameById(TennisclubNeuEntities db, int? id)
        {
            Spieler spieler = (from Spieler s in db.Spieler where s.Id == id select s).FirstOrDefault();

            if (spieler == null)
            {
                return "";
            }
            return spieler.Nachname + ", " + spieler.Vorname;
        }
        /// <summary>
        /// Ist Bool true wird der Spieler entfernt, bei False erfasst
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="_bool"></param>
        public static void SetzeSpielerInBearbeitung(int Id, bool _bool) {
            if (_bool)
            {//Der Spieler ist bereits angemeldet und muss entfernt werden
                using (TennisclubNeuEntities db = new TennisclubNeuEntities())
                {
                    SpielerInBearbeitung sib = (from SpielerInBearbeitung s in db.SpielerInBearbeitung where s.SpielerId == Id select s).FirstOrDefault();
                    if (sib!=null) { 
                    db.SpielerInBearbeitung.Remove(sib);
                    db.SaveChanges();
                    }
                }
            }
            else
            {//Der Spieler wird angemeldet
                using (TennisclubNeuEntities db = new TennisclubNeuEntities())
                {
                    SpielerInBearbeitung s = new SpielerInBearbeitung();
                    s.SpielerId = Id;
                    db.SpielerInBearbeitung.Add(s);
                    db.SaveChanges();
                }
            }

            
        }

        public static void ClearSpielerInBearbeitung()
        {
           
                using (TennisclubNeuEntities db = new TennisclubNeuEntities())
                {
                    List<SpielerInBearbeitung> sib = (from SpielerInBearbeitung s in db.SpielerInBearbeitung select s).ToList();
                    if (sib != null)
                    {
                        db.SpielerInBearbeitung.RemoveRange(sib);
                        db.SaveChanges();
                    }
                }
        }

        public static void ClearSpielerIstGebucht() {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                List<int> _alleSpielerIdsGebucht = new List<int>();
                List<Spieler> _alleSpieler = (from Spieler spieler in db.Spieler select spieler).ToList();
                List <Buchungen> _buchungen = (from Buchungen bu in db.Buchungen where (bu.Endzeit > DateTime.Now) && (bu.Spieler1Id != null) select bu).ToList();
                foreach (Buchungen buchung in _buchungen)
                {
                    if (buchung.Spieler1Id != null)
                    {
                            _alleSpielerIdsGebucht.Add((int)buchung.Spieler1Id);
                    }

                    if (buchung.Spieler2Id != null)
                    {
                            _alleSpielerIdsGebucht.Add((int)buchung.Spieler2Id);
                    }

                    if (buchung.Spieler3Id != null)
                    {
                            _alleSpielerIdsGebucht.Add((int)buchung.Spieler3Id);
                    }

                    if (buchung.Spieler4Id != null)
                    {
                            _alleSpielerIdsGebucht.Add((int)buchung.Spieler4Id);
                    }
                }
                foreach (Spieler spieler in _alleSpieler)
                {
                    if (_alleSpielerIdsGebucht.Contains(spieler.Id)) {
                        spieler.IstGebucht = true;
                    } else {
                        spieler.IstGebucht = false;
                    }
                }
                db.SaveChanges();
            }
        }

        public static Buchungen CheckSpielerGebucht(int spielerId) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where (bu.Endzeit > DateTime.Now) && (bu.Spieler1Id == spielerId || bu.Spieler2Id == spielerId || bu.Spieler3Id == spielerId || bu.Spieler4Id == spielerId) select bu).FirstOrDefault();
                if (buchung != null)
                {
                    //Der Spieler ist bereits gebucht. Die Buchung wird dann in der Anzeige zur Bestätigung verwendet
                        return buchung;
                }
            }
            return null;
        }

        /// <summary>
        /// Bei True wir der Spieler auf IstGebucht gesetzt
        /// </summary>
        /// <param name="spielerId"></param>
        /// <param name="_bool"></param>
        public static void SetzeSpielerIstGebucht(int spielerId, bool _bool) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                Spieler spieler = (from Spieler s in db.Spieler where s.Id == spielerId select s).FirstOrDefault();
                spieler.IstGebucht = _bool;
                db.SaveChanges();
            }
        }
    }
}

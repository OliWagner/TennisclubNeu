using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class SpielerRepository
    {
        static SpielerRepository rep = null;

        private SpielerRepository() {
        }

        public static SpielerRepository GetInstance() {
            if (rep == null) {
                rep = new SpielerRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public List<Spieler> GetSpieler()
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<Spieler> spieler = (from Spieler s in db.Spieler orderby s.Nachname, s.Vorname select s).ToList();
                return spieler;
            }
        }

        public Spieler GetSpielerById(int Id)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Spieler spieler = (from Spieler s in db.Spieler where s.Id == Id select s).FirstOrDefault();
                return spieler;
            }
        }

        public void Save(string tbIdText, string tbNachname, string tbVorname, string tbChipId, bool chkAktivIsChecked) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {

                if (tbIdText.Equals("0"))
                {
                    Spieler s = new Spieler();
                    s.Nachname = tbNachname;
                    s.Vorname = tbVorname;
                    s.ChipId = tbChipId;
                    s.IstAdminBuchungen = false;
                    s.IstAdminFesteBuchungen = false;
                    s.IstAdminPlatzsperre = false;
                    s.IstAdminRechte = false;
                    s.IstAdminTurniere = false;
                    s.IstAdminSpieler = false;
                    s.IstAktiv = true;
                    db.Spieler.Add(s);
                }
                else
                {
                    int i = Int32.Parse(tbIdText);
                    Spieler s = (from Spieler sp in db.Spieler where sp.Id == i select sp).FirstOrDefault();
                    s.Nachname = tbNachname;
                    s.Vorname = tbVorname;
                    s.ChipId = tbChipId;
                    s.IstAktiv = chkAktivIsChecked;
                }
                db.SaveChanges();
            }
        }

        public List<string> GetListChipIds(int id) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                return (from Spieler spieler in db.Spieler where spieler.Id != id select spieler.ChipId).ToList();
            }
        }

        public void SetzeSpielerInBearbeitung(int Id, bool _bool) {
            if (_bool)
            {//Der Spieler ist bereits angemeldet und muss entfernt werden
                using (TennisclubNeuEntities db = new TennisclubNeuEntities())
                {
                    SpielerInBearbeitung sib = (from SpielerInBearbeitung s in db.SpielerInBearbeitung where s.SpielerId == Id select s).FirstOrDefault();
                    if (sib != null)
                    {
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

        public void ClearSpielerInBearbeitung()
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

        public void ClearSpielerIstGebucht()
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<int> _alleSpielerIdsGebucht = new List<int>();
                List<Spieler> _alleSpieler = (from Spieler spieler in db.Spieler select spieler).ToList();
                List<Buchungen> _buchungen = (from Buchungen bu in db.Buchungen where (bu.Endzeit > DateTime.Now) && (bu.Spieler1Id != null) select bu).ToList();
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
                    if (_alleSpielerIdsGebucht.Contains(spieler.Id))
                    {
                        spieler.IstGebucht = true;
                    }
                    else
                    {
                        spieler.IstGebucht = false;
                    }
                }
                db.SaveChanges();
            }
        }

        public Buchungen CheckSpielerGebucht(int spielerId)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where (bu.Endzeit > DateTime.Now) && (bu.Spieler1Id == spielerId || bu.Spieler2Id == spielerId || bu.Spieler3Id == spielerId || bu.Spieler4Id == spielerId) select bu).FirstOrDefault();
                if (buchung != null)
                {
                    //Der Spieler ist bereits gebucht. Die Buchung wird dann in der Anzeige zur Bestätigung verwendet
                    return buchung;
                }
            }
            return null;
        }

        public void SetzeSpielerIstGebucht(int spielerId, bool _bool)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Spieler spieler = (from Spieler s in db.Spieler where s.Id == spielerId select s).FirstOrDefault();
                spieler.IstGebucht = _bool;
                db.SaveChanges();
            }
        }

        public List<Spieler> GetFuerBuchungVerfuegbareSpieler(string tbAnzeigeText, int Spieler1Id, int Spieler2Id, int Spieler3Id, int Spieler4Id) {

            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<Spieler> alleSpieler = (from Spieler s in db.Spieler where (s.Nachname.Contains(tbAnzeigeText) || s.Vorname.Contains(tbAnzeigeText)) && (s.Id != Spieler1Id) && (s.Id != Spieler2Id) && (s.Id != Spieler3Id) && (s.Id != Spieler4Id) && s.IstGebucht == false orderby s.Nachname select s).ToList();
                List<int> idsEntfernen = (from SpielerInBearbeitung sib in db.SpielerInBearbeitung select sib.SpielerId).ToList();
                List<Spieler> toDelete = new List<Spieler>();
                foreach (int id in idsEntfernen)
                {
                    Spieler spieler = (from Spieler s in db.Spieler where s.Id == id select s).FirstOrDefault();
                    if (alleSpieler.Contains(spieler))
                    {
                        alleSpieler.Remove(spieler);
                    }
                }
                return alleSpieler;
            }
        }
    }
}

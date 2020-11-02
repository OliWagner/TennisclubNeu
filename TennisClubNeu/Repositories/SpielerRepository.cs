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
    }
}

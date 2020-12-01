using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class BuchungenRepository
    {
        static BuchungenRepository rep = null;

        private BuchungenRepository() {
        }

        public static BuchungenRepository GetInstance() {
            if (rep == null) {
                rep = new BuchungenRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public List<Buchungen> GetBuchungen() {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<Buchungen> bookings = (from Buchungen s in db.Buchungen orderby s.Endzeit descending select s).ToList();
                return bookings;
            }
        }

        public Buchungen GetBuchungById(int Id)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen booking = (from Buchungen s in db.Buchungen where s.Id == Id  select s).FirstOrDefault();
                return booking;
            }
        }

        public void RemoveBuchung(int id) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where bu.Id == id select bu).FirstOrDefault();
                db.Buchungen.Remove(buchung);
                db.SaveChanges();
            }
        }

        public void BuchungEintragen(Buchungen buchung) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                db.Buchungen.Add(buchung);
                db.SaveChanges();
            }
        }

        public string GetUhrzeitPlatzFrei(Buchungen buchung)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen _buchung = (from Buchungen b in db.Buchungen where DbFunctions.TruncateTime(b.Startzeit) == DbFunctions.TruncateTime(buchung.Startzeit) && b.PlatzId == buchung.PlatzId orderby b.Endzeit descending select b).FirstOrDefault();
                return _buchung.Endzeit.ToShortTimeString();
            }
        }

        public List<Buchungen> GetAlleAktuellenBuchungen(DateTime Vergleichszeit)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                return (from Buchungen bu in db.Buchungen
                        where bu.Startzeit < Vergleichszeit && bu.Endzeit > Vergleichszeit
                        select bu).ToList();
            }
        }

        public Buchungen GetBuchungByPlatzUndStartzeit(int platzId) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                return (from Buchungen b in db.Buchungen where b.PlatzId == platzId && b.Startzeit > DateTime.Now orderby b.Startzeit ascending select b).FirstOrDefault();
            }
            }
    }
}

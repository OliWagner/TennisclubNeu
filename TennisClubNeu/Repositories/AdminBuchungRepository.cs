using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class AdminBuchungRepository
    {
        static AdminBuchungRepository rep = null;

        private AdminBuchungRepository() {
        }

        public static AdminBuchungRepository GetInstance() {
            if (rep == null) {
                rep = new AdminBuchungRepository();
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

        public void RemoveBuchung(int id) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where bu.Id == id select bu).FirstOrDefault();
                db.Buchungen.Remove(buchung);
                db.SaveChanges();
            }
        }
    }
}

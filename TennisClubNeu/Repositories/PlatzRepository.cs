using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class PlatzRepository
    {
        static PlatzRepository rep = null;

        private PlatzRepository() {
        }

        public static PlatzRepository GetInstance() {
            if (rep == null) {
                rep = new PlatzRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public void SavePlatzsperre(List<int> arr) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<Platzsperre> liste = (from Platzsperre ps in db.Platzsperre select ps).ToList();
                db.Platzsperre.RemoveRange(liste);
                foreach (int id in arr)
                {
                    Platzsperre ps = new Platzsperre();
                    ps.PlatzId = id;
                    db.Platzsperre.Add(ps);
                }
                db.SaveChanges();
            }
        }

        public int[] GetPlatzsperren()
        {
            int[] plId;
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                plId = (from Platzsperre ps in db.Platzsperre select ps.PlatzId).ToArray();
            }
            return plId;
        }

        public List<Plätze> GetPlätze() {
            string strAnzahlPlätze = ConfigurationSettings.AppSettings.Get("AnzahlPlätze");
            int intAnzahlPlätze = Int32.Parse(strAnzahlPlätze);
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                return (from Plätze pl in db.Plätze where pl.Id <= intAnzahlPlätze orderby pl.Id ascending select pl).ToList();
            }
        }

        public GridInfo GetGridInfo(int sgtPlätzeCount)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                //In der Tabelle ist ein Fehler, die Anzahl heisst da PlatzId
                return (from GridInfo gi in db.GridInfo where gi.PlatzId == sgtPlätzeCount select gi).FirstOrDefault();
            }
        }

        public int GetPlatznummer(int PlatzId)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                return (from Plätze p in db.Plätze where p.Id == PlatzId select p.Platznummer).FirstOrDefault();
            }
        }
    }
}

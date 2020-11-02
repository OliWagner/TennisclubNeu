using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class PlatzsperreRepository
    {
        static PlatzsperreRepository rep = null;

        private PlatzsperreRepository() {
        }

        public static PlatzsperreRepository GetInstance() {
            if (rep == null) {
                rep = new PlatzsperreRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public void Save(List<int> arr) {
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
    }
}

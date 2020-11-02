using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class RechteRepository
    {
        static RechteRepository rep = null;

        private RechteRepository() {
        }

        public static RechteRepository GetInstance() {
            if (rep == null) {
                rep = new RechteRepository();
            }
            return rep;
        }

        //Methoden für Daten

    }
}

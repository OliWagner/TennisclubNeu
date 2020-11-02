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

    }
}

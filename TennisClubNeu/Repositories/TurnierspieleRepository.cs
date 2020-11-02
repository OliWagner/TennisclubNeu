using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisClubNeu.Repositories
{
    public class TurnierspieleRepository
    {
        static TurnierspieleRepository rep = null;

        private TurnierspieleRepository() {
        }

        public static TurnierspieleRepository GetInstance() {
            if (rep == null) {
                rep = new TurnierspieleRepository();
            }
            return rep;
        }

        //Methoden für Daten

    }
}

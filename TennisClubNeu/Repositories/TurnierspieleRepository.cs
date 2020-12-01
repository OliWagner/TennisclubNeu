using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisClubNeu.Classes;

namespace TennisClubNeu.Repositories
{
    public class TurnierspieleRepository
    {
        static TurnierspieleRepository rep = null;

        private TurnierspieleRepository()
        {
        }

        public static TurnierspieleRepository GetInstance()
        {
            if (rep == null)
            {
                rep = new TurnierspieleRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public bool CheckBuchungen(List<Buchungen> buchungenToCheck, string tbGuidText, out string fehler)
        {
            bool returner = true;
            StringBuilder _fehler = new StringBuilder();

            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                foreach (Buchungen buchung in buchungenToCheck)
                {
                    Buchungen b = (from Buchungen bu in db.Buchungen where bu.PlatzId == buchung.PlatzId && !bu.FesteBuchungGuid.Equals(tbGuidText) && ((bu.Startzeit <= buchung.Startzeit && bu.Endzeit > buchung.Startzeit) || (bu.Startzeit < buchung.Endzeit && bu.Endzeit >= buchung.Endzeit)) select bu).FirstOrDefault();
                    if (b != null)
                    {
                        _fehler.Append(buchung.Startzeit.ToShortDateString() + " P" + buchung.PlatzId + " " + b.Titel + "\n");
                        returner = false;
                    }

                }
                fehler = _fehler.ToString();
            }
            return returner;
        }

        public Buchungen GetBuchung(string azdfbGuid)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where bu.TurnierspielGuid.Equals(azdfbGuid) select bu).FirstOrDefault();
                return buchung;
            }
        }

        public void Save(string tbGuidText, List<Buchungen> buchungenToCheck)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                //Erst alle löschen wenn vorhanden
                if (!tbGuidText.Equals(""))
                {
                    List<Buchungen> toDelete = (from Buchungen bu in db.Buchungen where bu.TurnierspielGuid.Equals(tbGuidText) select bu).ToList();
                    db.Buchungen.RemoveRange(toDelete);
                }

                Guid guid = Guid.NewGuid();

                StringBuilder plaetze = new StringBuilder();
                foreach (Buchungen buchung in buchungenToCheck)
                {
                    buchung.TurnierspielGuid = guid.ToString().Replace("-", "");
                    db.Buchungen.Add(buchung);
                    plaetze.Append(" " + buchung.PlatzId);
                }
                db.SaveChanges();
            }
        }

        public List<AnzeigeDatenTurnierspiel> GetListBuchungen()
        {
            List<AnzeigeDatenTurnierspiel> lstDaten = new List<AnzeigeDatenTurnierspiel>();
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<string> guids = new List<string>();
                List<Buchungen> liste = (from Buchungen bu in db.Buchungen where bu.TurnierspielGuid != null orderby bu.TurnierspielGuid select bu).ToList();
                foreach (Buchungen buchung in liste)
                {
                    int[] plaetze = (from Buchungen bu in db.Buchungen where bu.TurnierspielGuid.Equals(buchung.TurnierspielGuid) select bu.PlatzId).ToArray();

                    if (!guids.Contains(buchung.TurnierspielGuid))
                    {
                        guids.Add(buchung.TurnierspielGuid);
                        AnzeigeDatenTurnierspiel daten = new AnzeigeDatenTurnierspiel();
                        daten.Von = buchung.Startzeit.ToLongTimeString();
                        daten.Bis = buchung.Endzeit.ToLongTimeString();
                        daten.Guid = buchung.TurnierspielGuid;
                        daten.Titel = buchung.Titel;
                        daten.Startdatum = buchung.Startzeit;
                        daten.Enddatum = buchung.Endzeit;
                        daten.Plätze = GetStringFromArray(plaetze);
                        lstDaten.Add(daten);
                    }
                }
                return lstDaten;
            }
        }


        private string GetStringFromArray(int[] plaetze)
        {
            StringBuilder sb = new StringBuilder();
            List<int> done = new List<int>();
            foreach (int id in plaetze)
            {
                if (!done.Contains(id))
                {
                    done.Add(id);
                    sb.Append(id + ";");
                }

            }
            return sb.ToString(0, sb.Length - 1).ToString();
        }
    }
}

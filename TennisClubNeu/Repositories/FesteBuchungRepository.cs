using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisClubNeu.Classes;

namespace TennisClubNeu.Repositories
{
    public class FesteBuchungRepository
    {
        static FesteBuchungRepository rep = null;

        private FesteBuchungRepository() {
        }

        public static FesteBuchungRepository GetInstance() {
            if (rep == null) {
                rep = new FesteBuchungRepository();
            }
            return rep;
        }

        //Methoden für Daten
        public List<AnzeigeDatenFesteBuchung> GetGridDaten()
        {
            List<AnzeigeDatenFesteBuchung> lstDaten = new List<AnzeigeDatenFesteBuchung>();
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<string> guids = new List<string>();
                List<Buchungen> liste = (from Buchungen bu in db.Buchungen where bu.FesteBuchungGuid != null orderby bu.FesteBuchungGuid select bu).ToList();
                foreach (Buchungen buchung in liste)
                {
                    int[] plaetze = (from Buchungen bu in db.Buchungen where bu.FesteBuchungGuid.Equals(buchung.FesteBuchungGuid) select bu.PlatzId).ToArray();
                    DateTime[] wochentage = (from Buchungen bu in db.Buchungen where bu.FesteBuchungGuid.Equals(buchung.FesteBuchungGuid) select bu.Startzeit).ToArray();
                    DateTime _endzeit = (from Buchungen bu in db.Buchungen where bu.FesteBuchungGuid.Equals(buchung.FesteBuchungGuid) orderby bu.Endzeit descending select bu.Endzeit).FirstOrDefault();

                    if (!guids.Contains(buchung.FesteBuchungGuid))
                    {
                        guids.Add(buchung.FesteBuchungGuid);
                        AnzeigeDatenFesteBuchung daten = new AnzeigeDatenFesteBuchung();
                        daten.Von = buchung.Startzeit.ToLongTimeString();
                        daten.Bis = buchung.Endzeit.ToLongTimeString();
                        daten.Guid = buchung.FesteBuchungGuid;
                        daten.Titel = buchung.Titel;
                        daten.Enddatum = _endzeit;
                        daten.Startdatum = wochentage.OrderBy(x => x.Minute).First();
                        daten.Plätze = GetStringFromArray(plaetze);
                        daten.Wochentag = GetWochentag(wochentage);
                        lstDaten.Add(daten);
                    }
                }
                return lstDaten;
            }
        }

        public bool CheckBuchungen(List<Buchungen> buchungenToCheck, string tbGuidText, out string fehler) {
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

        public void Save(string tbGuidText, List<Buchungen> buchungenToCheck) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                //Erst alle löschen wenn vorhanden
                if (!tbGuidText.Equals(""))
                {
                    List<Buchungen> toDelete = (from Buchungen bu in db.Buchungen where bu.FesteBuchungGuid.Equals(tbGuidText) select bu).ToList();
                    db.Buchungen.RemoveRange(toDelete);
                }

                Guid guid = Guid.NewGuid();
                List<int> plaetze = new List<int>();
                StringBuilder sb = new StringBuilder();
                foreach (Buchungen buchung in buchungenToCheck)
                {
                    buchung.FesteBuchungGuid = guid.ToString().Replace("-", "");
                    db.Buchungen.Add(buchung);
                    if (!plaetze.Contains(buchung.PlatzId))
                    {
                        plaetze.Add(buchung.PlatzId);
                        sb.Append(" " + buchung.PlatzId);
                    }
                }

                db.SaveChanges();
            }

        }

        public Buchungen GetBuchung(string azdfbGuid)
        {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Buchungen buchung = (from Buchungen bu in db.Buchungen where bu.TurnierspielGuid.Equals(azdfbGuid) select bu).FirstOrDefault();
                return buchung;
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

        private string GetWochentag(DateTime[] dts)
        {
            List<DayOfWeek> liste = new List<DayOfWeek>();
            StringBuilder sb = new StringBuilder();
            foreach (DateTime dt in dts)
            {
                if (dt.DayOfWeek == DayOfWeek.Monday && !liste.Contains(DayOfWeek.Monday))
                {
                    liste.Add(DayOfWeek.Monday);
                    sb.Append("Mo;");
                }
                if (dt.DayOfWeek == DayOfWeek.Tuesday && !liste.Contains(DayOfWeek.Tuesday))
                {
                    liste.Add(DayOfWeek.Tuesday);
                    sb.Append("Di;");
                }
                if (dt.DayOfWeek == DayOfWeek.Wednesday && !liste.Contains(DayOfWeek.Wednesday))
                {
                    liste.Add(DayOfWeek.Wednesday);
                    sb.Append("Mi;");
                }
                if (dt.DayOfWeek == DayOfWeek.Thursday && !liste.Contains(DayOfWeek.Thursday))
                {
                    liste.Add(DayOfWeek.Thursday);
                    sb.Append("Do;");
                }
                if (dt.DayOfWeek == DayOfWeek.Friday && !liste.Contains(DayOfWeek.Friday))
                {
                    liste.Add(DayOfWeek.Friday);
                    sb.Append("Fr;");
                }
                if (dt.DayOfWeek == DayOfWeek.Saturday && !liste.Contains(DayOfWeek.Saturday))
                {
                    liste.Add(DayOfWeek.Saturday);
                    sb.Append("Sa;");
                }
                if (dt.DayOfWeek == DayOfWeek.Sunday && !liste.Contains(DayOfWeek.Sunday))
                {
                    liste.Add(DayOfWeek.Sunday);
                    sb.Append("So;");
                }
            }
            return sb.ToString().Substring(0, sb.Length - 1);
        }
    }
}


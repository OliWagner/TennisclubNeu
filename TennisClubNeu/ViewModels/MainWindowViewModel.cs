using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TennisClubNeu.Classes;

namespace TennisClubNeu.ViewModels
{
    /// <summary>
    /// Liefert alle nötigen Daten für die Anzeige der einzelnen Plätze
    /// </summary>
    public class MainWindowPlatzAnzeigeViewModel
    {
        private List<AnzeigePlatzViewModel> Buchungen = new List<AnzeigePlatzViewModel>();

        public List<AnzeigePlatzViewModel> ListePlatzAnzeige = new List<AnzeigePlatzViewModel>();

        private DateTime Jetzt = DateTime.Now;

        public MainWindowPlatzAnzeigeViewModel(DateTime? AnzeigeStart) {

            DateTime Vergleichszeit = AnzeigeStart == null ? Jetzt : (DateTime)AnzeigeStart;

            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
               
                List<Buchungen> AlleAktuellenBuchungen = (from Buchungen bu in db.Buchungen where 
                            bu.Startzeit < Vergleichszeit && bu.Endzeit > Vergleichszeit select bu).ToList();
                foreach (Buchungen buchung in AlleAktuellenBuchungen)
                {
                    AnzeigePlatzViewModel apvm = new AnzeigePlatzViewModel();
                    apvm.PlatzId = GetPlatznummer(db, buchung.PlatzId);
                    
                    
                        apvm.Titelzeile = buchung.Titel;
                        apvm.AnzeigeUhrzeit = GetUhrzeitPlatzFrei(db, buchung);
                        apvm.Status = "belegt";

                        if (String.IsNullOrEmpty(buchung.FesteBuchungGuid) && String.IsNullOrEmpty(buchung.TurnierspielGuid))
                        {
                            //Platzbuchung eines Spielers
                            apvm.Zeile1 = buchung.Startzeit.ToShortTimeString();
                            apvm.Zeile2 = Helpers.GetSpielerNameById(db, buchung.Spieler1Id);
                            apvm.Zeile3 = Helpers.GetSpielerNameById(db, buchung.Spieler2Id); 
                            apvm.Zeile4 = Helpers.GetSpielerNameById(db, buchung.Spieler3Id); 
                            apvm.Zeile5 = Helpers.GetSpielerNameById(db, buchung.Spieler4Id);
                        }

                        if (String.IsNullOrEmpty(buchung.FesteBuchungGuid) && !String.IsNullOrEmpty(buchung.TurnierspielGuid))
                        {
                            //Turnierspiel
                            apvm.Zeile1 = buchung.Zeile1;
                            apvm.Zeile2 = buchung.Zeile2; 
                            apvm.Zeile3 = buchung.Zeile3; 
                            apvm.Zeile4 = buchung.Zeile4; 
                            apvm.Zeile5 = buchung.Zeile5; 
                        }

                        if (!String.IsNullOrEmpty(buchung.FesteBuchungGuid) && String.IsNullOrEmpty(buchung.TurnierspielGuid))
                        {
                            //Feste Buchung
                            apvm.Zeile1 = buchung.Zeile1; 
                            apvm.Zeile2 = buchung.Zeile2; 
                            apvm.Zeile3 = buchung.Zeile3; 
                            apvm.Zeile4 = buchung.Zeile4; 
                            apvm.Zeile5 = buchung.Zeile5; 
                        }
                    
                    Buchungen.Add(apvm);
                }
                ListePlatzAnzeige = GetPlatzListe(db, Buchungen);
            }
        }

        //private string GetSpielerNameById(TennisclubNeuEntities db, int? id) {
        //    Spieler spieler = (from Spieler s in db.Spieler where s.Id == id select s).FirstOrDefault();

        //    if (spieler == null) {
        //        return "";
        //    }                
        //    return spieler.Nachname + ", " + spieler.Vorname;
        //}

        private string GetUhrzeitPlatzFrei(TennisclubNeuEntities db, Buchungen buchung) {
            Buchungen _buchung = (from Buchungen b in db.Buchungen where DbFunctions.TruncateTime(b.Startzeit) == DbFunctions.TruncateTime(buchung.Startzeit) && b.PlatzId == buchung.PlatzId orderby b.Endzeit descending select b).FirstOrDefault();
            return _buchung.Endzeit.ToShortTimeString();
        }

        private int GetPlatznummer(TennisclubNeuEntities db, int PlatzId)
        {
            return (from Plätze p in db.Plätze where p.Id == PlatzId select p.Platznummer).FirstOrDefault();
        }

        private List<AnzeigePlatzViewModel> GetPlatzListe(TennisclubNeuEntities db, List<AnzeigePlatzViewModel> buchungen)
        {
            int[] Platzsperren;
            Platzsperren = (from Platzsperre ps in db.Platzsperre select ps.PlatzId).ToArray();
            List<AnzeigePlatzViewModel> ReturnList = new List<AnzeigePlatzViewModel>();
            List<Plätze> AllePlätze =  (from Plätze p in db.Plätze orderby p.Id ascending select p).ToList();
            foreach (Plätze platz in AllePlätze)
            {
                if (Platzsperren.Contains(platz.Id))
                {
                    //Platz ist gesperrt
                    AnzeigePlatzViewModel vm = new AnzeigePlatzViewModel();
                    vm.Titelzeile = "gesperrt";
                    vm.AnzeigeUhrzeit = "gesperrt";
                    vm.PlatzId = platz.Platznummer;
                    vm.Status = "gesperrt";
                    vm.Zeile1 = "";
                    vm.Zeile2 = "";
                    vm.Zeile3 = "";
                    vm.Zeile4 = "";
                    vm.Zeile5 = "";
                    ReturnList.Add(vm);
                }
                else
                {
                    AnzeigePlatzViewModel apvm = buchungen.Where(x => x.PlatzId == platz.Id).FirstOrDefault();
                    if (apvm != null)
                    {
                        //Buchung liegt vor
                        ReturnList.Add(apvm);
                    }
                    else
                    {
                        //es liegt keine Buchung vor
                        AnzeigePlatzViewModel vm = new AnzeigePlatzViewModel();
                        vm.Titelzeile = "frei";
                        vm.AnzeigeUhrzeit = "frei";
                        vm.PlatzId = platz.Platznummer;
                        vm.Status = "frei";
                        vm.Zeile1 = "";
                        vm.Zeile2 = "";
                        vm.Zeile3 = "";
                        vm.Zeile4 = "";
                        vm.Zeile5 = "";
                        //Achtung, es kann sein, dass der Platz innerhalb der nächsten Stunde belegt wird
                        Buchungen buchung = (from Buchungen b in db.Buchungen where b.PlatzId == platz.Id && b.Startzeit > DateTime.Now orderby b.Startzeit ascending select b).FirstOrDefault();
                        if (buchung != null) {
                            TimeSpan test = buchung.Startzeit - DateTime.Now;
                            if (test.Hours == 0) {
                                vm.Titelzeile = "In Kürze belegt";
                                vm.AnzeigeUhrzeit = "-00:" + test.Minutes;
                                vm.Zeile1 = "("+buchung.Titel+")";
                            }
                        }
                        
                        ReturnList.Add(vm);
                    }
                }
            }
            return ReturnList;
        }

    }
}

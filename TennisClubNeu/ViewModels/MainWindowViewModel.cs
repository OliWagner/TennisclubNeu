using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

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

                List<Buchungen> AlleAktuellenBuchungen = BuchungenRepository.GetInstance().GetAlleAktuellenBuchungen(Vergleichszeit);
                foreach (Buchungen buchung in AlleAktuellenBuchungen)
                {
                    AnzeigePlatzViewModel apvm = new AnzeigePlatzViewModel();
                    apvm.PlatzId = PlatzRepository.GetInstance().GetPlatznummer(buchung.PlatzId);

                        apvm.Titelzeile = buchung.Titel;
                        apvm.AnzeigeUhrzeit = BuchungenRepository.GetInstance().GetUhrzeitPlatzFrei(buchung);
                        apvm.Status = "belegt";

                        if (String.IsNullOrEmpty(buchung.FesteBuchungGuid) && String.IsNullOrEmpty(buchung.TurnierspielGuid))
                        {
                            //Platzbuchung eines Spielers
                            apvm.Zeile1 = buchung.Startzeit.ToShortTimeString();
                            apvm.Zeile2 = Helpers.GetSpielerNameById(buchung.Spieler1Id);
                            apvm.Zeile3 = Helpers.GetSpielerNameById(buchung.Spieler2Id); 
                            apvm.Zeile4 = Helpers.GetSpielerNameById(buchung.Spieler3Id); 
                            apvm.Zeile5 = Helpers.GetSpielerNameById(buchung.Spieler4Id);
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
                ListePlatzAnzeige = GetPlatzListe(Buchungen);
        }

        private List<AnzeigePlatzViewModel> GetPlatzListe(List<AnzeigePlatzViewModel> buchungen)
        {
            int[] Platzsperren;
            Platzsperren = PlatzRepository.GetInstance().GetPlatzsperren(); ;
            List<AnzeigePlatzViewModel> ReturnList = new List<AnzeigePlatzViewModel>();
            List<Plätze> AllePlätze =  PlatzRepository.GetInstance().GetPlätze();
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
                        Buchungen buchung = BuchungenRepository.GetInstance().GetBuchungByPlatzUndStartzeit(platz.Id);
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

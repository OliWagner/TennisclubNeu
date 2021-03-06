﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisClubNeu.Classes;

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
        public List<Spieler> GetSpieler() {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                List<Spieler> spieler = (from Spieler s in db.Spieler orderby s.Nachname, s.Vorname select s).ToList();
                return spieler;
            } 
        }

        public void Save(int id, bool chkBuchungen, bool chkFesteBuchungen, bool chkSpieler, bool chkTurnierspiele, bool chkRechte, bool chkPlatzsperre) {
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                Spieler spieler = (from Spieler s in db.Spieler where s.Id == id select s).FirstOrDefault();

                spieler.IstAdminBuchungen = chkBuchungen;
                spieler.IstAdminFesteBuchungen = chkFesteBuchungen;
                spieler.IstAdminSpieler = chkSpieler;
                spieler.IstAdminTurniere = chkTurnierspiele;
                spieler.IstAdminRechte = chkRechte;
                spieler.IstAdminPlatzsperre = chkPlatzsperre;
                db.SaveChanges();
            }
        }

        public Rechte GetRechteFuerAnmeldung(string anmeldeId) {
            Rechte rechte = new Rechte();
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {

                Spieler spieler = (from Spieler sp in db.Spieler where sp.ChipId.Equals(anmeldeId) select sp).FirstOrDefault();
                if (spieler == null)
                {
                    return null;
                }
                rechte.Name = spieler.Vorname + " " + spieler.Nachname;
                rechte.Id = spieler.Id;
                rechte.IsAdminBuchungen = spieler.IstAdminBuchungen;
                rechte.IsAdminFesteBuchungen = spieler.IstAdminFesteBuchungen;
                rechte.IsAdminTurnierspiele = spieler.IstAdminTurniere;
                rechte.IsAdminPlatzsperre = spieler.IstAdminPlatzsperre;
                rechte.IsAdminRechte = spieler.IstAdminRechte;
                rechte.IsAdminSpieler = spieler.IstAdminSpieler;
                return rechte;
            }
        }
    }
}

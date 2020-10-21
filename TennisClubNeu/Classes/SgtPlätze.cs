using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace TennisClubNeu.Classes
{
    public sealed class SgtPlätze
    {
        private static SgtPlätze instance = null;
        private static readonly object padlock = new object();
        public List<Plätze> Plätze { get; }


        private SgtPlätze()
        {
            string strAnzahlPlätze = ConfigurationSettings.AppSettings.Get("AnzahlPlätze");
            int intAnzahlPlätze = Int32.Parse(strAnzahlPlätze);
            using (TennisclubNeuEntities db = new TennisclubNeuEntities()) {
                Plätze = (from Plätze pl in db.Plätze where pl.Id <= intAnzahlPlätze orderby pl.Id ascending select pl).ToList();
            }
        }

        public static SgtPlätze Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SgtPlätze();
                    }
                    return instance;
                }
            }
        }
    }
}

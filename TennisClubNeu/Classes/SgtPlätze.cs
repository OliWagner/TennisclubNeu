using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.Classes
{
    public sealed class SgtPlätze
    {
        private static SgtPlätze instance = null;
        private static readonly object padlock = new object();
        public List<Plätze> Plätze { get; }


        private SgtPlätze()
        {
            Plätze = PlatzRepository.GetInstance().GetPlätze();
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

﻿using System;

namespace TennisClubNeu.Classes
{
    public class AnzeigeDatenFesteBuchung
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Enddatum { get; set; }
        public string Wochentag { get; set; }
        public string Titel { get; set; }
        public string Von { get; set; }
        public string Bis { get; set; }
        public string Plätze { get; set; }
    }
}

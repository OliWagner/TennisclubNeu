//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TennisClubNeu
{
    using System;
    using System.Collections.Generic;
    
    public partial class Buchungen
    {
        public int Id { get; set; }
        public int PlatzId { get; set; }
        public System.DateTime Startzeit { get; set; }
        public System.DateTime Endzeit { get; set; }
        public Nullable<int> Spieler1Id { get; set; }
        public Nullable<int> Spieler2Id { get; set; }
        public Nullable<int> Spieler3Id { get; set; }
        public Nullable<int> Spieler4Id { get; set; }
        public string Titel { get; set; }
        public string Zeile1 { get; set; }
        public string Zeile2 { get; set; }
        public string Zeile3 { get; set; }
        public string Zeile4 { get; set; }
        public string Zeile5 { get; set; }
        public string FesteBuchungGuid { get; set; }
        public string TurnierspielGuid { get; set; }
    }
}

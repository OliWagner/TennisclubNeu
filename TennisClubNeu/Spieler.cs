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
    
    public partial class Spieler
    {
        public int Id { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string ChipId { get; set; }
        public bool IstGebucht { get; set; }
        public bool IstAdminTurniere { get; set; }
        public bool IstAdminPlatzsperre { get; set; }
        public bool IstAdminFesteBuchungen { get; set; }
        public bool IstAdminBuchungen { get; set; }
        public bool IstAdminSpieler { get; set; }
        public bool IstAdminRechte { get; set; }
        public Nullable<bool> IstAktiv { get; set; }
    }
}
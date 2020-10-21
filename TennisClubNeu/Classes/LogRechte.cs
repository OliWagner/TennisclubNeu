using log4net;


namespace TennisClubNeu.Classes
{
    public class LogRechte
    {
        public Rechte Rechte { get; set; }
        public ILog Logger { get; set; }

        public LogRechte(Rechte rechte, ILog log) {
            Rechte = rechte;
            Logger = log;
        }       
    }
}

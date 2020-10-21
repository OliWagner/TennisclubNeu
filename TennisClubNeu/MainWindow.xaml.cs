using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TennisClubNeu.ViewModels;
using TennisClubNeu.Classes;
using TennisclubNeu;
using TennisClubNeu.UserControls;
using TennisClubNeu.UserControls.Admin;
using TennisClubNeu.UserControls.Platzbuchung;
using log4net;

namespace TennisClubNeu
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Rechte Rechte = null;
        Bedienleiste Bedienleiste = new Bedienleiste();
        public List<Plätze> sgtPlätze;
        //Dictionary<int, int[]> dic = new Dictionary<int, int[]>();
        System.Windows.Threading.DispatcherTimer anzeigeTimer;

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            //Beim STart alle SPieler frei geben die eventuell noch vorhanden sind
            Helpers.ClearSpielerInBearbeitung();
            ZeichneMainGrid();
            InitialisiereDenAnzeigeTimer();
            StarteDenAnzeigeTimer();
            AddEvents();
        }

        private void AddEvents() {
            Bedienleiste.btnAnmelden.Click += BtnBedienleisteAnmelden_Click;
            Bedienleiste.btnBuchungen.Click += BtnBedienleisteBuchungen_Click;
            Bedienleiste.btnFesteBuchungen.Click += BtnBedienleisteFesteBuchungen_Click;
            Bedienleiste.btnTurnierspiele.Click += BtnBedienleisteTurnierspiele_Click;
            Bedienleiste.btnPlatzsperre.Click += BtnBedienleistePlatzsperre_Click;
            Bedienleiste.btnSpieler.Click += BtnBedienleisteSpieler_Click;
            Bedienleiste.btnRechte.Click += BtnBedienleisteRechte_Click;
            Bedienleiste.AnmeldefeldTimer.Tick += BedienleisteAnmeldefeldTimer_Tick;
        }

        private void ZeichneMainGrid() {
            Helpers.ClearSpielerIstGebucht();
            MainWindowPlatzAnzeigeViewModel model = new MainWindowPlatzAnzeigeViewModel(null);
            //Plätze einlesen, Singleton, ändert sich zur Laufzeit nicht 
            sgtPlätze = SgtPlätze.Instance.Plätze;
            
            //Mit der Anzahl der Plätze das Template mit seinen Rows und Columns ermitteln
            using (TennisclubNeuEntities db = new TennisclubNeuEntities())
            {
                //In der Tabelle ist ein Fehler, die Anzahl heisst da PlatzId
                GridInfo info = (from GridInfo gi in db.GridInfo where gi.PlatzId == sgtPlätze.Count select gi).FirstOrDefault();
                grdMain.Children.Clear();
                grdMain = Helpers.GetMainGrid(info);
                string[] arrayCol = info.PositionsColumn.Split(';');
                string[] arrayRow = info.PositionsRow.Split(';');
                int trigger = 0;

                this.Content = grdMain;

                //Bedienleiste oben einblenden
                //Bedienleiste leiste = new Bedienleiste();
                Bedienleiste.SetValue(Grid.ColumnSpanProperty, sgtPlätze.Count > 4 ? 10 : sgtPlätze.Count + 2);
                Grid.SetRow(Bedienleiste, 0);
                Grid.SetColumn(Bedienleiste, 0);
                if (Rechte != null) {
                    Bedienleiste.SetRechte(Rechte);
                }
                grdMain.Children.Add(Bedienleiste);

                //Einzelne Grids produzieren und Auf dem Maingrid darstellen
                for (int i = 0; i < sgtPlätze.Count; i++)
                {
                    //Umbruch der Anzeige nach 6 Plätzen
                    if (i == 6) { trigger++; }
                    AnzeigePlatz ap = Helpers.GetAnzeigePlatz(model.ListePlatzAnzeige[i]);
                    if (!ap.Status.Equals("gesperrt") && Rechte != null) {
                        ap.BtnBuchen.Click += BtnBuchen_Click;
                    }
                    Grid.SetRow(ap, Int32.Parse(arrayRow[trigger]));
                    Grid.SetColumn(ap, Int32.Parse(arrayCol[i]));
                    grdMain.Children.Add(ap);
                }
            }
        }

        #region Timers
        //AnzeigeTimer
        private void InitialisiereDenAnzeigeTimer()
        {
            int x = 5;
            anzeigeTimer = new System.Windows.Threading.DispatcherTimer();
            anzeigeTimer.Tick += new EventHandler(anzeigeErneuerungTimer_Tick);
            anzeigeTimer.Interval = new TimeSpan(0, 0, x);
            
        }
        private void StarteDenAnzeigeTimer()
        {            
            anzeigeTimer.Start();
        }
        private void StoppeDenAnzeigeTimer() {
            anzeigeTimer.Stop();
        }
        void anzeigeErneuerungTimer_Tick(object sender, EventArgs e)
        {
            ZeichneMainGrid();
        }

        #endregion

        #region Events
        /// <summary>
        /// Müsste eher Abmelden heißen, angemeldet wird über den Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBedienleisteAnmelden_Click(object sender, RoutedEventArgs e)
        {
            //Bedienleiste bedienleiste = (Bedienleiste)e.Source;
            
            Bedienleiste.ResetRechte();
            Helpers.SetzeSpielerInBearbeitung(Rechte.Id, true);
            Rechte = null;
            StarteDenAnzeigeTimer();
           
            Bedienleiste.TxtAnmeldung.Text = "";
            Bedienleiste.AnmeldefeldTimer.Start();
            ZeichneMainGrid();
        }

        private void BedienleisteAnmeldefeldTimer_Tick(object sender, EventArgs e)
        {
            
            string _chipId = Bedienleiste.TxtAnmeldung.Text;

            if (!_chipId.Equals("") && Rechte == null)
            {
                Rechte = Helpers.GetRechteFuerAnmeldung(_chipId.Trim());
                if(Rechte != null) {
                    ZeichneMainGrid();
                    //Ab hier umleiten wenn bereits gebucht
                    Buchungen buchung = Helpers.CheckSpielerGebucht(Rechte.Id);
                    if (buchung != null)
                    {
                        
                        StoppeDenAnzeigeTimer();
                        
                        grdMain.Children.Clear();
                        UserControlBestaetigung ctr = new UserControlBestaetigung();
                        ctr.btnBestaetigen.Click += UcBestaetigungBtnBestaetigen_Click;
                        ctr.btnLoeschen.Click += UcBestaetigungBtnLoeschen_Click;
                        Grid.SetRow(ctr, 1);
                        Grid.SetColumn(ctr, 2);
                        ctr.SetValue(Grid.ColumnSpanProperty, 6);
                        ctr.ZeichneGrid(buchung);
                        grdMain.Children.Add(ctr);
                    }
                }
            }
            Bedienleiste.TxtAnmeldung.Text = "";
            Bedienleiste.TxtAnmeldung.Focus();
            
        }

        private void UcBestaetigungBtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            StarteDenAnzeigeTimer();
            
            ZeichneMainGrid();
        }

        private void UcBestaetigungBtnBestaetigen_Click(object sender, RoutedEventArgs e)
        {
            StarteDenAnzeigeTimer();
            BtnBedienleisteAnmelden_Click(sender, e);
            ZeichneMainGrid();
        }

        private void BtnBuchen_Click(object sender, RoutedEventArgs e)
        {
            Button _sender = (Button)sender;
            _ap = (AnzeigePlatz)e.Source;
            ZeichneAdminGrid("Buchung");
        }
        #endregion

        #region Bedienleiste Events

        private AnzeigePlatz _ap;
        private void BtnBedienleisteRechte_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("Rechte");
        }

        private void BtnBedienleisteSpieler_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("Spieler");
        }

        private void BtnBedienleistePlatzsperre_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("Platzsperre");
        }

        private void BtnBedienleisteTurnierspiele_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("Turnierspiele");
        }

        private void BtnBedienleisteFesteBuchungen_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("FesteBuchungen");
        }

        private void BtnBedienleisteBuchungen_Click(object sender, RoutedEventArgs e)
        {
            ZeichneAdminGrid("Buchungen");
        }

        private void ZeichneAdminGrid(string typ) {
            
            Bedienleiste.AnmeldefeldTimer.Stop();
            grdMain.Children.Clear();

            ColumnDefinition def1 = new ColumnDefinition();
            GridLength len = new GridLength(1000);
            def1.Width = len;
            grdMain.ColumnDefinitions.Add(def1);

            RowDefinition rd = new RowDefinition();
            GridLength len2 = new GridLength(50);
            rd.Height = len2;
            grdMain.RowDefinitions.Add(rd);

            RowDefinition rd2 = new RowDefinition();
            grdMain.RowDefinitions.Add(rd2);

            Bedienleiste.SetValue(Grid.ColumnSpanProperty, sgtPlätze.Count > 4 ? 10 : sgtPlätze.Count + 2);
            Grid.SetRow(Bedienleiste, 0);
            Grid.SetColumn(Bedienleiste, 0);
            grdMain.Children.Add(Bedienleiste);

            if (typ.Equals("Turnierspiele"))
            {
                UserControlTurnierspiele ctr = new UserControlTurnierspiele(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                ctr.SetValue(Grid.ColumnSpanProperty, 6);
                ctr.ZeichneGrid(sgtPlätze);
                grdMain.Children.Add(ctr);
            }

            if (typ.Equals("Rechte"))
            {
                UserControlRechte ctr = new UserControlRechte(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                ctr.SetValue(Grid.ColumnSpanProperty, 8);
                ctr.SetValue(Grid.RowSpanProperty, 3);
                grdMain.Children.Add(ctr);
            }

            if (typ.Equals("Buchungen"))
            {
                UserControlAdminBuchung ctr = new UserControlAdminBuchung(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                ctr.SetValue(Grid.ColumnSpanProperty, 6);
                grdMain.Children.Add(ctr);
            }

            if (typ.Equals("Spieler"))
            {
                UserControlSpieler ctr = new UserControlSpieler(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                ctr.SetValue(Grid.ColumnSpanProperty, 3);
                grdMain.Children.Add(ctr);
                
            }

            if (typ.Equals("FesteBuchungen"))
            {
                UserControlFesteBuchung ctr = new UserControlFesteBuchung(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                ctr.SetValue(Grid.ColumnSpanProperty, 6);
                ctr.ZeichneGrid(sgtPlätze);
                grdMain.Children.Add(ctr);
            }

            if (typ.Equals("Buchung"))
            {
                if (Rechte == null)
                {
                    ZeichneMainGrid();
                }
                else
                {
                    UserControlPlatzbuchung ctr = new UserControlPlatzbuchung();
                    Grid.SetRow(ctr, 1);
                    Grid.SetColumn(ctr, 1);
                    ctr.SetValue(Grid.ColumnSpanProperty, 8);
                    ctr.SetValue(Grid.RowSpanProperty, 3);
                    ctr.btnAbbrechen.Click += BtnPlatzbuchungAbbrechen_Click;
                    ctr.btnBuchungSpeichern.Click += BtnPlatzbuchungSpeichern_Click;
                    ctr.InitializeData(_ap, Rechte.Id);
                    grdMain.Children.Add(ctr);
                }
            }

            if (typ.Equals("Platzsperre")) {
                UserControlPlatzsperre ctr = new UserControlPlatzsperre(new LogRechte(Rechte, log));
                Grid.SetRow(ctr, 1);
                Grid.SetColumn(ctr, 1);
                grdMain.Children.Add(ctr);
                ctr.ZeichneGrid(sgtPlätze);
            }

            StoppeDenAnzeigeTimer();
        }

        private void BtnPlatzbuchungAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            StarteDenAnzeigeTimer();
            
            ZeichneMainGrid();
        }

        private void BtnPlatzbuchungSpeichern_Click(object sender, RoutedEventArgs e)
        {
            StarteDenAnzeigeTimer();
            BtnBedienleisteAnmelden_Click(sender, e);
            ZeichneMainGrid();
        }
        #endregion
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TennisClubNeu.Classes;

namespace TennisClubNeu.UserControls
{
    /// <summary>
    /// Interaktionslogik für Bedienleiste.xaml
    /// </summary>
    public partial class Bedienleiste : UserControl
    {
        Rechte Rechte;
        private System.Windows.Threading.DispatcherTimer anmeldefeldTimer;

        public DispatcherTimer AnmeldefeldTimer { get => anmeldefeldTimer; set => anmeldefeldTimer = value; }

        public Bedienleiste()
        {
            InitializeComponent();
            SetButtons();
            StarteTimer();
        }

        private void StarteTimer() {
            int x = 1;
            AnmeldefeldTimer = new System.Windows.Threading.DispatcherTimer();
            AnmeldefeldTimer.Tick += new EventHandler(AnmeldefeldTimer_Tick);
            AnmeldefeldTimer.Interval = new TimeSpan(0, 0, x);
            AnmeldefeldTimer.Start();
        }

        private void AnmeldefeldTimer_Tick(object sender, EventArgs e)
        {
            //Wird in MainWindow behandelt
        }

        private void SetButtons()
        {
            if(Rechte == null)
            {
                btnAnmelden.Visibility = Visibility.Hidden;
                btnBuchungen.Visibility = Visibility.Hidden;
                btnFesteBuchungen.Visibility = Visibility.Hidden;
                btnTurnierspiele.Visibility = Visibility.Hidden;
                btnRechte.Visibility = Visibility.Hidden;
                btnPlatzsperre.Visibility = Visibility.Hidden;
                btnSpieler.Visibility = Visibility.Hidden;
            }
            else
            {
                btnAnmelden.Visibility = Visibility.Visible;
                btnAnmelden.Content = "Logout";
                btnBuchungen.Visibility = Rechte.IsAdminBuchungen ? Visibility.Visible : Visibility.Hidden;
                btnFesteBuchungen.Visibility = Rechte.IsAdminFesteBuchungen ? Visibility.Visible : Visibility.Hidden;
                btnTurnierspiele.Visibility = Rechte.IsAdminTurnierspiele ? Visibility.Visible : Visibility.Hidden;
                btnRechte.Visibility = Rechte.IsAdminRechte ? Visibility.Visible : Visibility.Hidden;
                btnPlatzsperre.Visibility = Rechte.IsAdminPlatzsperre ? Visibility.Visible : Visibility.Hidden;
                btnSpieler.Visibility = Rechte.IsAdminSpieler ? Visibility.Visible : Visibility.Hidden;
                
            }
        }

        public void SetRechte(Rechte rechte) {
            Rechte = rechte;
            lblAngemeldet.Content = rechte.Name;
            SetButtons();
        }

        public void ResetRechte() {
            Rechte = null;
            lblAngemeldet.Content = "";
            SetButtons();
        }

        private void BtnAnmelden_Click(object sender, RoutedEventArgs e)
        {
            e.Source = this;
        }
        #region an MainWindow durchgereichte Events
        private void BtnPlatzsperre_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }

        private void BtnSpieler_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }

        private void BtnBuchungen_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }

        private void BtnFesteBuchungen_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }

        private void BtnTurnierspiele_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }

        private void BtnRechte_Click(object sender, RoutedEventArgs e)
        {
            //Handled in MainWindow.cs
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls.Admin
{
    
    /// <summary>
    /// Interaktionslogik für UserControlFesteBuchung.xaml
    /// </summary>
    public partial class UserControlFesteBuchung : UserControl
    {
        List<Plätze> ListPlätze;
        List<Buchungen> buchungenToCheck;

        public UserControlFesteBuchung()
        {
            InitializeComponent();
        }

        public void ZeichneGrid(List<Plätze> sgtPlätze) {
            if(ListPlätze == null)
                ListPlätze = sgtPlätze;

            string[] _hours = { "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
            string[] _minutes = { "00", "15", "30", "45" };

            int checkrows = 7;

            int counterRows = 1;
            int counterColumns = 0;
            foreach (var item in sgtPlätze)
            {
                CheckBox chk = new CheckBox();
                chk.Name = "chkPlatz" + item.Platznummer;
                chk.Content = "Platz " + item.Platznummer;
                chk.Unchecked += GrdMain_Changed;
                chk.Checked += GrdMain_Changed;
                chk.Width = 80;
                chk.HorizontalAlignment = HorizontalAlignment.Left;
                chk.Margin = new Thickness(counterColumns * 60, 0, 0, 0);
                Grid.SetColumn(chk, 1);
                Grid.SetRow(chk, counterRows);
                grdMain.Children.Add(chk);
                counterColumns++;
                if (counterColumns % checkrows == 0) {
                    counterRows++;
                    counterColumns = 0;
                }
            }

            cboStundenStart.ItemsSource = _hours;
            cboStundenEnde.ItemsSource = _hours;
            cboMinutenStart.ItemsSource = _minutes;
            cboMinutenEnde.ItemsSource = _minutes;
            ZeichneDatagrid();
        }

        private void ZeichneDatagrid() {
            List<AnzeigeDatenFesteBuchung> lstDaten = FesteBuchungRepository.GetInstance().GetGridDaten();
            dataGrid.ItemsSource = lstDaten;
            
        }

        #region Eventhandler
        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Id")
                e.Column.Visibility = System.Windows.Visibility.Hidden;
            if (e.Column.Header.ToString() == "Guid")
                e.Column.Visibility = System.Windows.Visibility.Hidden;
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
        }

        private bool CheckBuchungen() {
            
            buchungenToCheck = BaueBuchungen();
            string fehler = "";
            bool returner = FesteBuchungRepository.GetInstance().CheckBuchungen(buchungenToCheck, tbGuid.Text, out fehler);
            tblWarnings.Text = fehler;
            return returner;
        }

        private List<Buchungen> BaueBuchungen() {
            //Startdatum
            DateTime _startDatum = (DateTime)dpStart.SelectedDate;
            
            //Enddatum
            DateTime _endDatum = (DateTime)dpEnde.SelectedDate;
            
            //Plätze
            List<int> _plaetze = new List<int>();
            foreach (Plätze platz in ListPlätze)
            {
                CheckBox checkBox = Helpers.FindChild<CheckBox>(grdMain, "chkPlatz" + platz.Platznummer.ToString());
                if ((bool)checkBox.IsChecked) {
                    _plaetze.Add(platz.Platznummer);
                }
            }

            //Wochentage
            List<DayOfWeek> _wochentage = new List<DayOfWeek>();
            if ((bool)chkMontag.IsChecked) { _wochentage.Add(DayOfWeek.Monday); }
            if ((bool)chkDienstag.IsChecked) { _wochentage.Add(DayOfWeek.Tuesday); }
            if ((bool)chkMittwoch.IsChecked) { _wochentage.Add(DayOfWeek.Wednesday); }
            if ((bool)chkDonnerstag.IsChecked) { _wochentage.Add(DayOfWeek.Thursday); }
            if ((bool)chkFreitag.IsChecked) { _wochentage.Add(DayOfWeek.Friday); }
            if ((bool)chkSamtag.IsChecked) { _wochentage.Add(DayOfWeek.Saturday); }
            if ((bool)chkSonntag.IsChecked) { _wochentage.Add(DayOfWeek.Sunday); }

            //StartZeit
            int _szHour = Int32.Parse(cboStundenStart.SelectedValue.ToString());
            int _szMinutes = Int32.Parse(cboMinutenStart.SelectedValue.ToString());

            //Endzeit
            int _ezHour = Int32.Parse(cboStundenEnde.SelectedValue.ToString());
            int _ezMinutes = Int32.Parse(cboMinutenEnde.SelectedValue.ToString());

            //Alle Daten zusammen, Liste bauen
            List<Buchungen> ReturnList = new List<Buchungen>();
            while (_startDatum <= _endDatum) {

                if (_wochentage.Contains(_startDatum.DayOfWeek))
                {
                    foreach (int platznummer in _plaetze) {
                        Buchungen buchung = new Buchungen();
                        buchung.Startzeit = new DateTime(_startDatum.Year, _startDatum.Month, _startDatum.Day, _szHour, _szMinutes, 0);
                        buchung.Endzeit = new DateTime(_startDatum.Year, _startDatum.Month, _startDatum.Day, _ezHour, _ezMinutes, 0);
                        buchung.PlatzId = platznummer;
                        buchung.Titel = txtTitel.Text;
                        buchung.Zeile1 = tbZeile1.Text;
                        buchung.Zeile2 = tbZeile2.Text;
                        buchung.Zeile3 = tbZeile3.Text;
                        buchung.Zeile4 = tbZeile4.Text;
                        buchung.Zeile5 = tbZeile5.Text;
                        ReturnList.Add(buchung);
                    }
                }
                _startDatum =  _startDatum.AddDays(1);
            }
            return ReturnList.OrderBy(x => x.Startzeit).ToList();
        }
        

        private void BtnSpeichern_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //No need to implement
        }

        private void BtnSpeichern_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CheckFelder()) {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
        }

        private bool CheckFelder() {

            if (txtTitel.Text.Length < 2) { return false; }

            List<int> ids = new List<int>();
            foreach (Plätze platz in ListPlätze)
            {
                var checkBox = Helpers.FindChild<CheckBox>(grdMain, "chkPlatz" + platz.Platznummer);
                if ((bool)checkBox.IsChecked) {
                    ids.Add(platz.Platznummer);
                }
            }
            if (ids.Count == 0) { return false; }

            if (cboMinutenEnde.SelectedItem == null) { return false; }
            if (cboMinutenStart.SelectedItem == null) { return false; }
            if (cboStundenEnde.SelectedItem == null) { return false; }
            if (cboStundenStart.SelectedItem == null) { return false; }

            if (dpStart.SelectedDate == null) { return false; }
            if (dpEnde.SelectedDate == null) { return false; }
            if (dpStart.SelectedDate >= dpEnde.SelectedDate) { return false; }

            bool checkWd = false;
            if ((bool)chkMontag.IsChecked) checkWd = true;
            if ((bool)chkDienstag.IsChecked) checkWd = true;
            if ((bool)chkMittwoch.IsChecked) checkWd = true;
            if ((bool)chkDonnerstag.IsChecked) checkWd = true;
            if ((bool)chkFreitag.IsChecked) checkWd = true;
            if ((bool)chkSamtag.IsChecked) checkWd = true;
            if ((bool)chkSonntag.IsChecked) checkWd = true;
            if (!checkWd) return false;
            return true;
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            string tbGuidText = tbGuid.Text;
            if (btnSpeichern.Content.Equals("Speichern")) {
                FesteBuchungRepository.GetInstance().Save(tbGuid.Text, buchungenToCheck);
                Reset();
                ZeichneDatagrid();
            } else {
                if (CheckBuchungen()) {
                    btnSpeichern.Content = "Speichern";
                }
            }
        }

        private void Reset() {
            buchungenToCheck = null;
            tbGuid.Text = "";
            tblWarnings.Text = "";
            cboStundenStart.SelectedItem = null;
            cboMinutenStart.SelectedItem = null;

            cboStundenEnde.SelectedItem = null;
            cboMinutenEnde.SelectedItem = null;

            dpStart.SelectedDate = null;
            dpEnde.SelectedDate = null;

            txtTitel.Text = "";
            tbZeile1.Text = "";
            tbZeile2.Text = "";
            tbZeile3.Text = "";
            tbZeile4.Text = "";
            tbZeile5.Text = "";

            chkMontag.IsChecked = false; 
            chkDienstag.IsChecked = false; 
            chkMittwoch.IsChecked = false; 
            chkDonnerstag.IsChecked = false; 
            chkFreitag.IsChecked = false; 
            chkSamtag.IsChecked = false; 
            chkSonntag.IsChecked = false; 

            foreach (Plätze p in ListPlätze)
            {
                CheckBox checkBox = Helpers.FindChild<CheckBox>(grdMain, "chkPlatz" + p.Platznummer.ToString());

                checkBox.IsChecked = false;
            }
        }
    

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Reset();
            DataGrid grid = (DataGrid)sender;
            AnzeigeDatenFesteBuchung azdfb = (AnzeigeDatenFesteBuchung)grid.SelectedItem;
            tbGuid.Text = azdfb.Guid;
            cboStundenStart.SelectedItem = azdfb.Startdatum.ToShortTimeString().Split(':')[0];
            cboMinutenStart.SelectedItem = azdfb.Startdatum.ToShortTimeString().Split(':')[1];

            cboStundenEnde.SelectedItem = azdfb.Enddatum.ToShortTimeString().Split(':')[0];
            cboMinutenEnde.SelectedItem = azdfb.Enddatum.ToShortTimeString().Split(':')[1];

            dpStart.SelectedDate = azdfb.Startdatum;
            dpEnde.SelectedDate = azdfb.Enddatum;

            txtTitel.Text = azdfb.Titel;

            string[] wochentage = azdfb.Wochentag.Split(';');
            if (wochentage.Contains("Mo")) { chkMontag.IsChecked = true; }
            if (wochentage.Contains("Di")) { chkDienstag.IsChecked = true; }
            if (wochentage.Contains("Mi")) { chkMittwoch.IsChecked = true; }
            if (wochentage.Contains("Do")) { chkDonnerstag.IsChecked = true; }
            if (wochentage.Contains("Fr")) { chkFreitag.IsChecked = true; }
            if (wochentage.Contains("Sa")) { chkSamtag.IsChecked = true; }
            if (wochentage.Contains("So")) { chkSonntag.IsChecked = true; }

            string[] platzNummern = azdfb.Plätze.Split(';');
            foreach (string  id in platzNummern)
            {
                var checkBox = Helpers.FindChild<CheckBox>(grdMain, "chkPlatz"+id);

                checkBox.IsChecked = true;
            }
                Buchungen buchung = FesteBuchungRepository.GetInstance().GetBuchung(azdfb.Guid);
                tbZeile1.Text = buchung.Zeile1;
                tbZeile2.Text = buchung.Zeile2;
                tbZeile3.Text = buchung.Zeile3;
                tbZeile4.Text = buchung.Zeile4;
                tbZeile5.Text = buchung.Zeile5;
        }

        #endregion
        //public static T FindChild<T>(DependencyObject parent, string childName)
        //where T : DependencyObject
        //{
        //    // Confirm parent and childName are valid. 
        //    if (parent == null)
        //    {
        //        return null;
        //    }

        //    T foundChild = null;

        //    int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        //    for (int i = 0; i < childrenCount; i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        //        // If the child is not of the request child type child
        //        var childType = child as T;
        //        if (childType == null)
        //        {
        //            // recursively drill down the tree
        //            foundChild = FindChild<T>(child, childName);

        //            // If the child is found, break so we do not overwrite the found child. 
        //            if (foundChild != null)
        //            {
        //                break;
        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(childName))
        //        {
        //            var frameworkElement = child as FrameworkElement;
        //            // If the child's name is set for search
        //            if (frameworkElement != null && frameworkElement.Name == childName)
        //            {
        //                // if the child's name is of the request name
        //                foundChild = (T)child;
        //                break;
        //            }

        //            // Need this in case the element we want is nested
        //            // in another element of the same type
        //            foundChild = FindChild<T>(child, childName);
        //        }
        //        else
        //        {
        //            // child element found.
        //            foundChild = (T)child;
        //            break;
        //        }
        //    }

        //    return foundChild;
        //}

        private void GrdMain_Changed(object sender, RoutedEventArgs e)
        {
            if (btnSpeichern.Content.Equals("Speichern")){
                btnSpeichern.Content = "Check Termine";
            }
        }
    }

}


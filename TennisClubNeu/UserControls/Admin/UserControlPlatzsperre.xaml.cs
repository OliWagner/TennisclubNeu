using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TennisClubNeu.Classes;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.UserControls.Admin
{
    /// <summary>
    /// Interaktionslogik für UserControlPlatzsperre.xaml
    /// </summary>
    public partial class UserControlPlatzsperre : UserControl
    {

        public UserControlPlatzsperre()
        {
            InitializeComponent();
        }

        private void ErstelleGrid(List<Plätze> sgtPlätze) {
            grdMain.Children.Clear();
            ColumnDefinition cd = new ColumnDefinition();
            GridLength len = new GridLength(200);
            cd.Width = len;
            grdMain.ColumnDefinitions.Add(cd);

            ColumnDefinition cd2 = new ColumnDefinition();
            grdMain.ColumnDefinitions.Add(cd2);

            foreach (Plätze item in sgtPlätze)
            {
                RowDefinition rd = new RowDefinition();
                grdMain.RowDefinitions.Add(rd);
            }
            RowDefinition buttons = new RowDefinition();
            grdMain.RowDefinitions.Add(buttons);
        }

        public void ZeichneGrid(List<Plätze> sgtPlätze) {
            ErstelleGrid(sgtPlätze);
            int counter = 0;



            int[] plId = PlatzsperreRepository.GetInstance().GetPlatzsperren(); ;

                foreach (Plätze item in sgtPlätze)
                {
                    Label lbl = new Label();
                    
                    Grid.SetColumn(lbl, 0);
                    Grid.SetRow(lbl, counter);
                    grdMain.Children.Add(lbl);

                    CheckBox chk = new CheckBox();
                    chk.Content = "Platz " + item.Platznummer;
                    chk.IsChecked = plId.Contains(item.Platznummer);
                    Grid.SetColumn(chk, 1);
                    Grid.SetRow(chk, counter);
                    grdMain.Children.Add(chk);
                    counter++;
                }

            Button btn = new Button();
            btn.Content = "Speichern";
            btn.Click += BtnSpeichern_Click;
            Grid.SetColumn(btn, 1);
            Grid.SetRow(btn, counter);
            grdMain.Children.Add(btn);
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            List<int> sperren = new List<int>();
            

                foreach (DependencyObject obj in grdMain.Children)
                {
                    if (obj.GetType() == typeof(CheckBox)) {
                        CheckBox chk = (CheckBox)obj;
                        if ((bool)chk.IsChecked) {
                            string wert = chk.Content.ToString().Split(' ')[1];
                            int intwert = Int32.Parse(wert);

                            sperren.Add(intwert);
                        }
                    }
                }
            PlatzsperreRepository.GetInstance().Save(sperren);    
        }
    }
}

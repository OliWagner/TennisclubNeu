using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TennisClubNeu.Classes;

namespace TennisclubNeu
{
    /// <summary>
    /// Interaktionslogik für ucAnzeigePlatz.xaml
    /// </summary>
    public partial class AnzeigePlatz : UserControl
    {
        private int platzId;
        private string status;

        public int PlatzId { get => platzId; set => platzId = value; }
        public string Status { get => status; set => status = value; }

        public AnzeigePlatz(AnzeigePlatzViewModel apvm)
        {
            InitializeComponent();

            Status = apvm.Status;
            SetBackgroundImage(apvm.Status);
            ucAnzeigePlatzBackgroundImage = null;
            Platznummer.Content = apvm.PlatzId;
            PlatzId = apvm.PlatzId;
            Titel.Content = apvm.Titelzeile;
            Uhrzeit.Content = apvm.AnzeigeUhrzeit;
            Zeile1.Content = apvm.Zeile1;
            Zeile2.Content = apvm.Zeile2;
            Zeile3.Content = apvm.Zeile3;
            Zeile4.Content = apvm.Zeile4;
            Zeile5.Content = apvm.Zeile5;
        }

        public AnzeigePlatz() {
            
        }

        private void SetBackgroundImage(string Status)
        {
            string txt = "";
            switch (Status)
            {
                case "frei": txt = "Free"; break;
                case "belegt": txt = "Pending"; break;
                case "gesperrt": txt = "Taken"; break;
            }

            BitmapImage pic = new BitmapImage();
            pic.BeginInit();
            pic.UriSource = new Uri(@"/Images/" + txt + ".png", UriKind.RelativeOrAbsolute);
            pic.EndInit();
            ucAnzeigePlatzBackgroundImage.Source = pic;
        }

        private void BtnBuchen_Click(object sender, RoutedEventArgs e)
        {
            e.Source = this;
        }
    }

}


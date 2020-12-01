using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TennisclubNeu;
using TennisClubNeu.Repositories;

namespace TennisClubNeu.Classes
{
    public static class Helpers
    {
        public static Grid GetMainGrid(GridInfo info) {
            Grid Returngrid = new Grid();
                
                //GridColums
                for (int i = 1; i <= info.Columns; i++)
                {
                    //Abwechselnd Space und Content
                    if (i % 2 == 0)
                    {
                        //Content
                        ColumnDefinition cd = new ColumnDefinition();
                        GridLength len = new GridLength(info.ContentColumnWidth);
                        cd.Width = len;
                        Returngrid.ColumnDefinitions.Add(cd);
                    }
                    else
                    {
                        //Space
                        ColumnDefinition cd = new ColumnDefinition();
                        GridLength len = new GridLength(info.SpaceColumnWidth);
                        cd.Width = len;
                        Returngrid.ColumnDefinitions.Add(cd);
                    }
                }
                //GridRows
                for (int i = 1; i <= info.Rows; i++)
                {
                    //Abwechselnd Space und Content
                    if (i % 2 == 0)
                    {
                        //Content
                        RowDefinition rd = new RowDefinition();
                        GridLength len = new GridLength(info.ContentRowHeight);
                        rd.Height = len;
                        Returngrid.RowDefinitions.Add(rd);
                    }
                    else
                    {
                        //Space
                        RowDefinition rd = new RowDefinition();
                        GridLength len = new GridLength(info.SpaceRowHeight);
                        rd.Height = len;
                        Returngrid.RowDefinitions.Add(rd);
                    }
                }
                return Returngrid;
            }
            
        public static AnzeigePlatz GetAnzeigePlatz(AnzeigePlatzViewModel model) {
            return new AnzeigePlatz(model);
        }

        public static Rechte GetRechteFuerAnmeldung(string anmeldeId) {
            Rechte rechte = RechteRepository.GetInstance().GetRechteFuerAnmeldung(anmeldeId);
            return rechte;
        }

        public static string GetSpielerNameById(int? id)
        {
            Spieler spieler = SpielerRepository.GetInstance().GetSpielerById((int)id);

            if (spieler == null)
            {
                return "";
            }
            return spieler.Nachname + ", " + spieler.Vorname;
        }
        /// <summary>
        /// Ist Bool true wird der Spieler entfernt, bei False erfasst
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="_bool"></param>
        public static void SetzeSpielerInBearbeitung(int Id, bool _bool) {
            SpielerRepository.GetInstance().SetzeSpielerInBearbeitung(Id, _bool);
        }

        public static void ClearSpielerInBearbeitung()
        {
            SpielerRepository.GetInstance().ClearSpielerInBearbeitung();
        }

        public static void ClearSpielerIstGebucht() {
            SpielerRepository.GetInstance().ClearSpielerIstGebucht();
        }

        public static Buchungen CheckSpielerGebucht(int spielerId) {
            return SpielerRepository.GetInstance().CheckSpielerGebucht(spielerId);
        }

        /// <summary>
        /// Bei True wir der Spieler auf IstGebucht gesetzt
        /// </summary>
        /// <param name="spielerId"></param>
        /// <param name="_bool"></param>
        public static void SetzeSpielerIstGebucht(int spielerId, bool _bool) {
            SpielerRepository.GetInstance().SetzeSpielerIstGebucht(spielerId, _bool);
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);
                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                    // Need this in case the element we want is nested
                    // in another element of the same type
                    foundChild = FindChild<T>(child, childName);
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TennisclubNeu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisClubNeu.Classes;

namespace TennisclubNeu.Tests
{
    [TestClass()]
    public class AnzeigePlatzTests
    {
        [TestMethod()]
        public void AnzeigePlatzTest()
        {
            AnzeigePlatzViewModel apvm = new AnzeigePlatzViewModel();
            apvm.PlatzId = 1;
            apvm.Status = "Status";
            apvm.AnzeigeUhrzeit = "00:00";
            apvm.Titelzeile = "Titel";
            apvm.Zeile1 = "Zeile1";
            apvm.Zeile2 = "Zeile2";
            apvm.Zeile3 = "Zeile3";
            apvm.Zeile4 = "Zeile4";
            apvm.Zeile5 = "Zeile5";

            AnzeigePlatz ap = new AnzeigePlatz(apvm);

            Assert.IsTrue(ap.PlatzId == 1);
        }
    }
}
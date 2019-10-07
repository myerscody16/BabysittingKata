/* using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PillarTechBabysittingKata.Controllers;
using PillarTechBabysittingKata.Models;

namespace Tests {
    public class Test {
        [SetUp]
        public void Setup () { }

        [TestCase ("05:00:00", "12:00:00", ExpectedResult = 110)]
        [TestCase ("07:00:00", "2:00:00", ExpectedResult = 120)]
        [TestCase ("06:00:00", "04:00:00", ExpectedResult = 175)]
        public static int TestCalcFamA (string start, string end) {
            int TotalCost = 0;
            Appointments newAppointment = new Appointments ();
            newAppointment.FamilyId = "A";
            newAppointment.StartDate = Convert.ToDateTime ("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse (start);
            newAppointment.EndTime = TimeSpan.Parse (end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            //TotalCost = HomeController.CalculateFamilyA(newAppointment);
            return TotalCost;
        }

        [TestCase ("05:00:00", "12:00:00", ExpectedResult = 76)]
        [TestCase ("07:00:00", "2:00:00", ExpectedResult = 84)]
        [TestCase ("06:00:00", "04:00:00", ExpectedResult = 128)]
        public int TestCalcFamB (string start, string end) {
            int TotalCost = 0;
            Appointments newAppointment = new Appointments ();
            newAppointment.FamilyId = "B";
            newAppointment.StartDate = Convert.ToDateTime ("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse (start);
            newAppointment.EndTime = TimeSpan.Parse (end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            //TotalCost = HomeController.CalculateFamilyB(newAppointment);
            return TotalCost;
        }

        [TestCase ("05:00:00", "12:00:00", ExpectedResult = 129)]
        [TestCase ("07:00:00", "2:00:00", ExpectedResult = 117)]
        [TestCase ("06:00:00", "04:00:00", ExpectedResult = 168)]
        public int TestCalcFamC (string start, string end) {
            int TotalCost = 0;
            Appointments newAppointment = new Appointments ();
            newAppointment.FamilyId = "C";
            newAppointment.StartDate = Convert.ToDateTime ("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse (start);
            newAppointment.EndTime = TimeSpan.Parse (end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            // TotalCost = HomeController.CalculateFamilyC(newAppointment);
            return TotalCost;
        }
    }
} */
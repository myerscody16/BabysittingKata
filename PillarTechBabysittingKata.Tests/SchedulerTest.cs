using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PillarTechBabysittingKata.Models;
using PillarTechBabysittingKata.Services;

namespace PillarTechBabysittingKata.Tests
{
    public class SchedulerTest
    {
        Mock<BabysittingDbContext> mockContext;
        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<BabysittingDbContext>();

            var appointments = new List<Appointments>()
            {
                //new Appointments () { Id = 1, FamilyId = "A", StartDate = Convert.ToDateTime ("2019-10-4"), StartTime = TimeSpan.Parse ("05:00:00"), EndTime = TimeSpan.Parse ("12:00:00"), TotalCost = 0 },
                //new Appointments () { Id = 1, FamilyId = "C", StartDate = Convert.ToDateTime ("2019-10-4"), StartTime = TimeSpan.Parse ("05:00:00"), EndTime = TimeSpan.Parse ("12:00:00"), TotalCost = 0 }
            };
            var appointmentsQueryable = appointments.AsQueryable();
            var appointmentsMockSet = new Mock<DbSet<Appointments>>();

            appointmentsMockSet.As<IQueryable<Appointments>>().Setup(m => m.Expression).Returns(appointmentsQueryable.Expression);
            appointmentsMockSet.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointmentsQueryable.ElementType);
            appointmentsMockSet.As<IQueryable<Appointments>>().Setup(m => m.ElementType).Returns(appointmentsQueryable.ElementType);
            appointmentsMockSet.As<IQueryable<Appointments>>().Setup(m => m.GetEnumerator()).Returns(appointmentsQueryable.GetEnumerator);

            mockContext.Setup(x => x.Appointments).Returns(appointmentsMockSet.Object);

            var familyPayRates = new List<FamilyPayRates>()
            {
                new FamilyPayRates() { Id = 1, FamilyLetter = "A", PayRate = 15, StartTime = TimeSpan.Parse("05:00:00"), EndTime = TimeSpan.Parse("11:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "A", PayRate = 20, StartTime = TimeSpan.Parse("11:00:00"), EndTime = TimeSpan.Parse("04:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "B", PayRate = 12, StartTime = TimeSpan.Parse("05:00:00"), EndTime = TimeSpan.Parse("10:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "B", PayRate = 8, StartTime = TimeSpan.Parse("10:00:00"), EndTime = TimeSpan.Parse("12:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "B", PayRate = 16, StartTime = TimeSpan.Parse("12:00:00"), EndTime = TimeSpan.Parse("04:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "C", PayRate = 21, StartTime = TimeSpan.Parse("05:00:00"), EndTime = TimeSpan.Parse("09:00:00") },
                new FamilyPayRates() { Id = 1, FamilyLetter = "C", PayRate = 15, StartTime = TimeSpan.Parse("09:00:00"), EndTime = TimeSpan.Parse("04:00:00") },
            };
            var familyPayRatesQueryable = familyPayRates.AsQueryable();
            var familyPayRatesMockSet = new Mock<DbSet<FamilyPayRates>>();

            familyPayRatesMockSet.As<IQueryable<FamilyPayRates>>().Setup(m => m.Expression).Returns(familyPayRatesQueryable.Expression);
            familyPayRatesMockSet.As<IQueryable<FamilyPayRates>>().Setup(m => m.Provider).Returns(familyPayRatesQueryable.Provider);
            familyPayRatesMockSet.As<IQueryable<FamilyPayRates>>().Setup(m => m.ElementType).Returns(familyPayRatesQueryable.ElementType);
            familyPayRatesMockSet.As<IQueryable<FamilyPayRates>>().Setup(m => m.GetEnumerator()).Returns(familyPayRatesQueryable.GetEnumerator);

            mockContext.Setup(x => x.FamilyPayRates).Returns(familyPayRatesMockSet.Object);
        }
        /// <summary>
        /// Test CalculateFamilyA with a null argument thow an exception
        /// </summary>
        [Test]
        public void CalculateFamilyAWithNullAppointmentShouldThrow()
        {
            var scheduler = new Scheduler(mockContext.Object);
            Assert.Throws<System.NullReferenceException>(
                delegate { scheduler.CalculateFamilyA(null); });
        }

        [TestCase ("05:00:00", "12:00:00", ExpectedResult = 110)]
        [TestCase("07:00:00", "2:00:00", ExpectedResult = 120)]
        [TestCase("06:00:00", "04:00:00", ExpectedResult = 175)]
        [TestCase("05:00:00", "10:00:00", ExpectedResult = 75)]
        [TestCase("07:00:00", "12:00:00", ExpectedResult = 80)]
        [Test]
        public int TestCalcFamA(string start, string end)
        {
            var scheduler = new Scheduler(mockContext.Object);
            int TotalCost;
            Appointments newAppointment = new Appointments();
            newAppointment.FamilyId = "A";
            newAppointment.StartDate = Convert.ToDateTime("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse(start);
            newAppointment.EndTime = TimeSpan.Parse(end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            TotalCost = scheduler.CalculateFamilyA(newAppointment);
            return TotalCost;
        }

        [TestCase("05:00:00", "12:00:00", ExpectedResult = 76)]
        [TestCase ("07:00:00", "2:00:00", ExpectedResult = 84)]
        [TestCase ("06:00:00", "04:00:00", ExpectedResult = 128)]
        [TestCase("05:00:00", "11:00:00", ExpectedResult = 68)]
        [TestCase("09:00:00", "2:00:00", ExpectedResult = 60)]
        [Test]
        public int TestCalcFamB(string start, string end)
        {
            var scheduler = new Scheduler(mockContext.Object);
            int TotalCost = 0;
            Appointments newAppointment = new Appointments();
            newAppointment.FamilyId = "B";
            newAppointment.StartDate = Convert.ToDateTime("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse(start);
            newAppointment.EndTime = TimeSpan.Parse(end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            TotalCost = scheduler.CalculateFamilyB(newAppointment);
            return TotalCost;
        }

        [TestCase ("05:00:00", "12:00:00", ExpectedResult = 129)]
        [TestCase("07:00:00", "2:00:00", ExpectedResult = 117)]
        [TestCase("06:00:00", "04:00:00", ExpectedResult = 168)]
        [TestCase("05:00:00", "01:00:00", ExpectedResult = 144)]
        [TestCase("06:00:00", "01:00:00", ExpectedResult = 123)]
        [Test]
        public int TestCalcFamC(string start, string end) {
            var scheduler = new Scheduler(mockContext.Object);
            int TotalCost = 0;
            Appointments newAppointment = new Appointments();
            newAppointment.FamilyId = "C";
            newAppointment.StartDate = Convert.ToDateTime("2019-10-4");
            newAppointment.StartTime = TimeSpan.Parse(start);
            newAppointment.EndTime = TimeSpan.Parse(end);
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            TotalCost = scheduler.CalculateFamilyC(newAppointment);
            return TotalCost;
        }
    }
}
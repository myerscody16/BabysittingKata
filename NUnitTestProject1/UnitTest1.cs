using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class CalculationTests
    {
        private readonly BabysittingDbContext _context;
        private readonly IConfiguration _configuration;

        public CalculationTests(BabysittingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [TestCase("05:00:00", "12:00:00", ExpectedResult = 110)]
        [TestCase("07:00:00", "2:00:00", ExpectedResult = 120)]
        [TestCase("06:00:00", "04:00:00", ExpectedResult = 175)]
        public int TestCalcFamA(TimeSpan start, TimeSpan end)
        {
            int TotalCost = 0;
            Appointments newAppointment = new Appointments();
            newAppointment.FamilyId = "A";
            newAppointment.StartDate = Convert.ToDateTime("2019-10-4");
            newAppointment.StartTime = start;
            newAppointment.EndTime = end;
            newAppointment.TotalCost = 0;
            newAppointment.Id = 7;
            TotalCost = HomeController.CalculateFamilyA(newAppointment);
            return TotalCost;
        }
        [TestCase]
        public void TestCalcFamB(Appointments newAppointment)
        {
            newAppointment.StartTime = TimeSpan.Parse("05:00:00");
            newAppointment.EndTime = TimeSpan.Parse("04:00:00");
            double TotalCost = 0;
            List<FamilyPayRates> familyBPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "B").ToList();
            foreach (var timeframe in familyBPayRates)
            {
                if (newAppointment.StartTime >= timeframe.StartTime && newAppointment.StartTime < timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                else if (newAppointment.StartTime < timeframe.StartTime && newAppointment.EndTime > timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                else if (newAppointment.StartTime < timeframe.StartTime && newAppointment.EndTime <= timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                else if (newAppointment.StartTime > timeframe.StartTime && newAppointment.EndTime <= timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
            }
            Assert.That(TotalCost == 140);
        }
        [TestCase]
        public void TestCalcFamC(Appointments newAppointment)
        {
            newAppointment.StartTime = TimeSpan.Parse("07:00:00");
            newAppointment.EndTime = TimeSpan.Parse("01:00:00");
            double TotalCost = 0;
            List<FamilyPayRates> familyCPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "C").ToList();
            foreach (var timeframe in familyCPayRates)
            {
                if (newAppointment.StartTime >= timeframe.StartTime && newAppointment.StartTime < timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                else if (newAppointment.StartTime < timeframe.StartTime)
                {
                    TimeSpan timeSpan = newAppointment.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
            }
            Assert.That(TotalCost == 102);
        }
    }
}
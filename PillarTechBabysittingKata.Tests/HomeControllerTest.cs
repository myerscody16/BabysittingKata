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
using PillarTechBabysittingKata.Controllers;
using PillarTechBabysittingKata.Models;
using PillarTechBabysittingKata.Services;

namespace PillarTechBabysittingKata.Tests
{
    public class HomeControllerTest
    {
        Mock<IScheduler> schedulerMock;
        [SetUp]
        public void Setup()
        {
            schedulerMock = new Mock<IScheduler>();

            var appointments = new List<Appointments>() {
                new Appointments() { Id = 1, FamilyId = "A", StartDate = Convert.ToDateTime("2019-10-4"), StartTime = TimeSpan.Parse("05:00:00"), EndTime = TimeSpan.Parse("12:00:00"), TotalCost = 0 },
                new Appointments() { Id = 1, FamilyId = "C", StartDate = Convert.ToDateTime("2019-10-4"), StartTime = TimeSpan.Parse("05:00:00"), EndTime = TimeSpan.Parse("12:00:00"), TotalCost = 0 }
            };
            schedulerMock.Setup(x => x.GetAll()).Returns(appointments);
        }
        //Test the Action method returning the 2 objects
        [Test]
        public void TestIndex()
        {
            var obj = new HomeController(schedulerMock.Object, null);

            var actResult = obj.Index() as ViewResult;
            var models = actResult.Model as List<Appointments>;
            Assert.AreEqual(models.Count, 0);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PillarTechBabysittingKata.Controllers;
using PillarTechBabysittingKata.Models;
using Microsoft.EntityFrameworkCore;

namespace PillarTechBabysittingKata.Controllers
{
    public class HomeController : Controller
    {
        private BabysittingDbContext _context;
        private readonly IConfiguration _configuration;


        public HomeController(BabysittingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Appointments> allAppointments = _context.Appointments.ToList();
            return View(allAppointments);
        }
        [HttpGet]
        public IActionResult CreateAppointment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateAppointment(Appointments newAppointment)
        {
            return RedirectToAction("ConfirmPage", newAppointment);
        }
        public IActionResult ConfirmPage(Appointments newAppointment)
        {
            //Calculate total here
            if (newAppointment.FamilyId == "A")
            {
                newAppointment.TotalCost = CalculateFamilyA(newAppointment);
            }
            else if (newAppointment.FamilyId == "B")
            {
                newAppointment.TotalCost = CalculateFamilyB(newAppointment);
            }
            else if (newAppointment.FamilyId == "C")
            {
                newAppointment.TotalCost = CalculateFamilyC(newAppointment);
            }
            return View(newAppointment);
        }
        public IActionResult AddAppointment(Appointments newAppointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newAppointment);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult ListOfUpcomingAppointments()
        {
            List<Appointments> allAppointments = _context.Appointments.ToList();
            List<Appointments> orderedAppointments = new List<Appointments> { };
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "A" && appointment.StartDate > DateTime.Now)
                {
                    orderedAppointments.Add(appointment);
                }
            }
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "B" && appointment.StartDate > DateTime.Now)
                {
                    orderedAppointments.Add(appointment);
                }
            }
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "C" && appointment.StartDate > DateTime.Now)
                {
                    orderedAppointments.Add(appointment);
                }
            }
            return View(orderedAppointments);
        }
        public IActionResult UpdateAppointment(Appointments newAppointment)
        {
            return View(newAppointment);
        }
        public int CalculateFamilyA(Appointments newAppointment)
        {
            List<FamilyPayRates> familyAPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "A").ToList();
            int TotalCost = 0;
            foreach (var timeframe in familyAPayRates)
            {
                if (newAppointment.StartTime >= timeframe.StartTime && newAppointment.StartTime < timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                if (newAppointment.StartTime < timeframe.StartTime)
                {
                    TimeSpan timeSpan = TimeSpan.Parse("00:00:00");
                    if(newAppointment.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment.EndTime > TimeSpan.Parse("00:00:00"))
                    {
                        timeSpan = newAppointment.EndTime.Add(TimeSpan.Parse("01:00:00"));
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                    }
                }
            }
            return TotalCost;
        }
        public int CalculateFamilyB(Appointments newAppointment)
        {
            List<FamilyPayRates> familyBPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "B").ToList();
            int TotalCost = 0;
            foreach (var timeframe in familyBPayRates)
            {
                if (newAppointment.StartTime >= timeframe.StartTime && newAppointment.StartTime <= timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                if (newAppointment.StartTime <= timeframe.StartTime && newAppointment.EndTime <= timeframe.EndTime)//logic in if statement is causing the third payrate if statement to be used causing a negative timespan
                {
                    TimeSpan timeSpan = newAppointment.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                //if (newAppointment.StartTime <= timeframe.StartTime && newAppointment.EndTime >= timeframe.EndTime && newAppointment.StartTime <= TimeSpan.Parse("12:00:00"))
                //{
                //    TimeSpan timeSpan = timeframe.EndTime.Subtract(timeframe.StartTime);
                //    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                //}
                if(newAppointment.StartTime >= timeframe.StartTime && newAppointment.EndTime >= TimeSpan.Parse("00:00:00") && newAppointment.EndTime <= TimeSpan.Parse("04:00:00"))
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
            }
            return TotalCost;
        }
        public  int CalculateFamilyC(Appointments newAppointment)
        {
            List<FamilyPayRates> familyCPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "C").ToList();
            int TotalCost = 0;
            foreach (var timeframe in familyCPayRates)
            {
                if (newAppointment.StartTime >= timeframe.StartTime && newAppointment.StartTime < timeframe.EndTime)
                {
                    TimeSpan timeSpan = timeframe.EndTime.Subtract(newAppointment.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
                if (newAppointment.StartTime < timeframe.StartTime)
                {
                    TimeSpan timeSpan = TimeSpan.Parse("00:00:00");
                    if (newAppointment.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment.EndTime > TimeSpan.Parse("00:00:00"))
                    {
                        timeSpan = newAppointment.EndTime.Add(TimeSpan.Parse("03:00:00"));
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                    }
                }
            }
            return TotalCost;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



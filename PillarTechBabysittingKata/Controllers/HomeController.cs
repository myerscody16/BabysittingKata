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
        public List<FamilyPayRates> familyAPayRates;
        public List<FamilyPayRates> familyBPayRates;
        public List<FamilyPayRates> familyCPayRates;

        public HomeController(BabysittingDbContext context, IConfiguration configuration, List<FamilyPayRates> familyAPayRates, List<FamilyPayRates> familyBPayRates, List<FamilyPayRates> familyCPayRates)
        {
            _context = context;
            _configuration = configuration;
            familyAPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "A").ToList();
            familyBPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "B").ToList();
            familyCPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "C").ToList();
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
        public int CalculateFamilyA(Appointments newAppointment)//needs to be tested and logic checked
        {
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
                    TimeSpan timeSpan = newAppointment.EndTime.Subtract(timeframe.StartTime);
                    TotalCost += Convert.ToInt32(timeSpan.TotalHours) * timeframe.PayRate;
                }
            }
            return TotalCost;


        }
        public int CalculateFamilyB(Appointments newAppointment)//needs logic checked
        {
            int TotalCost = 0;
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
            return TotalCost;
        }
        public int CalculateFamilyC(Appointments newAppointment)//needs to be tested and logic checked
        {
            int TotalCost = 0;
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
            return TotalCost;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



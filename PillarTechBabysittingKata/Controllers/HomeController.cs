using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PillarTechBabysittingKata.Controllers;
using PillarTechBabysittingKata.Models;
using PillarTechBabysittingKata.Services;

namespace PillarTechBabysittingKata.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private IScheduler _scheduler;

        public HomeController(IScheduler scheduler, IConfiguration configuration)
        {
            _scheduler = scheduler;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Appointments> allAppointments = _scheduler.GetAll();
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

        [HttpGet]
        public IActionResult CreateAppointment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAppointment(Appointments newAppointment)
        {
            List<Appointments> allAppointments = _scheduler.GetAll();
            Appointments newAppointment1 = newAppointment.ShallowCopy();
            #region Form validation
            if (newAppointment1.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment1.StartTime < TimeSpan.Parse("12:00:00"))
            {
                newAppointment1.EndTime = TimeSpan.Parse("12:00:00");
            }
            if(newAppointment1.StartTime == TimeSpan.Parse("12:00:00"))
            {
                newAppointment1.StartTime = TimeSpan.Parse("00:00:00");
            }
            foreach(var appointment in allAppointments)
            {
                if(newAppointment1.StartDate == appointment.StartDate)
                {
                    ViewBag["message"] = "This date has already been taken";
                    return RedirectToAction("CreateAppointment");
                }
            }
            if(newAppointment1.StartTime >= newAppointment1.EndTime)
            {
                ViewBag["message"] = "This time is invalid";
                return RedirectToAction("CreateAppointment");
            }
            #endregion
            return RedirectToAction("ConfirmPage", newAppointment);
        }
        public IActionResult ConfirmPage(Appointments newAppointment)
        {
            //Calculate total here
            if (newAppointment.FamilyId == "A")
            {
                newAppointment.TotalCost = _scheduler.CalculateFamilyA(newAppointment);
            }
            else if (newAppointment.FamilyId == "B")
            {
                newAppointment.TotalCost = _scheduler.CalculateFamilyB(newAppointment);
            }
            else if (newAppointment.FamilyId == "C")
            {
                newAppointment.TotalCost = _scheduler.CalculateFamilyC(newAppointment);
            }
            return View(newAppointment);
        }
        public IActionResult AddAppointment(Appointments newAppointment)
        {
            if (ModelState.IsValid)
            {
                _scheduler.Add(newAppointment);
            }
            return RedirectToAction("Index");
        }
        public IActionResult ListOfUpcomingAppointments()
        {
            List<Appointments> allAppointments = _scheduler.GetAll();
            List<Appointments> orderedAppointments = new List<Appointments> { };
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "A" && appointment.StartDate > DateTime.Now) {
                    orderedAppointments.Add(appointment);
                }
            }
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "B" && appointment.StartDate > DateTime.Now) {
                    orderedAppointments.Add(appointment);
                }
            }
            foreach (Appointments appointment in allAppointments)
            {
                if (appointment.FamilyId == "C" && appointment.StartDate > DateTime.Now) {
                    orderedAppointments.Add(appointment);
                }
            }
            return View(orderedAppointments);
        }
        public IActionResult UpdateAppointment(Appointments newAppointment)
        {
            return View(newAppointment);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
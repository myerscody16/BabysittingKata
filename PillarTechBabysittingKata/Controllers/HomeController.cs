using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PillarTechBabysittingKata.Models;

namespace PillarTechBabysittingKata.Controllers
{
    public class HomeController : Controller
    {
        private readonly BabysittingDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController (BabysittingDbContext context, IConfiguration configuration)
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
            if(newAppointment.FamilyId == "A")
            {
                newAppointment.TotalCost = CalculateFamilyA();
            }
            else if (newAppointment.FamilyId == "B")
            {
                newAppointment.TotalCost = CalculateFamilyB();
            }
            else if (newAppointment.FamilyId == "C")
            {
                newAppointment.TotalCost = CalculateFamilyC();
            }
            return View(newAppointment);
        }
        public IActionResult AddAppointment(Appointments newAppointment)
        {
            if(ModelState.IsValid)
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
            foreach(Appointments appointment in allAppointments)
            {
                if(appointment.FamilyId == "A" && appointment.StartDate > DateTime.Now)
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
        public static int CalculateFamilyA()
        {
            int TotalCost = 0;
            return TotalCost;
        }
        public static int CalculateFamilyB()
        {
            int TotalCost = 0;
            return TotalCost;
        }
        public static int CalculateFamilyC()
        {
            int TotalCost = 0;
            return TotalCost;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

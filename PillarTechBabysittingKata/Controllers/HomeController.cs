using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PillarTechBabysittingKata.Models;

namespace PillarTechBabysittingKata.Controllers
{
    public class HomeController : Controller
    {
        private readonly BabysittingDbContext _context;

        public HomeController (BabysittingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateAppointment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateAppointment(Appointments newAppointment)
        {
            //List<Appointments> appointments = _context.Appointments.ToList();
            //_context.Add(newAppointment);
            //_context.SaveChanges();
            return RedirectToAction("ConfirmPage", newAppointment);
        }
        public IActionResult ConfirmPage(Appointments newAppointment)
        {
            return View(newAppointment);
        }
        public IActionResult AddAppointment(Appointments newAppointment)
        {
            List<Appointments> appointments = _context.Appointments.ToList();
            _context.Add(newAppointment);
            _context.SaveChanges();
            return RedirectToAction("ListOfAppointments");
        }
        public IActionResult ListOfAppointments()
        {
            List<Appointments> allAppointments = _context.Appointments.ToList();
            return View();
        }
        //public IActionResult ListFamilyAppointment(Appointments newAppointment)
        //{
        //    List<Appointments> allAppointments = _context.Appointments.Where(u => u.FamilyId == newAppointment.FamilyId).ToList();//write test to make sure this works
        //    return View(allAppointments);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

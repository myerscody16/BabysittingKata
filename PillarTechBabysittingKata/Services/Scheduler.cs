using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PillarTechBabysittingKata.Models;

namespace PillarTechBabysittingKata.Services
{
    public interface IScheduler
    {
        void Add(Appointments newAppointment);
        int CalculateFamilyA(Appointments newAppointment);
        int CalculateFamilyB(Appointments newAppointment);
        int CalculateFamilyC(Appointments newAppointment);
        List<Appointments> GetAll();
    }

    public class Scheduler : IScheduler
    {
        private BabysittingDbContext _context;

        public Scheduler(BabysittingDbContext context)
        {
            _context = context;
        }

        public List<Appointments> GetAll()
        {
            return _context.Appointments.ToList();
        }

        public void Add(Appointments newAppointment)
        {
            _context.Add(newAppointment);
            _context.SaveChanges();
        }

        public int CalculateFamilyA(Appointments newAppointment)
        {
            List<FamilyPayRates> familyAPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "A").ToList();
            int TotalCost = 0;

            if (newAppointment.StartTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment starts at midnight
            {
                newAppointment.StartTime = TimeSpan.Parse("00:00:00");
            }
            if (newAppointment.EndTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment ends at midnight
            {
                newAppointment.EndTime = TimeSpan.Parse("00:00:00");
            }

            #region Start Time at midnight or later
            //should turn this into its own method.
            if (newAppointment.StartTime >= TimeSpan.Parse("00:00:00") && newAppointment.StartTime <= TimeSpan.Parse("03:00:00"))//handles start times after 12AM and before 3AM
            {
                //morningAppt might just be redundant because this block of code is only used if the start time is at midnight or later.
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                if (morningAppt.StartTime < TimeSpan.Parse("00:00:00"))//if an appointments end time is after midnight, this will override the end time to end it at midnight and still use it in calculations.
                {
                    morningAppt.StartTime = newAppointment.StartTime;
                }
                //calculate total cost
                foreach (var time in familyAPayRates)//we only need to handle from 12am to 4am in this loop
                {
                    if (time.StartTime == TimeSpan.Parse("11:00:00"))//will need to account for the hour between 11pm and midnight in another block of code
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
                return TotalCost;//might not need this since total cost exists in the scope of the entire method
            }
            #endregion
            #region Start time before midnight with end time after midnight
            if (newAppointment.EndTime >= TimeSpan.Parse("00:00:00") && newAppointment.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00"))//handles end times after midnight with start times before midnight
            {
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                Appointments nightAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = newAppointment.StartTime, StartDate = newAppointment.StartDate, EndTime = TimeSpan.Parse("12:00:00") };

                //calculate total cost for night appt
                foreach (var time in familyAPayRates)// four set ups, when both are in range, when start is in range, and when end is in range, when neither are in range
                {
                    if (time.StartTime == TimeSpan.Parse("11:00:00"))
                    {
                        time.EndTime = TimeSpan.Parse("12:00:00");//This makes it so that the end time of the payrate is midnight
                    }
                    if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime > time.EndTime)//handles any start time in the timeframe with an endtime after the end of the pay rate's timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime <= time.EndTime)//handles when the start is before the timeframes start time and when the end time is before the end of the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime <= time.EndTime)// handles when both start and end times are in the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime > time.EndTime)//handles if both start and end are outside of the timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
                //calculate total cost for morning appt with its own foreach loop, probably can steal from the start time midnight or after region
                foreach (var time in familyAPayRates)
                {
                    if (time.StartTime == TimeSpan.Parse("11:00:00"))
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
            }
            #endregion
            #region Start time and end time before midnight
            if (newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00") && newAppointment.EndTime >= TimeSpan.Parse("06:00:00") && newAppointment.EndTime <= TimeSpan.Parse("12:00:00"))
            {
                foreach (var time in familyAPayRates)
                {
                    if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime >= time.EndTime)
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime <= time.EndTime)
                    {
                        TimeSpan timeSpan = newAppointment.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
            }

            #endregion

            return TotalCost;
        }
        public int CalculateFamilyB(Appointments newAppointment)
        {
            List<FamilyPayRates> familyBPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "B").ToList();
            int TotalCost = 0;
            if (newAppointment.StartTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment starts at midnight
            {
                newAppointment.StartTime = TimeSpan.Parse("00:00:00");
            }
            if (newAppointment.EndTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment ends at midnight
            {
                newAppointment.EndTime = TimeSpan.Parse("00:00:00");
            }
            #region Start Time at midnight or later
            //should turn this into its own method.
            if (newAppointment.StartTime >= TimeSpan.Parse("00:00:00") && newAppointment.StartTime <= TimeSpan.Parse("03:00:00"))//handles start times after 12AM and before 3AM
            {
                //morningAppt might just be redundant because this block of code is only used if the start time is at midnight or later.
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                if (morningAppt.StartTime < TimeSpan.Parse("00:00:00"))//if an appointments end time is after midnight, this will override the end time to end it at midnight and still use it in calculations.
                {
                    morningAppt.StartTime = newAppointment.StartTime;
                }
                //calculate total cost
                foreach (var time in familyBPayRates)//we only need to handle from 12am to 4am in this loop
                {
                    if (time.StartTime == TimeSpan.Parse("12:00:00"))
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
                return TotalCost;//might not need this since total cost exists in the scope of the entire method
            }
            #endregion
            #region Start time before midnight with end time after midnight
            if (newAppointment.EndTime >= TimeSpan.Parse("00:00:00") && newAppointment.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00"))//handles end times after midnight with start times before midnight
            {
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                Appointments nightAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = newAppointment.StartTime, StartDate = newAppointment.StartDate, EndTime = TimeSpan.Parse("12:00:00") };

                //calculate total cost for night appt
                foreach (var time in familyBPayRates)// four set ups, when both are in range, when start is in range, and when end is in range, when neither are in range
                {
                    if (time.StartTime == TimeSpan.Parse("12:00:00"))
                    {
                        time.EndTime = TimeSpan.Parse("12:00:00");//This makes it so that the end time of the payrate is midnight
                    }
                    if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime > time.EndTime)//handles any start time in the timeframe with an endtime after the end of the pay rate's timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime <= time.EndTime)//handles when the start is before the timeframes start time and when the end time is before the end of the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime <= time.EndTime)// handles when both start and end times are in the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime > time.EndTime)//handles if both start and end are outside of the timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
                //calculate total cost for morning appt with its own foreach loop, probably can steal from the start time midnight or after region
                foreach (var time in familyBPayRates)
                {
                    if (time.StartTime == TimeSpan.Parse("12:00:00"))
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
            }
            #endregion
            #region Start time and end time before midnight
            if (newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00") && newAppointment.EndTime >= TimeSpan.Parse("06:00:00") && newAppointment.EndTime <= TimeSpan.Parse("12:00:00"))
            {
                foreach (var time in familyBPayRates)
                {
                    if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime >= time.EndTime)
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime <= time.EndTime)
                    {
                        TimeSpan timeSpan = newAppointment.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
            }
            #endregion
            return TotalCost;
        }
        public int CalculateFamilyC(Appointments newAppointment)
        {
            List<FamilyPayRates> familyCPayRates = _context.FamilyPayRates.Where(u => u.FamilyLetter == "C").ToList();
            int TotalCost = 0;

            if (newAppointment.StartTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment starts at midnight
            {
                newAppointment.StartTime = TimeSpan.Parse("00:00:00");
            }
            if (newAppointment.EndTime == TimeSpan.Parse("12:00:00"))//this handles a situation where the appointment ends at midnight
            {
                newAppointment.EndTime = TimeSpan.Parse("00:00:00");
            }

            #region Start Time at midnight or later
            //should turn this into its own method.
            if (newAppointment.StartTime >= TimeSpan.Parse("00:00:00") && newAppointment.StartTime <= TimeSpan.Parse("03:00:00"))//handles start times after 12AM and before 3AM
            {
                //morningAppt might just be redundant because this block of code is only used if the start time is at midnight or later.
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                if (morningAppt.StartTime < TimeSpan.Parse("00:00:00"))//if an appointments end time is after midnight, this will override the end time to end it at midnight and still use it in calculations.
                {
                    morningAppt.StartTime = newAppointment.StartTime;
                }
                //calculate total cost
                foreach (var time in familyCPayRates)//we only need to handle from 12am to 4am in this loop
                {
                    if (time.StartTime == TimeSpan.Parse("09:00:00"))//will need to account for the hour between 11pm and midnight in another block of code
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
                return TotalCost;//might not need this since total cost exists in the scope of the entire method
            }
            #endregion
            #region Start time before midnight with end time after midnight
            if (newAppointment.EndTime >= TimeSpan.Parse("00:00:00") && newAppointment.EndTime <= TimeSpan.Parse("04:00:00") && newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00"))//handles end times after midnight with start times before midnight
            {
                Appointments morningAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = TimeSpan.Parse("00:00:00"), StartDate = newAppointment.StartDate.AddDays(1), EndTime = newAppointment.EndTime };
                Appointments nightAppt = new Appointments { Id = newAppointment.Id, FamilyId = newAppointment.FamilyId, StartTime = newAppointment.StartTime, StartDate = newAppointment.StartDate, EndTime = TimeSpan.Parse("12:00:00") };

                //calculate total cost for night appt
                foreach (var time in familyCPayRates)// four set ups, when both are in range, when start is in range, and when end is in range, when neither are in range
                {
                    if (time.StartTime == TimeSpan.Parse("09:00:00"))
                    {
                        time.EndTime = TimeSpan.Parse("12:00:00");//This makes it so that the end time of the payrate is midnight
                    }
                    if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime > time.EndTime)//handles any start time in the timeframe with an endtime after the end of the pay rate's timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime <= time.EndTime)//handles when the start is before the timeframes start time and when the end time is before the end of the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime >= time.StartTime && nightAppt.EndTime <= time.EndTime)// handles when both start and end times are in the timeframe
                    {
                        TimeSpan timeSpan = nightAppt.EndTime.Subtract(nightAppt.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (nightAppt.StartTime < time.StartTime && nightAppt.EndTime > time.EndTime)//handles if both start and end are outside of the timeframe
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(time.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
                //calculate total cost for morning appt with its own foreach loop, probably can steal from the start time midnight or after region
                foreach (var time in familyCPayRates)
                {
                    if (time.StartTime == TimeSpan.Parse("09:00:00"))
                    {
                        time.StartTime = TimeSpan.Parse("00:00:00");
                        if (morningAppt.StartTime >= time.StartTime && morningAppt.EndTime <= time.EndTime)//covers the case of a start time between 12am and the end time of the pay rate if the end time is outside of that
                        {
                            TimeSpan timeSpan = morningAppt.EndTime.Subtract(morningAppt.StartTime);
                            TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                        }
                    }
                }
            }
            #endregion
            #region Start time and end time before midnight
            if (newAppointment.StartTime < TimeSpan.Parse("12:00:00") && newAppointment.StartTime >= TimeSpan.Parse("05:00:00") && newAppointment.EndTime >= TimeSpan.Parse("06:00:00") && newAppointment.EndTime <= TimeSpan.Parse("12:00:00"))
            {
                foreach (var time in familyCPayRates)
                {
                    if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime >= time.EndTime)
                    {
                        TimeSpan timeSpan = time.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                    else if (newAppointment.StartTime >= time.StartTime && newAppointment.EndTime <= time.EndTime)
                    {
                        TimeSpan timeSpan = newAppointment.EndTime.Subtract(newAppointment.StartTime);
                        TotalCost += Convert.ToInt32(timeSpan.TotalHours) * time.PayRate;
                    }
                }
            }

            #endregion

            return TotalCost;
        }
    }
}
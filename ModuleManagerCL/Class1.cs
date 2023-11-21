using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ModuleManagerCL
{
    
    public class ModuleManager
    {
        
        //Calculates and returns the self study hours by dividing the credits and the weeks in the semester and subtracting the weekly class hours
        public decimal calculateSelfStudyHours(int credits, int weeksInSem, decimal weeklyClassHours)
        {
            int selfStudyHours = (int)Math.Round((decimal)credits * 10 / weeksInSem - weeklyClassHours);

            return selfStudyHours;
        }

        public bool CheckDifferentWeek(DateTime date1, DateTime date2)
        {
            // Gets information about the date format
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            // Gets the calendar for the current date format
            System.Globalization.Calendar cal = dfi.Calendar;

            // Calculates the week numbers for both input dates based on the calendar
            int week1 = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int week2 = cal.GetWeekOfYear(date2, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            // Checks if the weeks of the two dates are different and returns true or false
            return week1 != week2;
        }

        public DateTime GetStartOfWeek(DateTime date)
        {
            // Assuming Sunday is the start of the week
            int difference = (int)date.DayOfWeek - (int)DayOfWeek.Sunday;
            DateTime startOfWeek = date.AddDays(-difference).Date;
            return startOfWeek;
        }

        public bool CheckSameWeek(DateTime date1, DateTime date2)
        {
            // Check if the two dates fall within the same week
            return GetStartOfWeek(date1) == GetStartOfWeek(date2);
        }
    }

   
}
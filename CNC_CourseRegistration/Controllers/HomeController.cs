using CNC_CourseRegistration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CNC_CourseRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Home/Feedback
        [HttpPost]
        public string Feedback([FromBody]StudentModel student)
        {

            // generate a random 4-digit number for the student
            var randNo = GenerateRandomNo();

            // concatenate student name and random number together and all in lowercase
            string username = student.LastName.ToLower() + student.FirstName.Substring(0,1).ToLower() + randNo;

            // find the appropriate first date and time for the student
            string firstClassDateTime = FindFirstClassDatetime(student.ClassTime, student.PreferredStartDate);

            // create a FeedbackModel object
            var feedback = new FeedbackModel
            {
                Username = username,
                FirstClassDatetime = firstClassDateTime
            };

            // log the information for analysis
            var message = $"The student: { student.FirstName + " " + student.LastName } who has selected { student.ClassTime } and preferred start date { student.PreferredStartDate } has registered the class that starts from {feedback.FirstClassDatetime} with username: {feedback.Username}";
            _logger.LogInformation(message);

            // return the json string of feedback
            return JsonConvert.SerializeObject(feedback);
        }

        /// <summary>
        /// check whether the date is a holiday based on the 2021 - 2022 academic schedule from CNC
        /// </summary>
        /// <param name="date">the date to verify</param>
        /// <returns>true if it is a holiday otherwise false</returns>
        public static bool IsHoliday(DateTime date)
        {
            // it is supposed to be loaded holidays from database or a file. In this case, I just hardcoded
            DateTime[] holidays = new DateTime[]
            {
                new DateTime(2021, 8, 2),
                new DateTime(2021, 9, 6),
                new DateTime(2021, 10, 11),
                new DateTime(2021, 12, 25),
                new DateTime(2021, 12, 26),
                new DateTime(2022, 01, 01),
                new DateTime(2022, 02, 21),
                new DateTime(2022, 04, 15),
                new DateTime(2022, 04, 18),
                new DateTime(2022, 05, 23),
                new DateTime(2022, 07, 01)
            };

            return holidays.Contains(date);
        } 


        /// <summary>
        /// Returns a random 4-digit number as a string
        /// </summary>
        /// <returns>a string of 4 digit number</returns>
        public string GenerateRandomNo()
        {
            int max = 10000;
            Random rand = new Random();
            return rand.Next(0, max).ToString("D4");
        }

        /// <summary>
        /// This method finds the first date and time at least 10 days after the preferred start date, which
        /// also meets the selected preferred class time.
        /// </summary>
        /// <param name="classTime">Preferred class time selected by user</param>
        /// <param name="preferredStartDate">Preferred start time picked by the user</param>
        /// <returns>return a string of date and time for the first class</returns>
        public string FindFirstClassDatetime(string classTime, DateTime preferredStartDate)
        {
            DateTime firstClassDate;
            TimeSpan firstClassTime = new TimeSpan();

            // split the class time variable
            string[] strArray = classTime.Split("-");
            var dayOfWeek = strArray[0];
            var timeOfDay = strArray[1];


            // determine the time of day for the first class
            switch (timeOfDay)
            {
                case "morning":
                    firstClassTime = new TimeSpan(8, 0, 0);
                    break;
                case "afternoon":
                    firstClassTime = new TimeSpan(13, 30, 0);
                    break;
                case "evening":
                    firstClassTime = new TimeSpan(19, 0, 0);
                    break;
                default:
                    // log the error here
                    _logger.LogError("The selected class time is not handled in the FindFirstClassDatetime method of HomeController.");
                    break;
            }

            /*
             * The first date will be at least 10 days after the preferred class time and must meet the selected class time
             */
            int daysToAdd = 10;
            while (true)
            {
                // increment the preferred start date by counters days, initially is 10 days
                firstClassDate = preferredStartDate.AddDays(daysToAdd);


                if (dayOfWeek == "weekday")
                {
                    // when the selected day of week is weekday
                    if (firstClassDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        // move the start date to next monday
                        daysToAdd += 2;
                    }
                    else if (firstClassDate.DayOfWeek == DayOfWeek.Sunday || IsHoliday(firstClassDate))
                    {
                        // increment one day to move to the next day
                        daysToAdd += 1;
                    }
                    else
                    {
                        // the date of the first class meets the selected weekday so jump out of the loop
                        break;
                    }
                }
                else  // when the selected day of week is weekend
                {
                    if (firstClassDate.DayOfWeek != DayOfWeek.Saturday 
                        && firstClassDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        // if it is not on the weekend, move to next Saturday
                        daysToAdd += (int)DayOfWeek.Saturday - (int)firstClassDate.DayOfWeek;
                    }
                    else if (IsHoliday(firstClassDate))
                    {
                        // if it is on the weekend but the date is a holiday, increment the daysToAdd by one
                        daysToAdd += 1;
                    }
                    else
                    {
                        // the date is on the weekend and it is not a holiday so jump out of the loop
                        break;
                    }
                }

            } // end of the while loop

            // concatenate time and date together and format it as "Monday, June 15, 2009 1:45 PM"
            string firstClassDateTime = (firstClassDate + firstClassTime).ToString("f");

            return firstClassDateTime;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

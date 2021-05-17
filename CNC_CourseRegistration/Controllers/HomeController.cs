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
        //[ValidateAntiForgeryToken]
        public string Feedback(string studentJsonStr)
        {
            // parse the json string to StudentModel class
            var student = JsonConvert.DeserializeObject<StudentModel>(studentJsonStr);

            // generate a random 4-digit number for the student
            var randNo = GenerateRandomNo();

            // concatenate student name and random number together and all in lowercase
            string username = student.LastName.ToLower() + student.FirstName.Substring(0,1).ToLower() + randNo.ToString();

            // find the appropriate first date and time for the student
            string firstClassDateTime = FindFirstClassDatetime(student.ClassTime, student.PreferredStartDate);

            // create a FeedbackModel object
            var feedback = new FeedbackModel
            {
                Username = username,
                FirstClassDatetime = firstClassDateTime
            };

            return JsonConvert.SerializeObject(feedback);
        }


        //public IActionResult Feedback()
        //{
        //    var obj = JsonConvert.DeserializeObject<FeedbackModel>((string)TempData["Feedback"]);
            
        //    return View(feedback);
        //}

        /// <summary>
        /// Returns a random 4-digit number as a string
        /// </summary>
        /// <returns>string of 4 digit number</returns>
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
                    timeOfDay = "Unknown time";
                    break;
            }

            /*
             * The first date will be at least 10 days after the preferred class time and must meet the selected class time
             */
            int counter = 10;
            while (true)
            {
                // increment the preferred start date by counters days, initially is 10 days
                firstClassDate = preferredStartDate.AddDays(counter);


                if (dayOfWeek == "weekday")
                {
                    // when the selected day of week is weekday
                    if (firstClassDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        // move the start date to next monday
                        counter += 2;
                    }
                    else if (firstClassDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // increment one day to next monday
                        counter += 1;
                    }
                    else
                    {
                        // the date of the first class meets the selected weekday so jump out of the loop
                        break;
                    }
                }
                else  // when the selected day of week is weekend
                {
                    if(firstClassDate.DayOfWeek != DayOfWeek.Saturday && firstClassDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        // increase the day by one if it is not the weekend
                        counter += 1;
                    }
                    else
                    {
                        // the date of the first class is on the weekend so jump out of the loop
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

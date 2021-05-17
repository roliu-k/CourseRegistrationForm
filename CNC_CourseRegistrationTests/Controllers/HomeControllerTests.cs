using Microsoft.VisualStudio.TestTools.UnitTesting;
using CNC_CourseRegistration.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using CNC_CourseRegistration.Models;

namespace CNC_CourseRegistration.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {

        [TestMethod()]
        public void FindFirstClassDatetimeWeekdayTest()
        {
            // Arrange
            StudentModel student = new StudentModel
            {
                FirstName = "Monica",
                LastName = "Walter",
                ClassTime = "weekday-morning",
                PreferredStartDate = new DateTime(2021, 05, 27),
            };

            string expectedUsername = "walterm";
            string expectedStartdate = "2021-06-07";

            // Act
            


            // Assert
            
        }
    }
}
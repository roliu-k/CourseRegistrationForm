using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CNC_CourseRegistration.Models
{
    public class FeedbackModel
    {
        public string Username { get; set; }

        [Display(Name ="Date and Time of First Class")]
        public string FirstClassDatetime { get; set; }
    }
}

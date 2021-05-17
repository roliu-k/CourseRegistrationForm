using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CNC_CourseRegistration.Models
{
    public class StudentModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Preferred Class Time")]
        public string ClassTime { get; set; }

        [Required]
        [Display(Name = "Preferred Start Date")]
        public DateTime PreferredStartDate { get; set; }
    }
}

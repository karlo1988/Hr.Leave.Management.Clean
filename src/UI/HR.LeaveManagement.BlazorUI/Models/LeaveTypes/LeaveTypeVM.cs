using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.BlazorUI.Models.LeaveTypes
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the default number of days")]
        [Display(Name = "Default Number of Days")]
        public int DefaultDays { get; set; }
    }
}
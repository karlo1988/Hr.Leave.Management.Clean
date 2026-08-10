using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.BlazorUI.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a username")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "The username must be at least 6 characters long")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

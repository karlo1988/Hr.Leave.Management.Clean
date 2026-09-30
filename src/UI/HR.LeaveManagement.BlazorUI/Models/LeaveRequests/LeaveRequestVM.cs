using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;

namespace HR.LeaveManagement.BlazorUI.Models.LeaveRequests
{
    public class LeaveRequestVM : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the start date")]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Please enter the end date")]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a leave type")]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        public LeaveTypeVM LeaveType { get; set; }

        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }

        [MaxLength(500, ErrorMessage = "Comments must not exceed 500 characters")]
        [Display(Name = "Comments")]
        public string RequestComments { get; set; } = string.Empty;

        public bool? Approved { get; set; }
        public bool Cancelled { get; set; }
        public string RequestingEmployeeId { get; set; } = string.Empty;

        public string Status => Cancelled ? "Cancelled"
            : Approved == true ? "Approved"
            : Approved == false ? "Rejected"
            : "Pending";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue && EndDate <= StartDate)
            {
                yield return new ValidationResult("End date must be after the start date",
                    new[] { nameof(EndDate) });
            }
        }
    }
}

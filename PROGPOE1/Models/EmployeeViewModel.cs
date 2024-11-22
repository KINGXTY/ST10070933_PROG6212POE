using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("E-mail")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hours Worked is required")]
        [DisplayName("Hours Worked")]
        [Range(0, double.MaxValue, ErrorMessage = "Hours Worked must be a positive number")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required")]
        [DisplayName("Hourly Rate")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly Rate must be a positive number")]
        public double HourlyRate { get; set; }

        [DisplayName("Salary")]
        public double Salary { get; set; }

        
        public IFormFile? FileUpload { get; set; }  

       
        public string? Status { get; set; }  

        [DisplayName("Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}


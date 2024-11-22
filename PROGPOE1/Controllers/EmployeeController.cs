using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.DAL;
using PROG6212POE.Models;
using PROG6212POE.Models.DBEntities;
using System.Security.Claims;

namespace PROG6212POE.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Lecturer")
            {
                return RedirectToAction("Create");
            }

            // HR and Academic Manager can view the index
            var employees = _context.Employees.ToList();
            List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();

            foreach (var employee in employees)
            {
                var EmployeeViewModel = new EmployeeViewModel()
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    DateOfBirth = employee.DateOfBirth,
                    Email = employee.Email,
                    HoursWorked = employee.HoursWorked,
                    HourlyRate = employee.HourlyRate,
                    Salary = employee.Salary,
                    Status = employee.Status,
                };
                employeeList.Add(EmployeeViewModel);
            }
            return View(employeeList);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                TempData["errorMessage"] = $"Employee with id {id} not found.";
                return RedirectToAction("Index");
            }

            var model = new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Email = employee.Email,
                HourlyRate = employee.HourlyRate,
                HoursWorked = employee.HoursWorked,
                Salary = employee.Salary,
                Status = employee.Status  // Include Status
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            try
            {
                // Remove Status and FileUpload from ModelState since they're not required for editing
                ModelState.Remove("Status");
                ModelState.Remove("FileUpload");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["errorMessage"] = "Validation failed: " + string.Join(", ", errors);
                    return View(model);
                }

                var employee = _context.Employees.Find(model.Id);
                if (employee == null)
                {
                    TempData["errorMessage"] = "Employee not found.";
                    return RedirectToAction("Index");
                }

                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.DateOfBirth = model.DateOfBirth;
                employee.Email = model.Email;
                employee.HourlyRate = model.HourlyRate;
                employee.HoursWorked = model.HoursWorked;
                employee.Salary = model.HoursWorked * model.HourlyRate;  // Recalculate salary

                // Don't update the Status - keep the existing one
                // employee.Status = model.Status;  

                _context.Update(employee);
                _context.SaveChanges();

                TempData["successMessage"] = "Employee details updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error updating employee: " + ex.Message;
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeData)
        {
            try
            {
                var employee = new Employee()
                {
                    FirstName = employeeData.FirstName ?? "Unknown",
                    LastName = employeeData.LastName ?? "Unknown",
                    DateOfBirth = employeeData.DateOfBirth == default ? DateTime.Now : employeeData.DateOfBirth,
                    Email = string.IsNullOrWhiteSpace(employeeData.Email) ? "unknown@example.com" : employeeData.Email,
                    HoursWorked = employeeData.HoursWorked <= 0 ? 0 : employeeData.HoursWorked,
                    HourlyRate = employeeData.HourlyRate <= 0 ? 0 : employeeData.HourlyRate,
                    Salary = (employeeData.HoursWorked > 0 && employeeData.HourlyRate > 0) ? employeeData.HoursWorked * employeeData.HourlyRate : 0,
                    Status = employeeData.Status ?? "Pending"
                };

                // File upload handling
                if (employeeData.FileUpload != null && employeeData.FileUpload.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeData.FileUpload.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await employeeData.FileUpload.CopyToAsync(fileStream);
                    }

                    // Store the file path in the Employee model
                    employee.FilePath = uniqueFileName;
                }

                _context.Employees.Add(employee);
                _context.SaveChanges();
                TempData["successMessage"] = "Employee created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(employeeData);
            }
        }

        [HttpPost]
        public IActionResult Accept(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                employee.Status = "Accepted";
                _context.SaveChanges();
                TempData["successMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been accepted.";
            }
            else
            {
                TempData["errorMessage"] = "Employee not found.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                employee.Status = "Rejected";
                _context.SaveChanges();
                TempData["successMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been rejected.";
            }
            else
            {
                TempData["errorMessage"] = "Employee not found.";
            }

            return RedirectToAction("Index");
        }

    }
}




using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.DAL;
using PROG6212POE.Models;
using System.Security.Claims;

namespace PROG6212POE.Controllers
{
    public class UserController : Controller
    {
        private readonly EmployeeDbContext _context;

        public UserController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If already logged in, redirect to Employee Index
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Employee");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    TempData["errorMessage"] = "Username already exists. Please choose a different username.";
                    return View(user);
                }

                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["successMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            TempData["errorMessage"] = "Invalid data. Please try again.";
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to Employee Index
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Employee");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["errorMessage"] = "Username and password cannot be empty.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    });

                TempData["successMessage"] = $"Welcome, {user.Username}!";
                return RedirectToAction("Index", "Employee");
            }

            TempData["errorMessage"] = "Invalid credentials. Please try again.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Web_prace.Data;
using Web_prace.Models;
using BCrypt.Net;

namespace Web_prace.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string jmeno, string heslo, string heslo2)
        {
            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(heslo))
            {
                ModelState.AddModelError("", "Jméno a heslo jsou povinné");
                return View();
            }

            if (heslo != heslo2)
            {
                ModelState.AddModelError("", "Hesla se neshodují");
                return View();
            }

            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Jmeno == jmeno);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Uživatel s tímto jménem již existuje");
                return View();
            }

            var hesloHash = BCrypt.Net.BCrypt.HashPassword(heslo);

            var newUser = new User
            {
                Jmeno = jmeno,
                HesloHash = hesloHash
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string jmeno, string heslo)
        {
            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(heslo))
            {
                ModelState.AddModelError("", "Jméno a heslo jsou povinné");
                return View();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Jmeno == jmeno);

            if (user == null)
            {
                ModelState.AddModelError("", "Uživatel nebo heslo je nesprávné");
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(heslo, user.HesloHash))
            {
                ModelState.AddModelError("", "Uživatel nebo heslo je nesprávné");
                return View();
            }

            HttpContext.Session.SetString("UserName", user.Jmeno);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login");
            }

            ViewData["Jmeno"] = userName;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}


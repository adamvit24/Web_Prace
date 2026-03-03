using Microsoft.AspNetCore.Mvc;

namespace Web_prace.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string jmeno, string heslo, string heslo2)
        {
            // Validace
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

            // Zde by byla logika uložení do databáze
            // Po registraci přesměruj na přihlášení
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string jmeno, string heslo)
        {
            // Validace
            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(heslo))
            {
                ModelState.AddModelError("", "Jméno a heslo jsou povinné");
                return View();
            }

            // Zde by byla logika ověření přihlášení
            // Přesměruj na Dashboard s jménem v ViewData
            ViewData["Jmeno"] = jmeno;
            return View("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}


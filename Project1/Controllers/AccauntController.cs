using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Project1.Service;
using Project1.Models.ViewsModel;
using Project1.Models.DBModels;
using System.Text.Json;
using System.Text;
namespace Project1.Controllers
{
    public class AccauntController : Controller
    {
        private readonly DBService bdService;
        public AccauntController(DBService _bdService)
        {
            bdService = _bdService;
        }

        [HttpGet]
        public IActionResult Registration() => View();

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!bdService.CheckRepetitionUser(model.Email))
            {
                ModelState.AddModelError("", "Пользователь с таким email уже существует.");
                return View(model);
            }

            UserModel user = new UserModel();
            user.password = model.Password;
            user.email = model.Email;
            user.name = model.Name;
            user.role = "Користувач";

            if (!bdService.AddUser(user))
            {
                ModelState.AddModelError("", "Виникла помилка при додаванні користувача.");
                return View(model);
            }

            await HttpContext.SignInAsync("Cookies", UserVerification.AuthorizationUser(user), UserVerification.AuthenProperties());
            return PersonalArea();
        }

        public IActionResult Authorization()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("PersonalArea", "Accaunt");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Authorization(AuthorizationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            UserModel user = bdService.GetUser(model.Email);
            if(user == null) {
				ModelState.AddModelError("", "Не вірна пошта!");
				return View(model);
			}
            if(user.password != model.Password)
            {
                ModelState.AddModelError("", "Не вірний пароль!");
                return View(model);
            }
            
            await HttpContext.SignInAsync("Cookies", UserVerification.AuthorizationUser(user), UserVerification.AuthenProperties());
            return PersonalArea();
        }
        public IActionResult PersonalArea()
        {
            if (User.Identity.IsAuthenticated)
            {
                UserModel user = new UserModel();
                user.name = User.FindFirstValue(ClaimTypes.Name);
                user.role = User.FindFirstValue(ClaimTypes.Role);
                return View("PersonalArea", user);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Clouse()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

    }
}

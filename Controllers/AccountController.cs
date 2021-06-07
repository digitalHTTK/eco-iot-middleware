using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plan_io_T.ViewModels;
using Plan_io_T.Models;
using Microsoft.AspNetCore.Identity;

namespace Plan_io_T.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RequestViewModel model) {
            if (ModelState.IsValid) {
                User user = new User {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    ReqText = model.ReqText,
                    Role = "none"
                };
                // Добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    // Установка куки (в работе не особо желательно, ибо роли)
                    //await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Success", "Home");
                } else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) {
                    // Проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    } else {
                        return RedirectToAction("Dashboard", "Data");
                    }
                } else {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            // Удаляет аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

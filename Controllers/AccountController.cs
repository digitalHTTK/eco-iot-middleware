using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plan_io_T.ViewModels;
using Plan_io_T.Models;
using Microsoft.AspNetCore.Identity;

namespace Plan_io_T.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                var result = await _userManager.AddToRoleAsync(user, "none");
                if (!result.Succeeded) {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                // Добавляем пользователя
                result = await _userManager.CreateAsync(user, model.Password);
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

        // Для создания усеров по умолчанию в базу данных
        [HttpPost]
        public async Task<IActionResult> CreateDefaultUsers() {
            User defaultAdmin = new User {
                Email = "r6a1999@gmail.com",
                UserName = "digitalHTTK",
                Name = "Grigoriy",
                Surname = "Rogov",
                ReqText = "-",
                Role = "admin"
            };
            User defaultUser = new User {
                Email = "classic@gmail.com",
                UserName = "def",
                Name = "Di",
                Surname = "Fence",
                ReqText = "I'm default user",
                Role = "user"
            };

            var result = await _userManager.CreateAsync(defaultAdmin, "123q456w");
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            result = await _userManager.CreateAsync(defaultUser, "abc123");
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            result = await _userManager.AddToRoleAsync(defaultAdmin, "admin");
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            result = await _userManager.AddToRoleAsync(defaultUser, "user");
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // Для ролей те же грабли
        [HttpPost]
        public async Task<IActionResult> CreateRoles() {
            IdentityRole role1 = new IdentityRole { Name = "admin" };
            IdentityRole role2 = new IdentityRole { Name = "user" };
            IdentityRole role3 = new IdentityRole { Name = "none" };

            var result = await _roleManager.CreateAsync(role1);
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            result = await _roleManager.CreateAsync(role2);
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            result = await _roleManager.CreateAsync(role3);
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Index", "Home");
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

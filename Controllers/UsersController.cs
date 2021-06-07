using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Plan_io_T.Models;
using Plan_io_T.ViewModels;
using System.Collections.Generic;

namespace Plan_io_T.Controllers {
    [Authorize(Roles = "admin")]
    public class UsersController : Controller {
        UserManager<User> _userManager;
        // Для создания ролей (код создания уже удален)
        RoleManager<IdentityRole> _roleManager;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model) {
            if (ModelState.IsValid) {
                User user = new User {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    ReqText = model.ReqText
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    await _userManager.AddToRoleAsync(user, "user");
                    return RedirectToAction("Index");
                } else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id) {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                ReqText = user.ReqText
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model) {
            if (ModelState.IsValid) {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null) {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.ReqText = model.ReqText;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) {
                        return RedirectToAction("Index");
                    } else {
                        foreach (var error in result.Errors) {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id) {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        // Методы для изменения пароля
        public async Task<IActionResult> ChangePassword(string id) {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        // Post-метод для смены пароля, тут используется метод ChangePasswordAsync для ввода и старого, и нового пароля
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (ModelState.IsValid) {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null) {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded) {
                        return RedirectToAction("Index");
                    } else {
                        foreach (var error in result.Errors) {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                } else {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ActivateUser(string id) {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                user.Role = "user";
                IdentityResult result = await _userManager.AddToRoleAsync(user, "user");
            }
            return RedirectToAction("Index");
        }
    }
}

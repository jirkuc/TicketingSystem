using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Models;
using TicketingSystem.ViewModels;

namespace TicketingSystem.Controllers; 
public class AccountController : Controller {
    private UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl) {
        LoginViewModel loginViewModel = new LoginViewModel();
        loginViewModel.ReturnUrl = returnUrl;
        return View(loginViewModel);
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login) {
        if (ModelState.IsValid) {
            AppUser appUser = await _userManager.FindByNameAsync(login.UserName);
            if (appUser != null) {
                await _signInManager.SignOutAsync();
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.RememberMe, false);
                if (result.Succeeded) {
                    return Redirect(login.ReturnUrl ?? "/");
                }
            }
            ModelState.AddModelError(nameof(login.UserName), "Login Failed: Invalid UserName or Password");
        }
        return View(login);
    }
    [HttpGet]
    public async Task<IActionResult> Logout() {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult AccessDenied() {
        return View();
    }
}

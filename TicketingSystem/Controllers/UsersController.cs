using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Models;
using TicketingSystem.ViewModels;

namespace TicketingSystem.Controllers;
[Authorize]
public class UsersController : Controller {
    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;
    private ApplicationDbContext _appDbContext;
    private IPasswordHasher<AppUser> _passwordHasher;
    private IPasswordValidator<AppUser> _passwordValidator;


    public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext appDbContext, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) {
        _userManager = userManager;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
        _passwordHasher = passwordHasher;
        _passwordValidator = passwordValidator;
    }


    // GET: AppUsersController
    public async Task<IActionResult> Index() {
        var users = await GetAllUsersViewModel();
        return View(users);
    }

    // GET: AppUsersController/Create
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateAsync() {

        await FillSelectsAsync();
        return View();
    }

    // POST: AppUsersController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAsync(AppUserViewModel appUserViewModel) {
        if (ModelState.IsValid) {
            AppUser newUser = new AppUser {
                UserName = appUserViewModel.UserName,
                Email = appUserViewModel.Email,
                PhoneNumber = appUserViewModel.PhoneNumber,
                DefaultRole = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == appUserViewModel.DefaultRoleId),
                IsActive = true,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, appUserViewModel.Password);
            if (result.Succeeded) {
                IdentityResult resultAddToRole = await _userManager.AddToRoleAsync(newUser, newUser.DefaultRole.NormalizedName);
                if (resultAddToRole.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    ProcessErrors(resultAddToRole);
                }
            }
            else {
                ProcessErrors(result);
            }
        }
        await FillSelectsAsync();
        return View(appUserViewModel);
    }

    // GET: AppUsersController/Details/5
    public async Task<IActionResult> Details(string id) {
        if (id == null) {
            return View("NotFound");
        }
        var appUser = await _userManager.Users
            .Include(user => user.DefaultRole)
            .FirstOrDefaultAsync(user => user.Id == id);
        if (appUser == null) {
            return View("NotFound");
        }
        return View(ModelToViewModel(appUser));
    }


    // GET: AppUsersController/Edit/5
    public async Task<IActionResult> Edit(string id) {
        if (id == null) {
            return View("NotFound");
        }
        var appUser = await _userManager.FindByIdAsync(id);

        if (appUser == null) {
            return View("NotFound");
        }
        await FillSelectsAsync();
        return View(appUser);
    }

    // POST: AppUsersController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAsync(string id, AppUser modifiedUser, string password) {
        AppUser userToEdit = await _userManager.FindByIdAsync(id);
        if (userToEdit == null) {
            ModelState.AddModelError("", "User not found.");
        }
        else if (!(userToEdit.ConcurrencyStamp).Equals(modifiedUser.ConcurrencyStamp)) {
            ModelState.AddModelError("", "User has been changed by a different instance, go back and try again.");
        }
        else if (!string.IsNullOrWhiteSpace(modifiedUser.Email) && !string.IsNullOrWhiteSpace(password)) {
            bool defaultRoleChanged = false;
            AppRole oldDefaultRole = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == userToEdit.DefaultRoleId);
            AppRole newDefaultRole = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == modifiedUser.DefaultRoleId);
            if (!(oldDefaultRole.NormalizedName).Equals(newDefaultRole.NormalizedName)) {
                userToEdit.DefaultRole = newDefaultRole;
                defaultRoleChanged = true;
            }
            userToEdit.IsActive = ((bool)modifiedUser.IsActive);
            userToEdit.Email = modifiedUser.Email;
            userToEdit.PhoneNumber = modifiedUser.PhoneNumber;
            userToEdit.UpdateDate = DateTime.UtcNow;
            IdentityResult validPassword = await _passwordValidator.ValidateAsync(_userManager, userToEdit, password);
            if (validPassword.Succeeded) {
                userToEdit.PasswordHash = _passwordHasher.HashPassword(userToEdit, password);
                IdentityResult identityResult = await _userManager.UpdateAsync(userToEdit);
                if (identityResult.Succeeded) {

                    if (defaultRoleChanged) {
                        IdentityResult resultRemoveFromRole = await _userManager.RemoveFromRoleAsync(userToEdit, oldDefaultRole.NormalizedName);
                        IdentityResult resultAddToRole = await _userManager.AddToRoleAsync(userToEdit, newDefaultRole.NormalizedName);
                        if (!resultRemoveFromRole.Succeeded) {
                            ProcessErrors(resultRemoveFromRole);
                        }
                        else if (!resultAddToRole.Succeeded) {
                            ProcessErrors(resultAddToRole);
                        }
                        else {
                            return RedirectToAction("Index");
                        }
                    }
                    return RedirectToAction("Index");
                }
                else {
                    ProcessErrors(identityResult);
                }
            }
            else {
                ProcessErrors(validPassword);
            }
        }
        await FillSelectsAsync();
        return View(userToEdit);
    }

    // GET: AppUsersController/Delete/5
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(string id) {
        if (id == null) {
            return View("NotFound");
        }
        var appUser = await _userManager.FindByIdAsync(id);
        if (appUser == null) {
            return View("NotFound");
        }
        return View(appUser);
    }

    // POST: AppUsersController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAsync(string id) {
        var appUser = await _userManager.FindByIdAsync(id);
        if (appUser == null) {
            ModelState.AddModelError("", "User not found.");
            return View(appUser);
        }
        else if (appUser.IsActive) {
            ModelState.AddModelError("", "User is still active, deactivate the user first.");
            return View(appUser);
        }
        else {
            IdentityResult result = await _userManager.DeleteAsync(appUser);
            if (result.Succeeded) {
                return RedirectToAction(nameof(Index));
            }
            else {
                ProcessErrors(result);
            }
        }
        return View("Index", _userManager.Users);
    }

    private void ProcessErrors(IdentityResult identityResult) {
        foreach (var error in identityResult.Errors) {
            ModelState.AddModelError("", error.Description);
        }
    }
    public async Task<RoleDropdownViewModel> GetRoleDropdownsData() {
        AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == _userManager.GetUserId(User));
        AppRole appRole = await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == appUser.DefaultRoleId);
        string appUserDefaultRole = appRole.NormalizedName;
        var rolesDropDownData = new RoleDropdownViewModel();
        if (appUserDefaultRole == "ADMIN") {
            rolesDropDownData = new RoleDropdownViewModel() {
                Roles = await _roleManager.Roles
                .Where(role => role.IsActive == true)
                .OrderBy(role => role.NormalizedName)
                .ToListAsync()
            };
        }
        else {
            rolesDropDownData = new RoleDropdownViewModel() {
                Roles = await _roleManager.Roles
                .Where(role => role.IsActive == true)
                .Where(role => role.Id == appRole.Id)
                .ToListAsync()
            };
        }
        return rolesDropDownData;
    }

    private async Task FillSelectsAsync() {
        var rolesDropDownData = await GetRoleDropdownsData();
        ViewBag.RolesDropDownData = new SelectList(rolesDropDownData.Roles, "Id", "NormalizedName");
    }

    public async Task<IEnumerable<AppUserViewModel>> GetAllUsersViewModel() {
        List<AppUser> users = await _userManager.Users
            .Include(user => user.DefaultRole)
            .OrderBy(user => user.NormalizedUserName)
            .OrderBy(user => user.DefaultRole.NormalizedName)
            .ToListAsync();
        List<AppUserViewModel> usersViewModel = new List<AppUserViewModel>();
        foreach (var user in users) {
            usersViewModel.Add(ModelToViewModel(user));
        }
        usersViewModel
            .OrderBy(user => user.DefaultRoleId)
            .OrderBy(user => user.NormalizedUserName);
        return usersViewModel;
    }

    public AppUserViewModel ModelToViewModel(AppUser user) {
        return new AppUserViewModel {
            UserName = user.UserName,
            NormalizedUserName = user.NormalizedUserName,
            Email = user.Email,
            DefaultRoleId = user.DefaultRoleId,
            DefaultRoleName = user.DefaultRole.NormalizedName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreateDate = user.CreateDate,
            UpdateDate = user.UpdateDate,
            Id = user.Id,
            ConcurrencyStamp = user.ConcurrencyStamp
        };
    }
}

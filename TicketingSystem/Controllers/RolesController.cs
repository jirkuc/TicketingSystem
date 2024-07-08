using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TicketingSystem.Models;

namespace TicketingSystem.Controllers {
    [Authorize(Roles ="ADMIN")]
    public class RolesController : Controller {
        private RoleManager<AppRole> _roleManager;
        private UserManager<AppUser> _userManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: Roles

        public async Task<IActionResult> Index() {
            return View(await _roleManager.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> DetailsAsync(string id) {
            if (id == null) {
                return View("NotFound");
            }
            var appRole = await _roleManager.Roles
                .FirstOrDefaultAsync(role => role.Id == id);
            if (appRole == null) {
                return View("NotFound");
            }
            return View(appRole);
        }

        // GET: Roles/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(AppRole appRole) {
            if (ModelState.IsValid) {
                IdentityResult result = await _roleManager.CreateAsync(new AppRole {
                    Name = appRole.Name,
                    IsActive = true,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    ConcurrencyStamp = HashCode.Combine(appRole.Name, DateTime.UtcNow).ToString()
                });
                if (result.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    ProcessErrors(result);
                }
            }
            return View(appRole);
        }


        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id) {
            if (id == null) {
                return View("NotFound");
            }

            var appRole = await _roleManager.FindByIdAsync(id);
            if (appRole == null) {
                return View("NotFound");
            }
            return View(appRole);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("IsActive,UpdateDate,Name")] AppRole appRole) {
        public async Task<IActionResult> EditAsync(string id, bool isActive, string concurrencyStamp) {
            AppRole appRole = await _roleManager.FindByIdAsync(id);
            if (appRole == null) {
                ModelState.AddModelError("", "Role not found.");
            }
            // else if (!AppRoleExists(appRole.Id)) {
            //     return View("NotFound");
            //     return NotFound();
            // }
            else if (!appRole.ConcurrencyStamp.Equals(concurrencyStamp)) {
                ModelState.AddModelError("", "Role has been changed by a different instance, go back and try again.");
            }
            else if (appRole.IsActive == isActive) {
                ModelState.AddModelError("", "Nothing has changed");
            }
            else {
                IdentityResult result;
                if (ModelState.IsValid) {
                    appRole.IsActive = isActive;
                    appRole.UpdateDate = DateTime.UtcNow;
                    result = await _roleManager.UpdateAsync(appRole);
                    if (!result.Succeeded) {
                        ProcessErrors(result);
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(appRole);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id) {
            if (id == null) {
                return View("NotFound");
            }

            var appRole = await _roleManager.FindByIdAsync(id);
            if (appRole == null) {
                return View("NotFound");
            }

            return View(appRole);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) {
            var appRole = await _roleManager.FindByIdAsync(id);
            if (appRole == null) {
                ModelState.AddModelError("", "Role not found.");
                return View(appRole);
            }
            else if (appRole.IsActive) {
                ModelState.AddModelError("", "Role is still active, deactivate the role first.");
                return View(appRole);
            }
            else if (await UsersInRoleExist(appRole.NormalizedName)) {
                ModelState.AddModelError("", "There are still users assigned to this role, re-assign them first.");
                return View(appRole);
            }
            else {
                IdentityResult result = await _roleManager.DeleteAsync(appRole);
                if (result.Succeeded) {
                    return RedirectToAction(nameof(Index));
                }
                else {
                    ProcessErrors(result);
                }
            }
            return View("Index", _roleManager.Roles);

        }

        private bool AppRoleExists(string id) {
            return _roleManager.Roles.Any(role => role.Id == id);
        }

        private void ProcessErrors(IdentityResult identityResult) {
            foreach (var error in identityResult.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }
        private async Task<bool> UsersInRoleExist(string roleName) {
            int usersInRole = await _userManager.Users.Where(user => user.DefaultRole.NormalizedName == roleName).CountAsync();
            //int usersInRole = await _userManager.Users.Where(user => _userManager.IsInRoleAsync(user, roleName).Result).CountAsync();
            return usersInRole > 0;
        }
    }
}

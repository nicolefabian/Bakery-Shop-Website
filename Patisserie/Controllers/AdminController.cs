using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Patisserie.Data;
using Patisserie.Models.DB;
using System.Data;

namespace Patisserie.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Role()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRole(IdentityRole model)
        {
            //checks if the role exists or not
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Role");
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]

        public async Task<IActionResult> EditRole(IdentityRole model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();
            }
            role.Name = model.Name;
            //execute the update
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Role");
            }
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("DeleteRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            //deleting if theres one
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Role");
            }
            return View(role);
        }
    }
}

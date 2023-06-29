using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Patisserie.Data;
using Patisserie.Models.DB;

namespace Patisserie.Controllers
{
    [Authorize] // must have an account to access 
    public class MemberController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        private readonly UserManager<ApplicationUser> _userManager;

        //constructor
        public MemberController(FSWD2023fabi18Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //found the solution to get currently logged in using this: https://stackoverflow.com/questions/38751616/asp-net-core-identity-get-current-user

        // GET: Member
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Staff"))
            {
                return _context.Members != null
                    ? View(await _context.Members.ToListAsync())
                    : Problem("Entity set 'FSWD2023fabi18Context.Members' is null.");
            }
            else if (User.Identity.IsAuthenticated)
            {
                // get the currently logged-in user
                var user = await _userManager.GetUserAsync(User);
                string userEmail = user.Email;

                if (user != null)
                {
                    // retrieve member record based on currently logged in user
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == userEmail);

                    if (member != null)
                    {
                        // check if the membership expired
                        if (member.MembershipExpiry < DateTime.Now)
                        {
                            ViewBag.MembershipExpiry = "Your membership has expired. You will NO longer receive discount perks. Please contact us to renew!";
                        }
                        // return the member record to view
                        return View(new List<Member> { member });
                    }
                }
               
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Administrator, Staff")]
        // GET: Member/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Staff")]
        // POST: Member/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FirstName,LastName,Email,PhoneNumber,Membership,MembershipExpiry,MembershipDuration")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [Authorize(Roles = "Administrator, Staff")]
        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        [Authorize(Roles = "Administrator, Staff")]
        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FirstName,LastName,Email,PhoneNumber,Membership,MembershipExpiry,MembershipDuration")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [Authorize(Roles = "Administrator")] // allow administrators to access this 
        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [Authorize(Roles = "Administrator")] // allow administrators to access this 
        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'FSWD2023fabi18Context.Members'  is null.");
            }
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return (_context.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}

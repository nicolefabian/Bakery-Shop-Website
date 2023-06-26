﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Patisserie.Data;
using Patisserie.Models.DB;

namespace Patisserie.Controllers
{
    public class OrderController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        private readonly UserManager<ApplicationUser> _userManager;

        //constructor
        public OrderController(FSWD2023fabi18Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CheckOut()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            string userEmail = user.Email;

            if (userEmail != null)
            {
                // Retrieve the member record based on the logged-in user's email
                var member = _context.Members.FirstOrDefault(m => m.Email == userEmail);

                if (member != null)
                {
                    // Get the cart items
                    var cartItems = GetCartItems();

                    if (cartItems != null && cartItems.Any())
                    {
                        // Create a new order
                        var order = new Order
                        {
                            Email = userEmail, // Set the customer's email
                            FirstName = user.FirstName, // Set the customer's first name
                            LastName = user.LastName, // Set the customer's last name
                            Total = CalculateTotal(cartItems),
                            MemberId = member.MemberId // Set the member ID
                        };

                        // Save the order to the database
                        _context.Orders.Add(order);
                        _context.SaveChanges();

                        // Clear the cart items
                        ClearCart();

                        return RedirectToAction("Index", "Order");
                    }
                    else
                    {
                        // Handle the case when the cart is empty
                        ModelState.AddModelError(string.Empty, "The cart is empty. Please add items to your cart before checking out.");
                    }
                }
            }

            return View("Error"); // Handle the case when the member is not found or there is no logged-in user
        }

        private decimal CalculateTotal(List<CartItem> cartItems)
        {
            decimal total = 0;

            foreach (var cartItem in cartItems)
            {
                total += cartItem.TotalAmount;
            }

            return total;
        }

        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.Set("CartItems", cartItems); // Update your cart in the session
        }

        public IActionResult ClearCart()
        {
            SaveCartItems(new List<CartItem>());
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCartItems()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }
            return cartItems;
        }

        private decimal GetDiscountPercentage(string membershipLevel, DateTime membershipExpiry)
        {
            decimal discountPercentage = 0.0m;

            // Check if the membership is still valid
            if (membershipExpiry > DateTime.Now.Date)
            {
                if (membershipLevel == "Gold")
                {
                    discountPercentage = 0.14m;
                }
                else if (membershipLevel == "Silver")
                {
                    discountPercentage = 0.09m;
                }
                else if (membershipLevel == "Bronze")
                {
                    discountPercentage = 0.06m;
                }
            }

            return discountPercentage;
        }





        // GET: Order
        public async Task<IActionResult> Index()
        {
            return _context.Orders != null ?
                        View(await _context.Orders.ToListAsync()) :
                        Problem("Entity set 'FSWD2023fabi18Context.Orders'  is null.");
        }

        /*    public async Task<IActionResult> Index()
            {
                var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
                if (cartItems != null)
                {
                    // Get the currently logged-in user
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user != null)
                    {
                        // Get the user's membership level
                        var membershipType = user.Membership;
                        var membershipExpiry = user.MembershipExpiry.Date;
                        decimal discountPercentage = GetDiscountPercentage(membershipType, membershipExpiry);

                        // Apply the membership discount to cart items and calculate total price after discount
                        decimal totalPrice = 0;
                        foreach (var item in cartItems)
                        {
                            decimal itemPrice = item.Product.Price; // Get the price of the product
                            decimal discountedPrice = itemPrice - (itemPrice * discountPercentage); // Calculate the discounted price
                            decimal totalPriceForItem = discountedPrice * item.Quantity; // Calculate the total price for the item (discounted price * quantity)
                            item.TotalAmount = totalPriceForItem; // Update the total amount for the item

                            totalPrice += totalPriceForItem; // Add the item's total price to the overall total price
                        }
                        ViewBag.CartItems = cartItems;
                        ViewBag.MembershipMessage = $"You are a {membershipType} member. You have a {discountPercentage:P0} discount";
                        ViewBag.DiscountedTotal = totalPrice.ToString("N2");
                    }
                }

                return View();
            }*/


        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,Email,FirstName,LastName,Total,CustomerId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,Email,FirstName,LastName,Total,CustomerId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'FSWD2023fabi18Context.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}

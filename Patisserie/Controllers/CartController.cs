using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Patisserie.Data;
using Patisserie.Models.DB;

namespace Patisserie.Controllers
{
    public class CartController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        private readonly UserManager<ApplicationUser> _userManager;

        //constructor
        public CartController(FSWD2023fabi18Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        /* public async Task<IActionResult> Index()
         {
             var cart = SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart");
             if (cart != null)
             {
                 var user = await _userManager.FindByNameAsync(User.Identity.Name);

                 MemberShip member = new MemberShip()
                 {

                     FirstName = user.FirstName,
                     LastName = user.LastName,
                     Email = user.Email,
                     Membership = user.Membership,
                     MembershipDuration = user.MembershipDuration,
                     MembershipExpiry = user.MembershipExpiry
                 };

                 if (member.Membership == "gold" && member.MembershipExpiry > DateTime.Now)
                 {
                     ViewBag.Cart = cart;

                     //Hosam: string message to display the discount amount to each user
                     string msg = "As a Gold member - Your order includes a 14% Discount";

                     //Akhil: calculations of doscount and total for each member
                     double discount = 0.14;
                     decimal total = cart.Sum(item => item.Cake.Price * item.Quantity) * decimal.Parse(discount.ToString());
                     decimal discounted = cart.Sum(item => item.Cake.Price * item.Quantity) - total;

                     ViewBag.msg = msg;
                     ViewBag.total = Math.Round(discounted, 2);
                     ViewBag.Discount = Math.Round(discount * 100, 2);
                 }
                 else if (member.Membership == "silver" && member.MembershipExpiry > DateTime.Now)
                 {
                     ViewBag.Cart = cart;

                     //Hosam: string message to display the discount amount to each user
                     string msg = "As a Silver member - Your order includes a 9% Discount";

                     //Akhil: calculations of doscount and total for each member
                     double discount = 0.09;
                     decimal total = cart.Sum(item => item.Cake.Price * item.Quantity) * decimal.Parse(discount.ToString());
                     Math.Round(total, 2);
                     decimal discounted = cart.Sum(item => item.Cake.Price * item.Quantity) - total;

                     ViewBag.msg = msg;
                     ViewBag.total = Math.Round(discounted, 2);
                     ViewBag.Discount = Math.Round(discount * 100, 2);
                 }
                 else if (member.Membership == "bronze" && member.MembershipExpiry > DateTime.Now)
                 {
                     ViewBag.Cart = cart;

                     //Hosam: string message to display the discount amount to each user
                     string msg = "As a Bronze member - Your order includes a 6% Discount";

                     //Akhil: calculations of doscount and total for each member
                     double discount = 0.06;
                     decimal total = cart.Sum(item => item.Cake.Price * item.Quantity) * decimal.Parse(discount.ToString());
                     Math.Round(total, 2);
                     decimal discounted = cart.Sum(item => item.Cake.Price * item.Quantity) - total;

                     //returning discount message using viewBag
                     ViewBag.msg = msg;
                     ViewBag.total = Math.Round(discounted, 2);
                     ViewBag.Discount = Math.Round(discount * 100, 2);
                 }
                 else
                 {
                     ViewBag.Cart = cart;

                     decimal total = cart.Sum(item => item.Cake.Price * item.Quantity);

                     ViewBag.total = Math.Round(total, 2);
                 }

             }
             else if (cart == null)
                 ViewBag.Empty = "Empty";


             return View();
         }*/
        public async Task<IActionResult> Index()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }
            else
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

                    // Set the ViewBag message, discounted total price, and item prices
                    ViewBag.MembershipMessage = $"You are a {membershipType} member. You have a {discountPercentage:P0} discount. Your membership expiry is {membershipExpiry.Date.ToString("yyyy-MM-dd")}";
                    ViewBag.DiscountedTotal = totalPrice.ToString("N2");
                }

            }

            return View(cartItems);
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






        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                var cartItems = GetCartItems();

                var existingCartItem = cartItems.FirstOrDefault(item => item.Product.ProductId == id);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++; // Increase the quantity if the item is already in the cart
                }
                else
                {
                    cartItems.Add(new CartItem { Product = product, Quantity = 1 }); // Add a new item if it doesn't exist in the cart
                }

                SaveCartItems(cartItems); // Update cart in the session
            }
            return RedirectToAction("Index");
        }


        private void UpdateCartItemQuantity(List<CartItem> cartItems, Product product)
        {
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == product.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem { Product = product, Quantity = 1 });
            }
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var cartItems = GetCartItems();
            var cartItem = cartItems.FirstOrDefault(item => item.Product.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--; // Decrease the quantity by 1
                    cartItem.TotalAmount = cartItem.Quantity * cartItem.Product.Price; // Update the subtotal price
                }
                else
                {
                    cartItems.Remove(cartItem); // Remove the item from the cart if the quantity is 1
                }

                SaveCartItems(cartItems); // Update your cart in the session
            }

            return RedirectToAction("Index");
        }


        public IActionResult IncreaseQuantity(int id)
        {
            var cartItems = GetCartItems();
            var cartItem = cartItems.FirstOrDefault(item => item.Product.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Increase the quantity by 1
                cartItem.TotalAmount = cartItem.Quantity * cartItem.Product.Price; // Update the subtotal price
            }

            SaveCartItems(cartItems); // Update your cart in the session

            return RedirectToAction("Index");
        }


        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.Set("CartItems", cartItems); // Update your cart in the session
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cartItems = GetCartItems();
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    cartItems.Remove(cartItem);
                }
            }
            SaveCartItems(cartItems);
            return RedirectToAction("Index");
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
    }
}


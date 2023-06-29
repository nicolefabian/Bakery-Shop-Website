using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Patisserie.Data;
using Patisserie.Models.DB;

namespace Patisserie.Controllers
{
    [Authorize] //allow authenticated users to access
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

        public async Task<IActionResult> Index()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }
            else
            {
                // Get the currently logged-in member
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {

                    decimal totalPrice = 0;
                    foreach (var item in cartItems)
                    {
                        decimal itemPrice = item.Product.Price; // Get the price of the product
                        decimal totalPriceForItem = itemPrice * item.Quantity;
                        item.TotalAmount = totalPriceForItem; // Update the total amount for the item

                        totalPrice += totalPriceForItem;
                    }

                    SaveCartItems(cartItems);
                    ViewBag.Message = "Discounts will apply upon proceeding to checkout";
                    ViewBag.Total = totalPrice.ToString("N2");
                }
            }
            return View(cartItems);
        }

        //retrieves cart 
        private List<CartItem> GetCartItems()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("CartItems");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
            }

            return cartItems;
        }

        //allows users to add products to cart
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

        //decrease product quantity in the cart
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

        //increase product quantity in the cart
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

        //save products in sesson
        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.Set("CartItems", cartItems); // Update cart in the session
        }

        //remove products from the cart 
        public IActionResult RemoveFromCart(int productId)
        {
            var cartItems = GetCartItems();
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItems.Remove(cartItem); // Remove the cart item from the list
            }
            SaveCartItems(cartItems);
            return RedirectToAction("Index");
        }

        //removes all products from the cart
        public IActionResult ClearCart()
        {
            SaveCartItems(new List<CartItem>());
            return RedirectToAction("Index");
        }
    

    }
}



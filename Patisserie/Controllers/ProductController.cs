using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Patisserie.Models.DB;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Patisserie.Controllers
{
    public class ProductController : Controller
    {
        private readonly FSWD2023fabi18Context _context;
        //for image
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(FSWD2023fabi18Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /*// GET: Product
        public async Task<IActionResult> Index()
        {
            var fSWD2023fabi18Context = _context.Products.Include(p => p.Category);
            return View(await fSWD2023fabi18Context.ToListAsync());
        }*/

        /*   public ActionResult FilteredProducts(string category, string searchString, int? page)
           {
               var pageNumber = page ?? 1;

               var product = _context.Products.Where(ct => ct.Category.Name == category).Include(c => c.Category);
               if (!String.IsNullOrEmpty(searchString))
               {
                   product = product.Where(p => p.Name.Contains(searchString)).Include(c => c.Category);
               }
               return View(product.ToPagedList(pageNumber, 9));
           }*/

        /*//modified to take parameters
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var pageNumber = page ?? 1;

            var product = _context.Products.Include(p => p.Category);
            if (!String.IsNullOrEmpty(searchString))
            {
                product = product.Where(p => p.Name.Contains(searchString)).Include(c => c.Category);
            }
            return View(product.ToPagedList(pageNumber, 9));
        }*/


        // Modified to take parameters
        public async Task<IActionResult> Index(string searchString, string category, int? page)
        {
            var pageNumber = page ?? 1;

            var product = _context.Products.Include(p => p.Category);

            if (!String.IsNullOrEmpty(searchString))
            {
                product = product.Where(p => p.Name.Contains(searchString)).Include(c => c.Category);
            }
            if (!String.IsNullOrEmpty(category))
            {
                if (category == "New")
                {
                    product = product.Where(p => p.Category.Name == "New").Include(c => c.Category);
                }
                else if (category == "Popular")
                {
                    product = product.Where(p => p.Category.Name == "Popular").Include(c => c.Category);
                }
                else if (category == "Special")
                {
                    product = product.Where(p => p.Category.Name == "Special").Include(c => c.Category);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(product.ToPagedList(pageNumber, 10));
        }
  

        //search for a product
        public string IndexAJAX(string searchString)
        {
            string sql = "SELECT * FROM Product WHERE Name LIKE @p0";
            string wrapString = "%" + searchString + "%";
            List<Product> prod = _context.Products.FromSqlRaw(sql, wrapString).ToList();
            string json = JsonConvert.SerializeObject(prod);
            return json;
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public ActionResult GetSpecialProducts()
        {
            // Logic to retrieve products with the "special" category
            var specialProducts = _context.Products.Where(p => p.Name == "Special").ToList();

            return View(specialProducts);
        }



        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }


        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,CategoryId,Image")] Product product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    //string uniqueFileName = image.FileName;
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    product.Image = uniqueFileName;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,CategoryId,Image")] Product product, IFormFile image)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    //string uniqueFileName = image.FileName;
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    product.Image = uniqueFileName;
                } 
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'FSWD2023fabi18Context.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}

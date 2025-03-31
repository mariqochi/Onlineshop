//using Microsoft.AspNetCore.Authorization;  // Add for authorization
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Identity;
//using OnlineStore.Interfaces;
//using OnlineStore.Models.Entities;
//using OnlineStore.Models;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace OnlineStore.Controllers
//{
//    // Apply the [Authorize] attribute to restrict the controller to authorized users, but we will specify the Admin role for CRUD actions.
//    [Authorize]
//    public class ProductController : Controller
//    {
//        private readonly IProductService _productService;
//        private readonly UserManager<AppUser> _userManager;
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<ProductController> _logger;



//        // Inject the ProductService and UserManager for Authentication
//        public ProductController(IProductService productService, UserManager<AppUser> userManager, ApplicationDbContext context, ILogger<ProductController> logger)
//        {
//            _productService = productService;
//            _userManager = userManager;
//            _context = context;
//            _logger = logger;

//        }

//        // Show all products with filtering, search, and pagination
//        public async Task<IActionResult> Index(string search, string category, string subcategory, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 10)
//        {
//            var products = await _productService.GetAllProductsAsync(search, category, subcategory, minPrice, maxPrice);

//            // Pass products to view
//            return View(products);
//        }

//        // Show product details (including reviews)
//        public async Task<IActionResult> Details(int id)
//        {
//            var product = await _productService.GetProductByIdAsync(id);
//            if (product == null)
//                return NotFound();

//            return View(product);
//        }

//        // Add product to cart (Only Authenticated Users can add to cart)
//        [HttpPost]
//        public async Task<IActionResult> AddToCart(int productId)
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                // Redirect to login if the user is not authenticated
//                return RedirectToAction("Login", "Account");
//            }

//            var user = await _userManager.GetUserAsync(User);
//            var product = await _productService.GetProductByIdAsync(productId);
//            if (product == null)
//                return NotFound();

//            // Add logic to add the product to the user's cart here

//            // For now, just return a success message
//            return RedirectToAction("Index");
//        }

//        // Add review (Only Authenticated Users can write a review)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                // Redirect to login if the user is not authenticated
//                return RedirectToAction("Login", "Account");
//            }

//            await _productService.AddReviewAsync(productId, rating, reviewText);
//            return RedirectToAction("Details", new { id = productId });
//        }

//        // Admin: Create a product (GET)
//        // GET: Product/Create
//        // Admin: Create a product (GET)
//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public IActionResult Create()
//        {
//            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
//            ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

//            return View(new Product()); // Ensure model is not null
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
//        {
//            // Check if the model is valid
//            if (!ModelState.IsValid)
//            {
//                // If the model is invalid, print the validation errors to the console or log
//                var errors = ModelState.Values.SelectMany(v => v.Errors);
//                foreach (var error in errors)
//                {
//                    // Log the error messages to the console (for debugging purposes)
//                    Console.WriteLine("ModelState Error: " + error.ErrorMessage);  // Or use a logger if available
//                }

//                // Return to the Create view with the product model to show the errors
//                return View(product);
//            }

//            // If the model is valid, proceed with file upload and saving the product
//            if (ImageFile != null && ImageFile.Length > 0)
//            {
//                // Generate a unique file name
//                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
//                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

//                // Upload the file
//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await ImageFile.CopyToAsync(stream);
//                }

//                // Set the relative ImageUrl in the Product model
//                product.ImageUrl = "/Images/" + fileName;
//            }

//            // Use ProductService to add the product (delegating to the service)
//            await _productService.AddProductAsync(product);


//                // Redirect to another page after successful creation (e.g., Index or Product List)
//            return RedirectToAction(nameof(Index));
//        }







//        // Admin: Edit a product (GET)
//        [HttpGet()]
//            [Authorize(Roles = "Admin")]  // Only Admins can access this action
//            public async Task<IActionResult> Edit(int id)
//            {
//                var product = await _productService.GetProductByIdAsync(id);
//                if (product == null)
//                    return NotFound();

//                return View(product);
//            }

//            // Admin: Edit a product (POST)
//            [HttpPost]
//            [Authorize(Roles = "Admin")]  // Only Admins can access this action
//            public async Task<IActionResult> Edit(int id, Product product)
//            {
//                if (id != product.Id)
//                    return NotFound();

//                if (ModelState.IsValid)
//                {
//                    await _productService.UpdateProductAsync(product);
//                    return RedirectToAction(nameof(Index));
//                }

//                return View(product);
//            }

//            // Admin: Delete product (GET)
//            [HttpGet]
//            [Authorize(Roles = "Admin")]  // Only Admins can access this action
//            public async Task<IActionResult> Delete(int id)
//            {
//                var product = await _productService.GetProductByIdAsync(id);
//                if (product == null)
//                    return NotFound();

//                return View(product);
//            }

//            // Admin: Delete product (POST)
//            [HttpPost, ActionName("Delete")]
//            [Authorize(Roles = "Admin")]  // Only Admins can access this action
//            public async Task<IActionResult> DeleteConfirmed(int id)
//            {
//                var product = await _productService.GetProductByIdAsync(id);
//                if (product != null)
//                {
//                    await _productService.DeleteProductAsync(id);
//                    return RedirectToAction(nameof(Index));
//                }

//                return NotFound();
//            }
//        }

//    }

using Microsoft.AspNetCore.Authorization;  // Add for authorization
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Interfaces;
using OnlineStore.Models.Entities;
using OnlineStore.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Controllers
{
    // Apply the [Authorize] attribute to restrict the controller to authorized users, but we will specify the Admin role for CRUD actions.
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;



        // Inject the ProductService and UserManager for Authentication
        public ProductController(IProductService productService, UserManager<AppUser> userManager, ApplicationDbContext context, ILogger<ProductController> logger)
        {
            _productService = productService;
            _userManager = userManager;
            _context = context;
            _logger = logger;

        }

        // Show all products with filtering, search, and pagination
        public async Task<IActionResult> Index(string search, string category, string subcategory, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 10)
        {
            var products = await _productService.GetAllProductsAsync(search, category, subcategory, minPrice, maxPrice);

            // Pass products to view
            return View(products);
        }

        // Show product details (including reviews)
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Add product to cart (Only Authenticated Users can add to cart)
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound();

            // Add logic to add the product to the user's cart here

            // For now, just return a success message
            return RedirectToAction("Index");
        }

        // Add review (Only Authenticated Users can write a review)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            await _productService.AddReviewAsync(productId, rating, reviewText);
            return RedirectToAction("Details", new { id = productId });
        }

        // Admin: Create a product (GET)
        // GET: Product/Create
        // Admin: Create a product (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

            return View(new Product()); // Ensure model is not null
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            // Check if the submitted model is valid
            if (!ModelState.IsValid)
            {
                // Repopulate categories and subcategories if the model is invalid
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

                // Return the view with the current model and validation errors
                return View(product);
            }

            // Handle image file upload (if provided)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                // Save the uploaded image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Assign the relative image URL to the product model
                product.ImageUrl = "/Images/" + fileName;
            }

            // Save the new product using the product service
            await _productService.AddProductAsync(product);

            // Redirect to the index page after successful product creation
            return RedirectToAction("Index");
        }
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        //{
        //    // Check if the model is valid
        //    if (!ModelState.IsValid)
        //    {
        //        // If the model is invalid, print the validation errors to the console or log
        //        var errors = ModelState.Values.SelectMany(v => v.Errors);
        //        foreach (var error in errors)
        //        {
        //            // Log the error messages to the console (for debugging purposes)
        //            Console.WriteLine("ModelState Error: " + error.ErrorMessage);  // Or use a logger if available
        //        }

        //        // Return to the Create view with the product model to show the errors
        //        return View(product);
        //    }

        //    // If the model is valid, proceed with file upload and saving the product
        //    if (ImageFile != null && ImageFile.Length > 0)
        //    {
        //        // Generate a unique file name
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

        //        // Upload the file
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await ImageFile.CopyToAsync(stream);
        //        }

        //        // Set the relative ImageUrl in the Product model
        //        product.ImageUrl = "/Images/" + fileName;
        //    }

        //    // Use ProductService to add the product (delegating to the service)
        //    await _productService.AddProductAsync(product);


        //    // Redirect to another page after successful creation (e.g., Index or Product List)
        //    return RedirectToAction(nameof(Index));
        //}







        // Admin: Edit a product (GET)
        [HttpGet()]
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Admin: Edit a product (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // Admin: Delete product (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // Admin: Delete product (POST)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                await _productService.DeleteProductAsync(id);
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }

}

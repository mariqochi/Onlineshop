
using Microsoft.AspNetCore.Authorization;  // Add for authorization
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Interfaces;
using OnlineStore.Models;
using OnlineStore.Models.Entities;

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
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, string category, string subcategory, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productService.GetAllProductsAsync(search, category, subcategory, minPrice, maxPrice);

            // ✅ Load categories & subcategories for dropdowns
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SubCategories = await _context.SubCategories.ToListAsync();

            return View(products);
        }

        // Show product details (including reviews)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Fetch reviews with the User data (eager loading)
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == id)
                .Include(r => r.User)  // Eager load User
                .ToListAsync();

            product.Reviews = reviews; // Assign reviews to product model

            // Calculate the average rating
            if (reviews.Any())
            {
                product.Rating = reviews.Average(r => r.Rating);
            }
            else
            {
                product.Rating = 0; // If no reviews, set to 0
            }

            return View(product); // Pass the product to the view
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login page if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Retrieve the product by its ID
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                // Return a "not found" response if the product doesn't exist
                return NotFound();
            }
            // Retrieve or create the user's cart
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);

            // If the user doesn't have a cart, create a new one
            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the product is already in the cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                // If the product is already in the cart, just update the quantity
                existingCartItem.Quantity += 1;
            }
            else
            {
                // If the product is not in the cart, add a new CartItem
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1,
                    Price = product.Price  // Save the price of the product at the time it is added to the cart
                };
                _context.CartItems.Add(cartItem);
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the index page or wherever you want
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            var user = await _userManager.GetUserAsync(User);

            // Create a new review object
            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                ReviewText = reviewText,
                UserId = user.Id,
                Date = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId }); // Redirect back to the product details page
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        // Redirect to login if the user is not authenticated
        //        return RedirectToAction("Login", "Account");
        //    }

        //    await _productService.AddReviewAsync(productId, rating, reviewText);
        //    return RedirectToAction("Details", new { id = productId });
        //}
        //Admin: Create a product(GET)
        // GET: Product/Create
        // Admin: Create a product(GET)
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
            // Save image if uploaded
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/Images/" + fileName;
            }

            // ❌ Skipping ModelState check
            await _productService.AddProductAsync(product);

            return RedirectToAction("Index");
        }

        // Admin: Create a product (GET)
        // GET: Product/Create
        // Admin: Create a product (GET)
        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public IActionResult Create()
        //{
        //    ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
        //    ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

        //    return View(new Product()); // Ensure model is not null
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        //{
        //    // Preserve ImageUrl if model validation fails
        //    if (ImageFile != null && ImageFile.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await ImageFile.CopyToAsync(stream);
        //        }

        //        product.ImageUrl = "/Images/" + fileName;
        //        TempData["UploadedImageUrl"] = product.ImageUrl; // Store temporarily
        //    }
        //    else
        //    {
        //        // If no new image was uploaded, keep the existing one
        //        product.ImageUrl = product.ImageUrl ?? TempData["UploadedImageUrl"]?.ToString();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
        //        ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

        //        return View(product); // Preserve values in case of error
        //    }

        //    // ✅ Save the product to the database
        //    await _productService.AddProductAsync(product);

        //    return RedirectToAction("Index");
        //}




        //[HttpPost] mushaobs bolo versiaa posti
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        //{
        //    // Check if the submitted model is valid
        //    if (!ModelState.IsValid)
        //    {
        //        // Repopulate categories and subcategories if the model is invalid
        //        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
        //        ViewBag.SubCategories = new SelectList(_context.SubCategories.ToList(), "Id", "Name");

        //        // Return the view with the current model and validation errors
        //        return View(product);
        //    }

        //    // Handle image file upload (if provided)
        //    if (ImageFile != null && ImageFile.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

        //        // Save the uploaded image to the server
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await ImageFile.CopyToAsync(stream);
        //        }

        //        // Assign the relative image URL to the product model
        //        product.ImageUrl = "/Images/" + fileName;
        //    }

        //    // Save the new product using the product service
        //    await _productService.AddProductAsync(product);

        //    // Redirect to the index page after successful product creation
        //    return RedirectToAction("Index");
        //}



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

        [HttpGet]
        public IActionResult GetSubCategories(int categoryId)
        {
            var subcategories = _context.SubCategories
                .Where(s => s.CategoryId == categoryId)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Json(subcategories);
        }






        // Admin: Edit a product (GET)
        [HttpGet()]
        [Authorize(Roles = "Admin")]  // Only Admins can access this action
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            _logger.LogInformation("Product retrieved: Id = {ProductId}, CategoryId = {CategoryId}, SubCategoryId = {SubCategoryId}",
            product.Id, product.CategoryId, product.SubCategoryId);

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", product.CategoryId);

            var subcategories = _context.SubCategories
                .Where(s => s.CategoryId == product.CategoryId)
                .ToList();

            _logger.LogInformation("Loaded {Count} subcategories for CategoryId {CategoryId}", subcategories.Count, product.CategoryId);


            // Load subcategories based on the selected category
            ViewBag.SubCategories = new SelectList(_context.SubCategories
                                                    .Where(s => s.CategoryId == product.CategoryId)
                                                    .ToList(), "Id", "Name", product.SubCategoryId);


            return View(product);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

            await _productService.UpdateProductAsync(product);


            return RedirectToAction(nameof(Index));
        }
        // Admin: Edit a product (POST)
        //[HttpPost]
        //[Authorize(Roles = "Admin")]  // Only Admins can access this action
        //public async Task<IActionResult> Edit(int id, Product product)
        //{
        //    if (id != product.Id)
        //        return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        await _productService.UpdateProductAsync(product);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(product);
        //}

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

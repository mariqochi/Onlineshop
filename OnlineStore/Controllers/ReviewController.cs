using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Interfaces;
using OnlineStore.Models.ViewModels;
using System.Security.Claims;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IProductService _productService;

    public ReviewController(IReviewService reviewService, IProductService productService)
    {
        _reviewService = reviewService;
        _productService = productService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ReviewViewModel
        {
            ProductId = product.Id,
        };

        return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

        if (userId == null) // Check if userId is null, this is a safeguard
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
        }

        // Add the review
        await _reviewService.AddReviewAsync(model.ProductId, model.Rating, model.ReviewText, userId);

        // Redirect back to product details page
        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }
}








//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using OnlineStore.Interfaces;
//using OnlineStore.Models.ViewModels;
//using System.Security.Claims;

//public class ReviewController : Controller
//{
//    private readonly IReviewService _reviewService;
//    private readonly IProductService _productService;

//    public ReviewController(IReviewService reviewService, IProductService productService)
//    {
//        _reviewService = reviewService;
//        _productService = productService;
//    }

//    [Authorize]
//    [HttpGet]
//    public async Task<IActionResult> Create(int productId)
//    {
//        var product = await _productService.GetProductByIdAsync(productId); // Assume this fetches the product based on ID
//        if (product == null)
//        {
//            return NotFound(); // Product not found
//        }

//        var viewModel = new ReviewViewModel
//        {
//            ProductId = product.Id,
//            // You could also pass FullName if needed here, but typically it’s fetched from the logged-in user
//        };

//        return View(viewModel); // Pass the ViewModel to the view
//    }
//    [Authorize]
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(ReviewViewModel model)
//    {
//        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

//        await _reviewService.AddReviewAsync(model.ProductId, model.Rating, model.ReviewText, userId);

//        return RedirectToAction("Details", "Product", new { id = model.ProductId });
//    }
//}

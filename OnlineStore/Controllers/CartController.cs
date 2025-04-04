using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.Entities;
using OnlineStore.Models.ViewModels;
using OnlineStore.Models;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userManager.GetUserAsync(User);

        // Retrieve the user's cart and its items, including product details
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (cart == null)
        {
            return View("EmptyCart");
        }

        // Map to CartViewModel
        var cartViewModel = new CartViewModel
        {
            CartId = cart.Id,
            CartItems = cart.CartItems.Select(ci => new CartItemViewModel
            {
                CartItemId = ci.Id,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                Price = ci.Price
            }).ToList()
        };

        return View(cartViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}

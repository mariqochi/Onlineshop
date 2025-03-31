using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models.Entities;
using OnlineStore.Models.ViewModels;
using Microsoft.Extensions.Logging;  // For logging
using System.Linq;  // For handling `ModelState` errors
using System.Threading.Tasks;

namespace OnlineStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger; // Injecting logger

        // Single constructor that accepts all dependencies
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Registration (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Registration (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration failed due to invalid model state.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation Error: " + error.ErrorMessage);
                }
                return View(model);
            }

            var user = new AppUser { UserName = model.Email, Email = model.Email };

            _logger.LogInformation($"Creating user: {model.Email}");

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully.");
                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Log each error from result
            foreach (var error in result.Errors)
            {
                _logger.LogError("User creation failed: " + error.Description);
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

            
        
        // Login (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

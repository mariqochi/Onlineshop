using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models.Entities;
using OnlineStore.Models;
using OnlineStore.Interfaces;
using OnlineStore.Services;
using Microsoft.AspNetCore.Http.Features;

namespace OnlineStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Set the file upload size limit (e.g., 100 MB)
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 104857600; // 100 MB
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
           


            // Register the ApplicationDbContext with SQL Server connection string
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Identity services with custom password settings
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;  // Minimum length
                options.Password.RequireDigit = false;  // No digit required
                options.Password.RequireUppercase = false;  // No uppercase required
                options.Password.RequireLowercase = false;  // No lowercase required
                options.Password.RequireNonAlphanumeric = false;  // No special characters required
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Register ProductService and its interface IProductService
            builder.Services.AddScoped<IProductService, ProductService>(); // Register service for DI

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin")); // Ensures "Admin" role policy is configured
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Ensure authentication is enabled
            app.UseAuthorization();

            // Set up the default route for controllers
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ensure that roles exist (e.g., Admin role)
            CreateRoles(app).Wait();

            // Create the admin user if it doesn't exist
            CreateAdminUser(app).Wait();

            // Run the application
            app.Run();
        }

        // Method to create the roles if they do not exist
        private static async Task CreateRoles(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // Method to create the admin user if it doesn't exist
        private static async Task CreateAdminUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            string adminEmail = "mariqochi350@gmail.com";
            string adminPassword = "123456";  // Update password to match new requirements

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AppUser { UserName = adminEmail, Email = adminEmail };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    if (!await userManager.IsInRoleAsync(newAdmin, "Admin")) // Ensure no duplicate role assignment
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
            }
        }
    }
}

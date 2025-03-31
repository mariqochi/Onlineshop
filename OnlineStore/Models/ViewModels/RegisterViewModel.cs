using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        // Ensure only "Admin" or "User" roles can be assigned
        [Required]
        [RegularExpression("Admin|User", ErrorMessage = "Invalid role. Choose either 'Admin' or 'User'.")]
        public string Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace InventoryService.Domain.Entities.Identity;

public class User
{
    [Key]
    public int Id { get; set; }

    // Authentication
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!; // Store hashed password

    /**
        // Refresh Token Support
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Security / Auditing
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        // Optional: Roles and Claims
        public string Role { get; set; } // e.g., "User", "Admin"

        // Optional: Password Reset
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    */
}

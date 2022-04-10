using AspNetCore.JWTDemo.Validates;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.JWTDemo.Dtos
{
    public class UserAuthDto
    {
        [AtLeastOneRequired(nameof(Email))]
        public virtual string? UserName { get; set; }
        [EmailAddress]
        public virtual string? Email { get; set; }
        [Required]
        public virtual string Password { get; set; } = string.Empty;
    }
}

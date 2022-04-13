using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore.JWTDemo.EntityFrameworkCore.Models
{
    public class User : IdentityUser, IHasOwner
    {
        [NotMapped]
        public string OwnerId => Id;
    }
}

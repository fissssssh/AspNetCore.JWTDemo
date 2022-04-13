namespace AspNetCore.JWTDemo.EntityFrameworkCore.Models
{
    public interface IHasOwner
    {
        string OwnerId { get; }
    }
}

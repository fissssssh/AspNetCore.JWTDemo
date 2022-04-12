using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.JWTDemo.Permissions
{
    public class RBACAuthorizationRequirement : IAuthorizationRequirement
    {
        public RBACAuthorizationRequirement(Resource resource, Operation operation)
        {
            Resource = resource;
            Operation = operation;
        }

        public Resource Resource { get; }
        public Operation Operation { get; }
    }
}

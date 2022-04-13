using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.JWTDemo.Permissions
{
    public class RBACAuthorizeAttribute : AuthorizeAttribute
    {
        public RBACAuthorizeAttribute(Resource resource, Operation operation) : base($"{EntityFrameworkCore.Permissions.PolicyDefinitions.RBAC}.{resource}.{(int)operation}")
        {
        }
    }
}
